using System;
using System.ComponentModel;

namespace EFS.Shared.EventModels
{
    public interface IFileTransferStatus : INotifyPropertyChanged
    {
        string FileName { get; set; }
        Guid TransferID { get; set; }
        double Progress { get; }
        long FileSizeBytes { get; set; }
        long TransferredSizeBytes { get; set; }
        bool Complete { get; set; }
        bool Successful { get; set; }
        bool Cancelled { get; set; }
        Exception Exception { get; set; }
        DateTime DateTimeStarted { get; set; }
        double SpeedBytesPerSecond { get; set; }
    }

    public abstract class FileTransferStatus : IFileTransferStatus
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; set; }
        public Guid TransferID { get; set; }
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

        private bool _cancalled;
        public bool Cancelled
        {
            get
            {
                return _cancalled;
            }
            set
            {
                _cancalled = value;
                OnPropertyChanged("Cancelled");
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
