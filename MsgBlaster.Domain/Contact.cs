using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsgBlaster.Domain
{
    public class Contact : IEntity, ILoggedEntity
    {
        public Contact()
        {
            //Groups = new List<Group>();
        }

        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        //public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? AnniversaryDate { get; set; }        
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public Gender? Gender { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                if (LastName == null && LastName == "") 
                { 
                    return string.Format("{0}", FirstName); 
                }
                else
                return string.Format("{0}", FirstName +" "+ LastName);
            }
            //set 
            //{
            //    Name = FirstName + " " + LastName;                
            //}
        }
        //public List<Group> Groups { get; set; }
    }
}
