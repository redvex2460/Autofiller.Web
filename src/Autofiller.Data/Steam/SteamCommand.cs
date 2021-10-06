using Autofiller.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Autofiller.Data.Steam
{
    public class SteamCommand
    {
        #region Public Properties
        DataManager DataManager => DataManager.GetInstance();
        public string Arguments { get; set; }
        public string InstallDir { get; set; } = null;
        public string Output { get; set; }
        public SteamPlatforms? Platform { get; set; } = null;
        public Process Process { get; set; }
        public bool Result { get; set; }
        public bool IsRunning { get; private set; }

        public delegate void MessageReadHandler(string message);
        public event MessageReadHandler MessageRead;

        #endregion Public Properties

        #region Public Methods

        public static SteamCommand Init()
        {
            return new SteamCommand();
        }

        public SteamCommand AddArgument(string property, string value = "")
        {
            if (value == "")
                Arguments += $" +{property}";
            else
                Arguments += $" +{property} {value}";
            return this;
        }

        public SteamCommand Execute(TimeSpan? timeout = null, TimeSpan? idleTimeout = null)
        {
            IsRunning = true;
            AddArgument("quit");
            timeout = timeout != null ? timeout : TimeSpan.FromMinutes(240);
            idleTimeout = idleTimeout != null ? idleTimeout : TimeSpan.FromMinutes(6);

            Process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = $"{DataManager.ScriptPath} {Arguments}",
                    CreateNoWindow = true,
                    FileName = "unbuffer",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = DataManager.ScriptDirectory
                }
            };
            Process.OutputDataReceived += (object sender, DataReceivedEventArgs message) => { MessageRead?.Invoke(message.Data); Console.WriteLine(message.Data); };
            Process.ErrorDataReceived += (object sender, DataReceivedEventArgs message) => { MessageRead?.Invoke(message.Data); Console.WriteLine(message.Data); };
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            Process.WaitForExit(timeout.Value.Milliseconds);
            while (!Process.HasExited)
            {
                Thread.Sleep(500);
            }
            Result = Process.ExitCode == 0;
            IsRunning = false;
            return this;
        }

        public SteamCommand Install()
        {
            string url = "http://media.steampowered.com/client/steamcmd_linux.tar.gz";
            string _folder = Path.Combine(Directory.GetCurrentDirectory(), "steam");
            string _file = Path.Combine(_folder, "steam.tar.gz");

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
                var webClient = new WebClient();
                webClient.DownloadFile(url, _file);
                var succeeded = Utils.StartProcess<bool>("tar", $"zxvf {_file}", _folder);
                File.Delete(_file);
                if (succeeded)
                {
                    Console.WriteLine("Successfully SteamCMD inizialised");
                    return Login("anonymous").Execute();
                }
                else
                {
                    Console.WriteLine("Error inizialing SteamCMD");
                    Environment.Exit(1);
                }
            }
            return this;
        }

        public SteamCommand Login(string username, string password = null, string guard = null)
        {
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                AddArgument("login", $"{username}");
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && string.IsNullOrEmpty(guard))
                AddArgument("login", $"{username} {password}");
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(guard))
                AddArgument("login", $"{username} {password} {guard}");
            return this;
        }

        public SteamCommand SetDirectory(string path)
        {
            AddArgument("force_install_dir", path);
            InstallDir = path;
            return this;
        }

        public SteamCommand SetPlatform(SteamPlatforms platform)
        {
            AddArgument("@sSteamCmdForcePlatformType", platform.ToString());
            Platform = platform;
            return this;
        }
        public SteamCommand Update(long appId)
        {
            if (InstallDir == null)
                SetDirectory(DataManager.Settings.DownloadDirectory);
            if (Platform == null)
                SetPlatform(SteamPlatforms.windows);
            AddArgument("app_license_request", appId.ToString());
            AddArgument("app_update", appId.ToString());
            return this;
        }
        public SteamCommand Kill()
        {
            Process.Kill(true);
            return this;
        }
        #endregion Public Methods
    }
}