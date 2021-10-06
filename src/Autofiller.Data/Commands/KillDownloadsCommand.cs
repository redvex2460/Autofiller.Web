using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Autofiller.Data.Commands
{
    public class KillDownloadsCommand : ICommand<bool>
    {
        public bool Result { get; set; }

        public ICommand<bool> Execute()
        {
            foreach(var process in Process.GetProcessesByName("steamcmd"))
            {
                process.Kill();
            }
            return this;
        }
    }
}
