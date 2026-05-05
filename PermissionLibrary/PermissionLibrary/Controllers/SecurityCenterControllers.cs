using Newtonsoft.Json;
using PermissionLibrary.Property;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PermissionLibrary
{
    public class SecurityCenterControllers
    {
        public SignInProperty SignIn(string userName, string password)
        {
            try
            {

                SignInProperty dataItem;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Properties.Settings.Default.BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string md5Password = CreateMD5(password);

                HttpResponseMessage response = client.GetAsync("http://192.168.14.235:7010/api/security-center/SignIn?data={\"USER_NAME\":\"" + userName + "\",\"USER_PASSWORD\":\"" + password + "\"}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var test = response.Content.ReadAsStringAsync().Result;
                    //dataItem = JsonConvert.DeserializeObject<SignInProperty>(response.Content.ReadAsStringAsync().Result);
                    var generatedType = JsonConvert.DeserializeObject<SignInProperty>(response.Content.ReadAsStringAsync().Result, new CustomBooleanJsonConverter());
                    dataItem = (SignInProperty)Convert.ChangeType(generatedType, typeof(SignInProperty));
                }
                else
                {
                    dataItem = new SignInProperty
                    {
                        Status = false,
                        Message = response.ReasonPhrase
                    };
                }

                return dataItem;
            }
            catch (Exception ex)
            {
                SignInProperty dataItem = new SignInProperty
                {
                    Status = false,
                    Message = ex.Message
                };

                return dataItem;
            }
        }

        public PermissionCheckProperty PermissionCheck(string applicationName, string menuName, string userGroupName, string userName)
        {
            try
            {

                PermissionCheckProperty dataItem;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Properties.Settings.Default.BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //string url = "api/security-center/permission-check?data={\"APPLICATION_NAME\":\"" + applicationName + "\", \"MENU_NAME\" : \"" + menuName + "\", \"USER_GROUP_NAME\":\"" + userGroupName + "\",\"USER_NAME\":\"" + userName + "\"}";

                HttpResponseMessage response = client.GetAsync("http://192.168.14.235:7010/api/security-center/permission-check?data={\"APPLICATION_NAME\":\"" + applicationName + "\", \"MENU_NAME\" : \"" + menuName + "\", \"USER_GROUP_NAME\":\"" + userGroupName + "\",\"USER_NAME\":\"" + userName + "\"}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var generatedType = JsonConvert.DeserializeObject<PermissionCheckProperty>(response.Content.ReadAsStringAsync().Result, new CustomBooleanJsonConverter());
                    dataItem = (PermissionCheckProperty)Convert.ChangeType(generatedType, typeof(PermissionCheckProperty));
                    if (dataItem.ResultOnDb.Count == 0)
                    {
                        dataItem.Status = false;
                        dataItem.Message = userName + " is not have permission.";
                    }
                }
                else
                {
                    dataItem = new PermissionCheckProperty
                    {
                        Status = false,
                        Message = response.ReasonPhrase
                    };
                }

                return dataItem;
            }
            catch (Exception ex)
            {
                PermissionCheckProperty dataItem = new PermissionCheckProperty
                {
                    Status = false,
                    Message = ex.Message
                };

                return dataItem;
            }
        }

        public GetUserGroupProperty GetUserGroup(string userName)
        {
            try
            {
                GetUserGroupProperty dataItem;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Properties.Settings.Default.BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/security-center/getUserGroup?data={\"USER_NAME\":\"" + userName + "\"}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var generatedType = JsonConvert.DeserializeObject<GetUserGroupProperty>(response.Content.ReadAsStringAsync().Result, new CustomBooleanJsonConverter());
                    dataItem = (GetUserGroupProperty)Convert.ChangeType(generatedType, typeof(GetUserGroupProperty));
                }
                else
                {
                    dataItem = new GetUserGroupProperty
                    {
                        Status = false,
                        Message = response.ReasonPhrase
                    };
                }

                return dataItem;
            }
            catch (Exception ex)
            {
                GetUserGroupProperty dataItem = new GetUserGroupProperty
                {
                    Status = false,
                    Message = ex.Message
                };

                return dataItem;
            }
        }

        public class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    }
}
