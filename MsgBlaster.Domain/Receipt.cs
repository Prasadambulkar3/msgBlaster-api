using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Receipt : IEntity, ILoggedEntity
    {
        public Receipt()
        {

        }

        public int Id { get; set; }
               
        public DateTime Date { get; set; }
        public int ReceiptNumber { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public float Amount { get; set; }
        
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }


    }
}
