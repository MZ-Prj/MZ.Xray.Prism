using System.Threading;

namespace MZ.Xray.Engine
{
    public class ProcessThread
    {
        public bool IsRunning;
        public int LazyTime = 1000;
        public Thread Thread;
    }
}
