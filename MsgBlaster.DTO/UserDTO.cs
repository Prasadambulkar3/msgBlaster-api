using MsgBlaster.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Mobile { get; set; }
        public int ClientId { get; set; }

        public UserAccessDTO UserAccessPrivileges { get; set; }
        public string UserType { get; set; }
      
    }
}
