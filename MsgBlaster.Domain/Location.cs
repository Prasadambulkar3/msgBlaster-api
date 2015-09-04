using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class Location : IEntity, ILoggedEntity
    {
        public Location()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
       
        public int ClientId { get; set; }
        public Client Client { get; set; }

        List<User> Users { get; set; }
    }
}
