using System.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace UninstallService
{
    internal static class ServiceHelper
    {
        private static string NzbDroneExe
        {
            get
            {
                return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "nzbdrone.exe");
            }
        }

        private static bool IsAnAdministrator()
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        internal static void Run(string arg)
        {
            if (!File.Exists(NzbDroneExe))
            {
                Console.WriteLine("Unable to find NzbDrone.exe in the current directory.");
                return;
            }

            if (!IsAnAdministrator())
            {
                Console.WriteLine("Access denied. Please run as administrator.");
                return;
            }

            var startInfo = new ProcessStartInfo
                                {
                                    FileName = NzbDroneExe,
                                    Arguments = arg,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true
                                };

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += (OnDataReceived);
            process.ErrorDataReceived += (OnDataReceived);

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

        }

        private static void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

    }
}