using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class MessageSentCountDTO
    {
        public string Days { get; set; }
        public int TotalCount { get; set; }
        public int CampaignCount { get; set; }
        public int EcouponCampaignCount { get; set; }


    }
}
