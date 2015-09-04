using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Setting : IEntity, ILoggedEntity
    {
        public Setting()
        {

        }

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
