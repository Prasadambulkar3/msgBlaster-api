using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class CreditRequest : IEntity, ILoggedEntity
    {
        public CreditRequest()
        {
           
        }

        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public DateTime Date { get; set; }
        public int RequestedCredit { get; set; }
        public int ProvidedCredit { get; set; }
        //public int OldBalance { get; set; }
        public double OldBalance { get; set; }
        public bool IsProvided { get; set; }
        public DateTime? ProvidedDate { get; set; }
        public double RatePerSMS { get; set; }
        public double Amount { get; set; }
        public double? Tax { get; set; }
        public double? TaxAmount { get; set; }
        public double GrandTotal { get; set; }

        public bool? IsBillGenerated { get; set; }

        public int PartnerId { get; set; }
        public Partner Partner { get; set; }


        public PaymentMode? PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public bool IsPaid { get; set; }


        [ForeignKey("User")]
        public int RequestedBy { get; set; }
        public User User { get; set; }

        public string PaymentId { get; set; }

        [NotMapped]
        public string _clientName{ get; set; }
        [NotMapped]
        public string ClientName
        {
            get
            {
                return  string.Format("{0}", _clientName);
            }
        }

        [NotMapped]
        public string _userName{ get; set; }
        [NotMapped]
        public string UserName
        {
            get
            {
                return string.Format("{0}", _userName);
            }          

        }

    }
}
