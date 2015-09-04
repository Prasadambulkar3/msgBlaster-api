using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class CouponDTO
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string Code { get; set; }
        public bool IsRedeem { get; set; }
        public DateTime? RedeemDateTime { get; set; }
        public string Remark { get; set; }
        public bool IsExpired { get; set; }
        //public string GatewayID { get; set; }
        public DateTime SentDateTime { get; set; }
        public string MessageId { get; set; }
        public int MessageCount { get; set; }
        //public double MessageCount { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string CouponCampaignName { get; set; }
        public string Message { get; set; }
        public int EcouponCampaignId { get; set; }
        public int ClientId { get; set; }
        public double? Amount { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public double RequiredCredits { get; set; }
        public bool IsCouponSent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string BillNumber { get; set; }
        public DateTime? BillDate { get; set; }
        public double MinPurchaseAmount { get; set; }

    }
}
