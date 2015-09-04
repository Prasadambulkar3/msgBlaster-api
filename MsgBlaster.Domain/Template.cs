using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Template : IEntity, ILoggedEntity
    {
        public Template()
        {

        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
