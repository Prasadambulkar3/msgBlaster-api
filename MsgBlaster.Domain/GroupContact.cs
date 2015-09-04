using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class GroupContact : IEntity, ILoggedEntity
    {
        public GroupContact()
        { 

        }
        public int Id { get; set; }
        
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
