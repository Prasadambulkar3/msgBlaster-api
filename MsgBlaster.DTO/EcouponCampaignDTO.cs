using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class EcouponCampaignDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReceipentNumber { get; set; }
        public int RecipientsCount { get; set; }
        public string Message { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public DateTime SendOn { get; set; }
        public string ScheduleTime { get; set; }
        public bool IsScheduled { get; set; }
        public bool IsSent { get; set; }
        public string IPAddress { get; set; }
        public int ClientId { get; set; }
        public int ReedeemedCount { get; set; }
        public int TotalCount { get; set; }

        public int CreatedBy { get; set; }

        //public string Groups { get; set; }
        //public List<GroupDTO> GroupDTOList { get; set; }
        public int? GroupId { get; set; }
        public string Group { get; set; }
        public int? GroupContactCount { get; set; }
        public bool ForAllContact { get; set; }
        public TemplateDTO TemplateDTO { get; set; }

        public double MinPurchaseAmount { get; set; }

        public double ConsumedCredits { get; set; }
        public double CreditsDiffrence { get; set; }
        public bool IsReconcile { get; set; }
        public DateTime ReconcileDate { get; set; }
        public double RequiredCredits { get; set; }

    }
}
