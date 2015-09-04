using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class CampaignDTO
    {
     
        public int Id { get; set; }
        public string Name { get; set; }
        public string RecipientsNumber { get; set; }
        public int RecipientsCount { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ScheduledTime { get; set; }
        public string IPAddress { get; set; }
       

        public int MessageCount { get; set; }
        public double RequiredCredits { get; set; }

        //  public bool IsSent { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

        public int ClientId { get; set; }
        public int CreatedBy { get; set; }

        //public string Groups { get; set; }
        //public List<GroupDTO> GroupDTOList { get; set; }

        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsUnicode { get; set; }
        public string LanguageCode { get; set; }
        public int? GroupContactCount { get; set; }
       
        public bool ForAllContact { get; set; }

        public double ConsumedCredits { get; set; }
        public double CreditsDiffrence { get; set; }
        public bool IsReconcile { get; set; }
        public DateTime ReconcileDate { get; set; }

        public TemplateDTO TemplateDTO { get; set; }

       

    }
}
