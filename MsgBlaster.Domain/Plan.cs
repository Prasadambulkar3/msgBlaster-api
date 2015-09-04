using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
    public class Plan : IEntity, ILoggedEntity
    {
        public Plan()
        {
            Clients = new List<Client>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool IsEcoupon { get; set; }
        public double? Tax { get; set; }

        public List<Client> Clients { get; set; }
    }
}
