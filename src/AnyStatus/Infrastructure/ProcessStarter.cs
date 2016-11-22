using System.Diagnostics;

namespace AnyStatus
{
    public class ProcessStarter : IProcessStarter
    {
        public void Start(string fileName)
        {
            Process.Start(fileName);
        }
    }
}
