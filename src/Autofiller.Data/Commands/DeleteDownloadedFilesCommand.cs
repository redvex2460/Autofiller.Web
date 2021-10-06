using Autofiller.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Autofiller.Data.Commands
{
    internal class DeleteDownloadedFilesCommand : ICommand<bool>
    {
        DataManager DataManager => DataManager.GetInstance();
        public bool Result { get; set; }

        public ICommand<bool> Execute()
        {
            Result = true;
            if (Directory.Exists(DataManager.Settings.DownloadDirectory))
                Result = Utils.StartProcess<bool>("rm", $"-rf {DataManager.Settings.DownloadDirectory}");
            return this;
        }
    }
}
