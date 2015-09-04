using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }       
        public int UserId { get; set; }         
        public int ClientId { get; set; }  
        public DateTime CreatedOn { get; set; }
        public string Client { get; set; }
        public string User { get; set; }

    }
}
