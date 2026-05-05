using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RawMat.Utilities
{
    public class MutexManager
    {
        private static MutexManager _instance;
        private static readonly object _lock = new object();
        private Dictionary<string, Mutex> reportMutexes = new Dictionary<string, Mutex>();

        private MutexManager() { }

        public static MutexManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MutexManager();
                    }
                    return _instance;
                }
            }
        }

        public bool TryCreateMutex(string mutexKey, out Mutex mutex)
        {
            mutex = new Mutex(true, mutexKey, out bool mutexCreated);
            if (!mutexCreated)
            {
                mutex.Dispose();
                return false;
            }
            reportMutexes[mutexKey] = mutex;
            Console.WriteLine($"Mutex {mutexKey} added. Count: {reportMutexes.Count}");
            return true;
        }

        public void ReleaseMutex(string mutexKey)
        {
            if (reportMutexes.ContainsKey(mutexKey))
            {
                try
                {
                    reportMutexes[mutexKey].ReleaseMutex();
                    reportMutexes[mutexKey].Dispose();
                    Console.WriteLine($"Mutex {mutexKey} released successfully.");
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine($"Mutex {mutexKey} already released: {ex.Message}");
                }
                finally
                {
                    reportMutexes.Remove(mutexKey);
                    Console.WriteLine($"Mutex {mutexKey} removed. Count: {reportMutexes.Count}");
                }
            }
        }

        public void ReleaseAllMutexes()
        {
            foreach (var mutexKey in reportMutexes.Keys.ToList())
            {
                ReleaseMutex(mutexKey);
            }
        }
    }
}
