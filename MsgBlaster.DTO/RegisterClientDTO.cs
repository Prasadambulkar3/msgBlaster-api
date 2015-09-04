using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MsgBlaster.DTO
{
    public class RegisterClientDTO
    {
        //For Client
        public int ClientId { get; set; }        
        public string Address { get; set; }        
        public string Company { get; set; }

        //For Location
        public string Location { get; set; }

        //For User
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Name { get; set; }
        public string Email { get; set; }  
        public string Password { get; set; }      
        public string Mobile { get; set; }
        public string UserType { get; set; }
        public UserAccessDTO UserAccessPrivileges { get; set; }
        
    }
}
