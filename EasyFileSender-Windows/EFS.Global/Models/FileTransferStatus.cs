using System;
using System.ComponentModel;

namespace EFS.Global.Models
{
    public class FileTransferStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Guid TransferID { get; set; }
        public String DestinationIP { get; set; }
        public string SourceFile { get; set; }
        public double Progress { get { return TransferredSizeBytes * 100.0 / FileSizeBytes; } }
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
        private double _bytesPerSecond;
        public double SpeedBytesPerSecond
        {
            get
            {
                if (DateTimeStarted != null)
                {
                    if (Complete == false)
                    {
                        TimeSpan timeElapsed = DateTime.UtcNow - DateTimeStarted;
                        _bytesPerSecond = TransferredSizeBytes / timeElapsed.TotalSeconds;
                        return _bytesPerSecond;
                    }
                    else
                    {
                        return _bytesPerSecond;
                    }
                }
                else
                {
                    return 0.0;
                }
            }
        }

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
