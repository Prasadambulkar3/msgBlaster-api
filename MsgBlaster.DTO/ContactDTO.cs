using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public int ClientId { get; set; }

        public List<GroupDTO> Groups { get; set; }        

        public int TotalCount { get; set; }
        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public bool IsValid { get; set; }
        public bool IsMobileValid { get; set; }



    }
}
