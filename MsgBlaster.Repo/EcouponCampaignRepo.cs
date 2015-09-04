using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class EcouponCampaignRepo : GenericRepository<EcouponCampaign>
    {
        internal EcouponCampaignRepo(UnitOfWork uow)
            : base(uow)
        {
        }
               
        protected override void BeforeInsert(EcouponCampaign entity)
        {
        }

        protected override void BeforeUpdate(EcouponCampaign entity)
        {
        }

        protected override void BeforeDelete(EcouponCampaign entity)
        {
        }
    }
}
