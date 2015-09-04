using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class ReceiptRepo : GenericRepository<Receipt>
    {
        internal ReceiptRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        public override Receipt GetById(int id)
        {
            return base.GetById(id, true);
        }

        protected override void BeforeInsert(Receipt entity)
        {

        }

        protected override void BeforeUpdate(Receipt entity)
        {

        }

        protected override void BeforeDelete(Receipt entity)
        {

        }
    }
}
