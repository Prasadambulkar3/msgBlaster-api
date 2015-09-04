using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Partner : IEntity, ILoggedEntity
    {
        public Partner()
        {            
            Receipts = new List<Receipt>();
            Clients = new List<Client>();
            CreditRequests = new List<CreditRequest>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Mobile{ get; set; }
        public string Email { get; set; }
        
        public string Password { get; set; }
        public bool AlertOnCredit { get; set; }
        public int? AlertCreditBalanceLimit { get; set; }

        //public UserType UserType { get; set; }
       
        public List<Receipt> Receipts { get; set; }
        public List<Client> Clients { get; set; }
        public List<CreditRequest> CreditRequests { get; set; }
    }
}
