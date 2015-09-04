using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class PaymentDTO
    {
        public payment payment { get; set; }
        public bool success { get; set; }
    }

    public class payment
    { 

        public bool success { get; set; }
        public OnlinePaymentDTO OnlinePaymentInfo { get; set; }
        public string payment_id { get; set; }
        public string amount { get; set; }
        public string buyer_name { get; set; }
        public string buyer_phone { get; set; }
        public string buyer_email { get; set; }
        public custom_fields custom_fields { get; set; }
    }

    public class custom_fields
    {
        //public string Field_52722 { get; set; }
        public Field_52722 Field_52722 { get; set; }
    }

    public class Field_52722
    {
        public string value { get; set; }
    }


}
