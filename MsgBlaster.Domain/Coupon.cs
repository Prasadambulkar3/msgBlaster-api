using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Coupon : IEntity
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string Code { get; set; }
        public bool IsRedeem { get; set; }
        public DateTime? RedeemDateTime { get; set; }
        public string Remark { get; set; }
        
        public int? UserId { get; set; }
        public User User { get; set; }
        public bool IsExpired { get; set; }
        //public string GatewayID { get; set; }        
        public DateTime SentDateTime { get; set; }
        public string MessageId { get; set; }
        public int MessageCount { get; set; }
        //public double MessageCount { get; set; }
        public string Message { get; set; }
        public double? Amount { get; set; }
        public int EcouponCampaignId { get; set; }
        public EcouponCampaign EcouponCampaign { get; set; }
        public double RequiredCredits { get; set; }
        public bool IsCouponSent { get; set; }

        public string BillNumber { get; set; }
        public DateTime? BillDate { get; set; }

    }
}
