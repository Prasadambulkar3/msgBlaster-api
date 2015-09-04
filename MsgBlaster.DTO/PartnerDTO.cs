using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class PartnerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AlertOnCredit { get; set; }
        public int? AlertCreditBalanceLimit { get; set; }
        //public UserType UserType { get; set; }
        public List<ClientDTO> Clients{ get; set; }
        
    }
}
