using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.DTO.Enums;

namespace MsgBlaster.DTO
{
    public class ReceiptDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ReceiptNumber { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public float Amount { get; set; }
        public int ClientId { get; set; }
        public int PartnerId { get; set; }
    }
}
