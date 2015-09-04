using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class RedeemedCount : IEntity, ILoggedEntity
    {
        public RedeemedCount()
        { 
        }

        public int Id { get; set; }

        //[ForeignKey("Client")]
        public int ClientId { get; set; }
        //public Client Client { get; set; }

        //[ForeignKey("User")]
        public int UserId { get; set; }
        //public User User { get; set; }

        //[ForeignKey("EcouponCampaign")]
        public int EcouponCampaignId { get; set; }
        //public EcouponCampaign EcouponCampaign { get; set; }

        public int RedeemCount { get; set; }


         
    }
}
