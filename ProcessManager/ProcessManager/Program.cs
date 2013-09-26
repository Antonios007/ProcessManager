using System;
using System.Threading;
using System.ServiceProcess;
using System.Diagnostics;


namespace ProcessManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Restarting TT");
            TimerCallback callback = new TimerCallback(Tick);
            // create a one second timer tick
            Timer stateTimer = new Timer(callback, null, 0, 1000);

            Process[] proc = Process.GetProcessesByName("x_trader");
            if (proc.Length > 0)
            {
                //works but X-trader popup confirm stops exit
                //proc[0].CloseMainWindow();
                try
                {
                    proc[0].Kill();
                }
                catch { Console.WriteLine("\nX_TRADER MUST BE RE-STARTED!"); }
            }

            stopService("TTSimManager");
            stopService("guardian");
            stopService("ttmd");
            Console.WriteLine("\nAll Services Stopped.");
            Thread.Sleep(1000);

            startService("ttmd");
            startService("TTSimManager");

            Process.Start("C:\\tt\\Guardian\\GuardianStart.exe");
            Console.WriteLine("Guardian Re-started");
        }

        private static void Tick(Object stateInfo)
        {
            Console.Write(".");
        }

        private static void stopService(string service)
        {
            ServiceController srvc = new ServiceController(service);
            try
            {

                srvc.Stop();
                srvc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));

                Console.WriteLine("\n{0} : {1}", srvc.DisplayName, srvc.Status);
            }
            catch (Exception ex)
            {
                Process[] proc = Process.GetProcessesByName(service);

                if (proc.Length > 0)
                    proc[0].WaitForExit(10);

                if (!Equals(srvc.Status, ServiceControllerStatus.Stopped))
                {
                    if (!proc[0].HasExited)
                        proc[0].Kill();

                    if (proc[0].HasExited)
                    { Console.WriteLine("\nProcess Killed : {0}", proc[0].ProcessName); }
                    else
                    {
                        Console.WriteLine("ERROR: {0}\nHIT ANY KEY TO CONTINUE\nYou may need to run this application again.", ex.Message);
                    }
                }
            }
        }

        private static void startService(string service)
        {
            try
            {
                ServiceController srvc = new ServiceController(service);
                srvc.Start();
                srvc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                Console.WriteLine("\n{0} : {1}", srvc.DisplayName, srvc.Status);

            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
        }
    }
}

