using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;


namespace MsgBlaster.Repo 
{
    public class GroupContactRepo : GenericRepository<GroupContact>
    {
        internal GroupContactRepo(UnitOfWork uow)  
            : base(uow)
        {
        }
        protected override void BeforeInsert(GroupContact entity)
        {

        }
        protected override void BeforeUpdate(GroupContact entity)
        {

        }

        protected override void BeforeDelete(GroupContact entity)
        {

        }

    }
}
