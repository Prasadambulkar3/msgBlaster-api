using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class ClientRepo : GenericRepository<Client>
    {
        internal ClientRepo(UnitOfWork uow)
            : base(uow)
        {
        }
              

        protected override void BeforeInsert(Client entity)
        {
        }

        protected override void BeforeUpdate(Client entity)
        {
        }

        protected override void BeforeDelete(Client entity)
        {
            var id = entity.Id;
            if (_uow.EcouponCampaignRepo.Get(c => c.ClientId == id).Any() || _uow.CampaignRepo.Get(c => c.ClientId == id).Any() || _uow.CreditRequestRepo.Get(a => a.ClientId == id).Any() || _uow.UserRepo.Get(a => a.ClientId == id).Any() || _uow.UserRepo.Get(a => a.ClientId == id).Any() ||
                 _uow.TemplateRepo.Get(a => a.ClientId == id).Any()) //|| _context.SMSGateways.Any(p=>p.Clients.Any(d=>d.Id==id))
                throw new Exception("Cannot delete Client when child entities exist");
        }
    }
}
