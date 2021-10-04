using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Autofiller.Data
{
    public static class Utils
    {
        public static T StartProcess<T>(string processName, string arguments, string Workfolder = "")
        {
            var result = "";
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = processName,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            if (Workfolder != "")
                process.StartInfo.WorkingDirectory = Workfolder;
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => { result += $"\n{e.Data}"; };
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { result += $"\n{e.Data}"; };
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            if(typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(process.ExitCode == 0, typeof(T));
            }
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default(T);
        }
    }
}
