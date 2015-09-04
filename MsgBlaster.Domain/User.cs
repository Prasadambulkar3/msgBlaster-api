using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MsgBlaster.Domain
{
    public class User : IEntity, ILoggedEntity
    {
        public User()
        {
            Coupons = new List<Coupon>();
            //Campaigns = new List<Campaign>();
            //CreditRequests = new List<CreditRequest>();
            //EcouponCampaigns = new List<EcouponCampaign>();               
        }

        public int Id{get; set;}
        //public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
       // public string Address { get; set; }
       // public string Location { get; set; }

       
        public int LocationId { get; set; }
        public Location Location { get; set; }
   
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Mobile { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public UserType UserType { get; set; }


        [NotMapped]
        public string Name
        {
            get
            {
                if (LastName == null && LastName == "")
                {
                    return string.Format("{0}", FirstName);
                }
                else
                    return string.Format("{0}", FirstName + " " + LastName);
            }
            //set
            //{
            //    Name = FirstName + " " + LastName;
            //}
        }

        public List<Coupon> Coupons { get; set; }
        //public List<Campaign> Campaigns { get; set; }
        //public List<CreditRequest> CreditRequests { get; set; }
        //public List<EcouponCampaign> EcouponCampaigns { get; set; }


      }
}
