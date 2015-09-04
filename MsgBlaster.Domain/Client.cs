using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Client : IEntity, ILoggedEntity
    {
        public Client()
        {
            Contacts = new List<Contact>();
            Groups = new List<Group>();
            Templates = new List<Template>();
            Campaigns = new List<Campaign>();
            CreditRequests = new List<CreditRequest>();            
            EcouponCampaigns = new List<EcouponCampaign>();
            Users = new List<User>();
            Locations = new List<Location>();
        }

        public int Id { get; set; }
        //public string Name { get; set; }    
   
        //public string Password { get; set; }
        //public string Email { get; set; }

        public string Address { get; set; }        
        public string Company { get; set; }
        //public string Mobile { get; set; }
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
        public bool AlertOnCredit { get; set; }
        public int? AlertCreditBalanceLimit { get; set; }        
        //public int SMSCredit { get; set; }
        public double SMSCredit { get; set; }

        public int? PlanId { get; set; }
        public Plan Plan { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }

        //public int SMSGatewayId { get; set; }
        //public SMSGateway SMSGateway { get; set; }

        public int TotalAppliedCredit { get; set; }
        public int TotalProvidedCredit { get; set; }


        public string SenderCode { get; set; }
        public string SenderCodeFilePath { get; set; }
       

        public bool IsDatabaseUploaded { get; set; }
        public bool AllowChequePayment { get; set; }

        public string DefaultCouponMessage { get; set; }
        public int? DefaultCouponExpire { get; set; }
        public CouponExpireType CouponExpireType { get; set; }

        //Properties for auto send birthday and anniversary messages
        public string BirthdayMessage { get; set; }
        public string AnniversaryMessage { get; set; }
        public int? BirthdayCouponExpire { get; set; }
        public CouponExpireType BirthdayCouponExpireType { get; set; }
        public int? AnniversaryCouponExpire { get; set; }
        public CouponExpireType AnniversaryCouponExpireType { get; set; }
        public bool IsSendBirthdayMessages { get; set; }
        public bool IsSendBirthdayCoupons { get; set; }
        public bool IsSendAnniversaryMessages { get; set; }
        public bool IsSendAnniversaryCoupons { get; set; }
        public double? MinPurchaseAmountForBirthdayCoupon { get; set; }
        public double? MinPurchaseAmountForAnniversaryCoupon { get; set; }

        public List<Contact> Contacts { get; set; }
        public List<Group> Groups { get; set; }
        public List<Template> Templates { get; set; }
        public List<Campaign> Campaigns { get; set; }
        public List<CreditRequest> CreditRequests { get; set; }       
        public List<EcouponCampaign> EcouponCampaigns { get; set; }
        public List<User> Users { get; set; }
        public List<Location> Locations { get; set; }
    }
}
