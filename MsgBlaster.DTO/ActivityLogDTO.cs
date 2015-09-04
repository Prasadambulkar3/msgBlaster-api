using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class ActivityLogDTO
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public int? PartnerId { get; set; }
        public string OperationType { get; set; }
        public int? ClientId { get; set; }

        public string User { get; set; }
        public string Partner { get; set; }
        public string Client { get; set; }
        public string EntityName { get; set; }
    }
}
