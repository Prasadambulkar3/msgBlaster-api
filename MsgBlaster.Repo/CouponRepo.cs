using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class CouponRepo : GenericRepository<Coupon>
    {
        internal CouponRepo(UnitOfWork uow)
            : base(uow)
        {

        }
          
        protected override void BeforeInsert(Coupon entity)
        {
        }

        protected override void BeforeUpdate(Coupon entity)
        {
        }

        protected override void BeforeDelete(Coupon entity)
        {
        }
    }
}
