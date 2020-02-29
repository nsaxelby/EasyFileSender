using EFS.Global.Models;

namespace EFS.WindowsFormApp.ViewModels
{
    public class ClientInfoViewModelListItem : ClientInfo
    {
        public bool IsSelfClient { get; set; }
        public bool AddedManually { get; set; }

        public ClientInfoViewModelListItem()
        {

        }

        public ClientInfoViewModelListItem(ClientInfo inClientInfo, bool isSelfClient, bool addedManually)
        {
            IpAddress = inClientInfo.IpAddress;
            ClientType = inClientInfo.ClientType;
            Version = inClientInfo.Version;
            IsSelfClient = isSelfClient;
            AddedManually = addedManually;
        }
    }
}
