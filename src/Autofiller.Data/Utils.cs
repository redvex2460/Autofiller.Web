using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Autofiller.Data
{
    public static class Utils
    {
        public static T StartProcess<T>(string processName, string arguments, string Workfolder = "")
        {
            var result = "";
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = arguments,
                    CreateNoWindow = true,
                    FileName = processName,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
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
            return default;
        }

        public static async Task<List<Tsource>> ToListAsync<Tsource>(this IEnumerable<Tsource> data)
        {
            List<Tsource> result = new List<Tsource>();
            await Task.Run(() =>
            {
                foreach(var d in data)
                {
                    result.Add(d);
                }
                return;
            });
            return result;
        }
    }
}
