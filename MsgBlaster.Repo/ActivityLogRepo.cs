using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;


namespace MsgBlaster.Repo
{
    public class ActivityLogRepo: GenericRepository<ActivityLog>
    {
        internal ActivityLogRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(ActivityLog entity)
        {
        }


        protected override void BeforeUpdate(ActivityLog entity)
        {
        }

        protected override void BeforeDelete(ActivityLog entity)
        {
             
        }
    }
}
