using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class CampaignRepo : GenericRepository<Campaign>
    {
        internal CampaignRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(Campaign entity)
        {
        }


        protected override void BeforeUpdate(Campaign entity)
        {
        }

        protected override void BeforeDelete(Campaign entity)
        {
            var id = entity.Id;
            if (_uow.CampaignLogXMLRepo.Get(c => c.CampaignId == id).Any())  //_uow.CampaignLogRepo.Get(c => c.CampaignId == id).Any() ||
                throw new Exception("Cannot delete Campaign when child entities exist");
        }
    }
}
