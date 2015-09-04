using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class CampaignLogXML : IEntity, ILoggedEntity
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public string XMLLog { get; set; } 
    }
}
