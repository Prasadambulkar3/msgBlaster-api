using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class CampaignLogDTO
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public bool IsSuccess { get; set; }
        public string RecipientsNumber { get; set; }

        public string Message { get; set; }
        public string MessageStatus { get; set; }
        public int MessageCount { get; set; }
        //public double MessageCount { get; set; }

        public string GatewayID { get; set; }
        public DateTime SentDateTime { get; set; }
        public string MessageID { get; set; }
        public double RequiredCredits { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy{ get; set; }
    }
}
