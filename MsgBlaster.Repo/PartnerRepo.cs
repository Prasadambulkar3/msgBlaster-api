using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class PartnerRepo : GenericRepository<Partner>
    {
        internal PartnerRepo(UnitOfWork uow)
            : base(uow)
        {
        }    

        protected override void BeforeInsert(Partner entity)
        {
           
        }
        protected override void BeforeUpdate(Partner entity)
        {
           
        }
        protected override void BeforeDelete(Partner entity)
        {
            var id = entity.Id;
            if (_context.Clients.Any(e => e.Partner.Id == id) || 
                _context.CreditRequests.Any(c => c.PartnerId == id)
                //||
                //_context.Receipts.Any(c => c.CreatedById == id)
                //_context.Bills.Any(c => c.CreatedById == id)
                )
                throw new msgBlasterValidationException("Cannot delete partner when child entities exist");
        }






    }       
}
