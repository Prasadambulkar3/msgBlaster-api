using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class LocationRepo : GenericRepository<Location>
    {
        internal LocationRepo(UnitOfWork uow)
            : base(uow)
        {
        }



        protected override void BeforeInsert(Location entity)
        {
        }

        protected override void BeforeUpdate(Location entity)
        {
        }

        protected override void BeforeDelete(Location entity)
        {
            var id = entity.Id;
            if (_uow.UserRepo.Get(c => c.LocationId == id).Any() )
                throw new Exception("Cannot delete Location when child entities exist");
        }

    }
}
