using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics;


namespace ProcessManager
{
    class Program
    {
        static void Main(string[] args)
        {
            stopService("TTSimManager");
            Console.WriteLine("Stopped TTSIM MGR");
            Console.ReadKey();
            stopService("guardian");
            Console.WriteLine("Stopped guardian");
            Console.ReadKey();
            stopService("ttmd");
            Console.WriteLine("Stopped ttmd");
            Console.ReadKey();
        }

        private static void stopService(string service)
        {
            ServiceController srvc = new ServiceController(service);
            srvc.Stop();
            srvc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            if (!Equals(srvc.Status, ServiceControllerStatus.Stopped))
            {
                Process[] proc = Process.GetProcessesByName(service);
                proc[0].Kill();
            }
        }
    }
}

