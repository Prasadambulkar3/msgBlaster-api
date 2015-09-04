using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class OnlinePaymentDTO
    {
        public string payment_id { get; set; }
        public string status { get; set; }
        public string buyer { get; set; }
        public string buyer_name { get; set; }
        public string currency { get; set; }
        public string quantity { get; set; }
        public string unit_price { get; set; }
        public string amount { get; set; }
        public string fees { get; set; }
        public string mac { get; set; }
    }
}
