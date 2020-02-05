using System;
using System.ComponentModel;

namespace EFS.Global.Models
{
    public class FileTransferStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Guid TransferID { get; set; }
        public string DestinationIP { get; set; }
        public string SourceFile { get; set; }
        public double Progress { get { return _transferredSizeBytes * 100.0 / FileSizeBytes; } }
        public long FileSizeBytes { get; set; }

        private long _transferredSizeBytes;
        public long TransferredSizeBytes 
        {
            get 
            {
                return _transferredSizeBytes;
            }
            set 
            {
                _transferredSizeBytes = value;
                OnPropertyChanged("TransferredSizeBytes");
            }
        }

        private bool _complete;
        public bool Complete
        {
            get
            {
                return _complete;
            }
            set
            {
                _complete = value;
                OnPropertyChanged("Complete");
            }
        }

        private bool _successful;
        public bool Successful
        {
            get
            {
                return _successful;
            }
            set
            {
                _successful = value;
                OnPropertyChanged("Successful");
            }
        }
        public Exception Exception { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
