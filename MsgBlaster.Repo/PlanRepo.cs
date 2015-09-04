using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class PlanRepo : GenericRepository<Plan>
    {
        internal PlanRepo(UnitOfWork uow)
            : base(uow)
        {
        }
        
        protected override void BeforeInsert(Plan entity)
        {

        }
        protected override void BeforeUpdate(Plan entity)
        {

        }
        protected override void BeforeDelete(Plan entity)
        {
            var id = entity.Id;
            if (_context.Clients.Any(e => e.Plan.Id == id))
                throw new msgBlasterValidationException("Cannot delete plan when child entities exist");
        }
    }
}
