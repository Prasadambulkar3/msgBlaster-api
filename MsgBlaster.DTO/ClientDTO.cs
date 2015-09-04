using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }
        //public string Name { get; set; }
        //public string Password { get; set; }
        public string Address { get; set; }
        //public string Email { get; set; }
        public string Company { get; set; }
        //public string Mobile { get; set; }
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
        public bool AlertOnCredit { get; set; }
        public int? AlertCreditBalanceLimit { get; set; }
        //public int SMSCredit { get; set; }
        public double SMSCredit { get; set; }

        public int TotalAppliedCredit { get; set; }
        public int TotalProvidedCredit { get; set; }

        public string SenderCode { get; set; }
        public string SenderCodeFilePath { get; set; }
        public bool IsDatabaseUploaded { get; set; }

        public int? PlanId { get; set; }
        public int PartnerId { get; set; }   
        public int SMSGatewayId { get; set; }

        public bool IsMailSend { get; set; }

        public bool AllowChequePayment { get; set; }

        public string DefaultCouponMessage { get; set; }
        public int DefaultCouponExpire { get; set; }
        public string CouponExpireType { get; set; }

        //Properties for auto send birthday and anniversary messages
        public string BirthdayMessage { get; set; }
        public string AnniversaryMessage { get; set; }
        public int? BirthdayCouponExpire { get; set; }
        public string BirthdayCouponExpireType { get; set; }
        public int? AnniversaryCouponExpire { get; set; }
        public string AnniversaryCouponExpireType { get; set; }
        public bool IsSendBirthdayMessages { get; set; }
        public bool IsSendBirthdayCoupons { get; set; }
        public bool IsSendAnniversaryMessages { get; set; }
        public bool IsSendAnniversaryCoupons { get; set; }
        public double? MinPurchaseAmountForBirthdayCoupon { get; set; }
        public double? MinPurchaseAmountForAnniversaryCoupon { get; set; }
    }
}
