using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class SettingDTO
    {
        public int Id { get; set; }
        public int MessageLength { get; set; }
        public int SingleMessageLength { get; set; }
        public int MobileNumberLength { get; set; }
        
        public double NationalCampaignSMSCount { get; set; }
        public double NationalCouponSMSCount { get; set; }

        public int UTFFirstMessageLength { get; set; }
        public int UTFSecondMessageLength { get; set; }
        public int UTFNthMessageLength { get; set; } 
    }
}
