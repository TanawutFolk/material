using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;

namespace RawMat.Utilities
{

    public class MqttManager : IDisposable
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _mqttOptions;
        private readonly string _clientId;
        private readonly string _brokerHost;
        private readonly int _brokerPort;
        private readonly Dictionary<string, (string payload, DateTime lastHeartbeat)> _reportLocks;
        private CancellationTokenSource _heartbeatCts;
        private Task _heartbeatTask; // เก็บ Task เพื่อตรวจสอบสถานะ

        public event EventHandler<MqttMessageReceivedEventArgs> MessageReceived;

        public MqttManager(string brokerHost = "localhost", int brokerPort = 1883)
        {
            _clientId = $"MqttClient_{Guid.NewGuid().ToString()}";
            _brokerHost = brokerHost;
            _brokerPort = brokerPort;
            _reportLocks = new Dictionary<string, (string, DateTime)>();
            _heartbeatCts = new CancellationTokenSource();

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_brokerHost, _brokerPort)
                .Build();

            SetupMqttClient();
        }

        private void SetupMqttClient()
        {
            _mqttClient.UseConnectedHandler(e =>
            {
                Console.WriteLine("MQTT Connected");
                _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic("report/lock/+/+")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build()).Wait();
            });

            _mqttClient.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("MQTT Disconnected, attempting to reconnect...");
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            await Task.Delay(5000);
                            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Reconnect failed: {ex.Message}");
                        }
                    }
                });
            });

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"Received: {topic} - {payload}");

                if (topic.StartsWith("report/lock/"))
                {
                    var parts = topic.Split('/');
                    if (parts.Length == 4)
                    {
                        var reportNo = parts[2];
                        var process = parts[3];
                        var key = $"report/lock/{reportNo}/{process}";
                        var payloadParts = payload.Split('|');
                        if (payloadParts[0] == "lock" && payloadParts.Length == 2)
                        {
                            _reportLocks[key] = (payload, DateTime.UtcNow);
                        }
                        else if (payloadParts[0] == "heartbeat" && payloadParts.Length == 2)
                        {
                            if (_reportLocks.ContainsKey(key))
                            {
                                var currentPayload = _reportLocks[key].payload;
                                if (currentPayload.Split('|')[1] == payloadParts[1])
                                {
                                    _reportLocks[key] = (currentPayload, DateTime.UtcNow);
                                }
                            }
                        }
                        else if (payload == "release")
                        {
                            _reportLocks.Remove(key);
                        }
                    }
                }

                MessageReceived?.Invoke(this, new MqttMessageReceivedEventArgs(topic, payload));
            });

            try
            {
                _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MQTT Connection Error: {ex.Message}");
            }
        }

        public async Task<bool> TryLockResource(string reportNo, string process)
        {
            var topic = $"report/lock/{reportNo}/{process}";
            if (IsResourceLocked(reportNo, process))
            {
                return false;
            }

            var timestamp = DateTime.UtcNow.Ticks;
            var payload = $"lock|{timestamp}";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithRetainFlag(true)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message, CancellationToken.None);

            _reportLocks[topic] = (payload, DateTime.UtcNow);

            // เริ่มส่ง heartbeat ใน background
            _heartbeatTask = Task.Run(() => SendHeartbeat(topic, timestamp, _heartbeatCts.Token));

            return true;
        }

        private async Task SendHeartbeat(string topic, long timestamp, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(10000, cancellationToken).ConfigureAwait(false); // ส่งทุก 10 วินาที
                    var payload = $"heartbeat|{timestamp}";
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(Encoding.UTF8.GetBytes(payload))
                        .WithRetainFlag(false)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .Build();

                    await _mqttClient.PublishAsync(message, CancellationToken.None).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // การยกเลิกจาก CancellationToken ไม่ต้องจัดการ
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Heartbeat error for {topic}: {ex.Message}");
            }
        }

        public async Task ReleaseResource(string reportNo, string process)
        {
            var topic = $"report/lock/{reportNo}/{process}";
            if (_reportLocks.ContainsKey(topic))
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes("release"))
                    .WithRetainFlag(true)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _mqttClient.PublishAsync(message, CancellationToken.None);

                _reportLocks.Remove(topic);
                _heartbeatCts.Cancel();
                _heartbeatCts = new CancellationTokenSource();
            }
        }

        public async Task PublishMessage(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithRetainFlag(false)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }

        public bool IsResourceLocked(string reportNo, string process)
        {
            var topic = $"report/lock/{reportNo}/{process}";
            if (!_reportLocks.ContainsKey(topic))
            {
                return false;
            }

            var (payload, lastHeartbeat) = _reportLocks[topic];
            var parts = payload.Split('|');
            if (parts.Length != 2 || parts[0] != "lock") return false;

            var currentTime = DateTime.UtcNow;
            var timeSinceLastHeartbeat = (currentTime - lastHeartbeat).TotalSeconds;
            if (timeSinceLastHeartbeat > 20)
            {
                _reportLocks.Remove(topic);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _heartbeatCts.Cancel();
            if (_heartbeatTask != null && !_heartbeatTask.IsCompleted)
            {
                _heartbeatTask.Wait(); // รอให้ heartbeat เสร็จก่อน dispose
            }
            _mqttClient?.DisconnectAsync(CancellationToken.None).Wait();
            _mqttClient?.Dispose();
        }
    }

    public class MqttMessageReceivedEventArgs : EventArgs
    {
        public string Topic { get; }
        public string Payload { get; }

        public MqttMessageReceivedEventArgs(string topic, string payload)
        {
            Topic = topic;
            Payload = payload;
        }
    }

}
