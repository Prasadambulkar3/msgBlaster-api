using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class EcouponCampaign : IEntity, ILoggedEntity
    {
        public EcouponCampaign()
        {
            Coupons = new List<Coupon>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ReceipentNumber { get; set; }
        public int RecipientsCount { get; set; }
        public string Message { get; set; }     
        public DateTime? ExpiresOn { get; set; }
        public DateTime SendOn { get; set; }
        public string ScheduleTime { get; set; }
        public bool IsScheduled { get; set; }

        public bool IsSent { get; set; }
        public string IPAddress { get; set; }


        public int ClientId { get; set; }
        public Client Client { get; set; }

        [ForeignKey("User")]
        public int CreatedBy { get; set; }
        public User User { get; set; }

        //public string Groups { get; set; }
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public bool ForAllContact { get; set; }

        public double MinPurchaseAmount { get; set; }

        public double ConsumedCredits { get; set; }
        public double CreditsDiffrence { get; set; }
        public bool IsReconcile { get; set; }
        public DateTime ReconcileDate { get; set; }
        public double RequiredCredits { get; set; }

        public List<Coupon> Coupons { get; set; }
    }
}
