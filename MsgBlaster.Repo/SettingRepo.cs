using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class SettingRepo : GenericRepository<Setting>
    {
        internal SettingRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(Setting entity)
        {

        }

        protected override void BeforeUpdate(Setting entity)
        {

        }

        protected override void BeforeDelete(Setting entity)
        {
            var id = entity.Id;
            if (_context.Settings.Any(e => e.Id == id))
                throw new msgBlasterValidationException("Cannot delete setting");
        }
    }
}
