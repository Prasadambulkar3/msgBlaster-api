using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class CreditRequestDTO
    {
        public CreditRequestDTO()
        {
             
        }

        public int Id { get; set; }
        public int ClientId { get; set; }    
        public DateTime Date { get; set; }
        public int RequestedCredit { get; set; }
        public int ProvidedCredit { get; set; }
        public int PartnerId { get; set; }
        //public int? PlanId { get; set; }

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
        public string ClientName { get; set; }
        public string UserName { get; set; }

        public int RequestedBy { get; set; }


        public string PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public bool IsPaid { get; set; }

        public string PaymentId { get; set; }

        public string OnlinePaymentURL { get; set; }
    
    }
}
