﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO
{
    public class GroupContactDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }  
        public int ContactId { get; set; }
        public int ContactCount { get; set; }

        public List<ContactDTO> Contacts { get; set; }
        public List<ContactDTO> UnwantedContacts { get; set; }  
    }
}
