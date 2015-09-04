using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;


namespace MsgBlaster.Repo
{
    public class CampaignLogXMLRepo : GenericRepository<CampaignLogXML>
    {
        internal CampaignLogXMLRepo(UnitOfWork uow)
            : base(uow)
        {
        }



        protected override void BeforeInsert(CampaignLogXML entity)
        {
        }

        protected override void BeforeUpdate(CampaignLogXML entity)
        {
        }

        protected override void BeforeDelete(CampaignLogXML entity)
        {
           
        }
    }
}
