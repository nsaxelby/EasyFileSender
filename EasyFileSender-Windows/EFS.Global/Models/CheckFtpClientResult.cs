namespace EFS.Global.Models
{
    public class CheckFtpClientResult
    {
        public bool ClientFound { get; set; }
        public string FailureMessage { get; set; }
        public ClientInfo ClientInfo { get; set; }
    }
}
