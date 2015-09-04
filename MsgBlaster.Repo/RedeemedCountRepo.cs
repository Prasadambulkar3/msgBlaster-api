using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;


namespace MsgBlaster.Repo
{
    public class RedeemedCountRepo : GenericRepository<RedeemedCount>
    {
        internal RedeemedCountRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(RedeemedCount entity)
        {

        }
        protected override void BeforeUpdate(RedeemedCount entity)
        {

        }
        protected override void BeforeDelete(RedeemedCount entity)
        {
            //var id = entity.Id;
            //if (_context.Clients.Any(e => e.Plan.Id == id))
            //    throw new msgBlasterValidationException("Cannot delete plan when child entities exist");
        }

    }
}
