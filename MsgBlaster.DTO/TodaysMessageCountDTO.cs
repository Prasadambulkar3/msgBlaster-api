using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class TodaysMessageCountDTO
    {
        public int TodaysTotalMessageSent { get; set; }
        public int TodaysTotalCouponMessageSent { get; set; }
        public int TodaysTotalCampaignMessageSent { get; set; }
        public double TotalPendingCreditRequest { get; set; }
    }
}
