using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class CreditRequestRepo : GenericRepository<CreditRequest>
    {
        internal CreditRequestRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(CreditRequest entity)
        {
           
        }

        protected override void BeforeUpdate(CreditRequest entity)
        {
            //var id = entity.Id;
            //var creditRequest = GetById(id, true);
            //if (creditRequest.IsProvided)
            //    throw new Exception("Credit is allready provided. Cannot update it.");
           
        }

        protected override void BeforeDelete(CreditRequest entity)
        {
            var id = entity.Id;
            var creditRequest = GetById(id, true);
            if(creditRequest.IsProvided)
                throw new Exception("Credit is allready provided. Cannot delete it.");
        }
    }
}
