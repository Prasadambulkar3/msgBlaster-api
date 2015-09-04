using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class TemplateRepo : GenericRepository<Template>
    {
        internal TemplateRepo(UnitOfWork uow)
            : base(uow)
        {
        }       

        protected override void BeforeInsert(Template entity)
        {
        }

        protected override void BeforeUpdate(Template entity)
        {
        }

        protected override void BeforeDelete(Template entity)
        {          
        }
    }
}
