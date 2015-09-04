using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MsgBlaster.Domain;

namespace MsgBlaster.Repo
{
    public class DocumentRepo : GenericRepository<Document>
    {
        internal DocumentRepo(UnitOfWork uow)
            : base(uow)
        {
        }
        

        protected override void BeforeInsert(Document entity)
        {
        }

        protected override void BeforeUpdate(Document entity)
        {
        }

        protected override void BeforeDelete(Document entity)
        {
        }
    }
}
