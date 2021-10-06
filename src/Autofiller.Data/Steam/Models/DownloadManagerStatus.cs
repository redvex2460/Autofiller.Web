using System;

namespace Autofiller.Data.Steam.Models
{
    public class DownloadManagerStatus
    {
        private DateTime changeTime = DateTime.Now;
        private DateTime oldChangeTime;
        public DateTime ChangeTime
        {
            get { return changeTime; }
            set { oldChangeTime = changeTime; changeTime = value; }
        }



        public string Action { get; set; }
        public double Progress { get; set; }
        public string Game { get; set; }
        public double DownloadSpeed => Math.Round(((currentBit - OldCurrentBit) / (changeTime - oldChangeTime).Seconds) / 1.049e+6,2);
        private double currentBit = 0;
        public double CurrentBit
        {
            get { return currentBit; }
            set
            {
                OldCurrentBit = CurrentBit;
                currentBit = value;
            }
        }
        public double MaximumBit { get; set; } = 0;

        private double OldCurrentBit;
    }
}
