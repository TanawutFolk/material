using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RawMat.Utilities
{
    public interface IParent
    {
        bool TryCreateMutex(string mutexKey, out Mutex mutex);
        void ReleaseReportMutex(string mutexKey);
    }
}
