using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Group : IEntity, ILoggedEntity
    {
        public Group()
        {
            //Contacts = new List<Contact>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public int ClientID { get; set; }
        public Client Client { get; set; }

        //public List<Contact> Contacts { get; set; }
    }
}
