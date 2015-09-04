using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class RedeemedCountDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }
        public int EcouponCampaignId { get; set; }
        public string CampaignName { get; set; }
        public int RedeemCount { get; set; }        
    }

    public class LocationWiseRedeemedCountDTO
    {
        //public int UserId { get; set; }
        //public int EcouponCampaignId { get; set; }
        public string Location { get; set; }
        public int RedeemCount { get; set; }
        public string UserName { get; set; }
        public string CampaignName { get; set; }
    }

}
