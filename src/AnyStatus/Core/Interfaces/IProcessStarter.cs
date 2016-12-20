using System;
using System.Diagnostics;

namespace AnyStatus
{
    public interface IProcessStarter
    {
        void Start(string fileName);


        int Start(ProcessStartInfo info, TimeSpan timeout);
    }
}
