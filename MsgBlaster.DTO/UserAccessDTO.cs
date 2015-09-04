using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    //public enum Modules
    //{
    //    Groups,
    //    Contacts,
    //    ImportContacts,
    //    Templates,
    //    Users,
    //    Locations,
    //    Campaigns,
    //    CreditRequests,
    //    Coupons,
    //    Settings,
    //    Redeems

    //}
    
    public class UserAccessDTO
    {
        public bool Groups { get; set; }
        public bool Contacts { get; set; }
        public bool ImportContacts { get; set; }     
        public bool Templates { get; set; } 
        public bool Users { get; set; } 
        public bool Locations { get; set; } 
        public bool Campaigns { get; set; } 
        public bool CreditRequests { get; set; }
        public bool Coupons { get; set; } 
        public bool Settings { get; set; }
        public bool Redeems { get; set; }
        public bool SenderCode { get; set; }
         
 
    }


}
