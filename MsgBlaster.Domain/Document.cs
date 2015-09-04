using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class Document : IEntity, ILoggedEntity
    {
        public Document()
        {
 
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public DateTime CreatedOn { get; set; }




    }
}
