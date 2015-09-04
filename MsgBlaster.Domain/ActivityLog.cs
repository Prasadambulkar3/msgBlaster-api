using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class ActivityLog : IEntity
    {
        public ActivityLog()
        {
 
        }

        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public int? PartnerId { get; set; }
        public string OperationType { get; set; }
        public int? ClientId { get; set; }

    }


    //public enum OperationType
    //{
    //    Created, 
    //    Modified, 
    //    Activated,
    //    Inactivated,
    //    Deleted 
    //}

    //public enum EntityType
    //{
    //    Campaign, 
    //    Client, 
    //    Contact,
    //    CreditRequest, 
    //    EcouponCampaign, 
    //    Group, 
    //    Partner, 
    //    Plan, 
    //    RedeemedCount, 
    //    Setting, 
    //    SMSGateway, 
    //    Template, 
    //    User
    //}



}
