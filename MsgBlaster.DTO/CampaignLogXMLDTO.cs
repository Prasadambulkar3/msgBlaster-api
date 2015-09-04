using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;

namespace MsgBlaster.DTO
{
    public class CampaignLogXMLDTO
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }        
        public string XMLLog { get; set; } 
    }
}
