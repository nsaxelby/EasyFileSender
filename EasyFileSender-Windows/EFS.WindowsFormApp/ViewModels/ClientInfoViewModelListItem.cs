using EFS.Global.Models;

namespace EFS.WindowsFormApp.ViewModels
{
    public class ClientInfoViewModelListItem : ClientInfo
    {
        public bool IsSelfClient { get; set; }

        public ClientInfoViewModelListItem()
        {

        }

        public ClientInfoViewModelListItem(ClientInfo inClientInfo, bool isSelfClient)
        {
            IpAddress = inClientInfo.IpAddress;
            ClientType = inClientInfo.ClientType;
            Version = inClientInfo.Version;
            IsSelfClient = isSelfClient;
        }
    }
}
