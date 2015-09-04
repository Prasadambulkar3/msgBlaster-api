using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class UserRepo : GenericRepository<User>
    {
        internal UserRepo(UnitOfWork uow)
            : base(uow)
        {
        }


        protected override void BeforeInsert(User entity)
        {
        }

        protected override void BeforeUpdate(User entity)
        {
        }

        protected override void BeforeDelete(User entity)
        {
            var id = entity.Id;
            if (_uow.EcouponCampaignRepo.Get(c => c.CreatedBy == id).Any() || _uow.CampaignRepo.Get(c => c.CreatedBy == id).Any() || _uow.CreditRequestRepo.Get(a => a.RequestedBy == id).Any() || _uow.CouponRepo.Get(c => c.UserId == id).Any())
                throw new Exception("Cannot delete user when child entities exist");
        }
    }
}
