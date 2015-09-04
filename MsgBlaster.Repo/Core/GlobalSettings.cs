using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.Repo
{
    public static class GlobalSettings
    {
        public static int? LoggedInClientId { get; set; }
        public static int? LoggedInUserId { get; set; }
        public static int? LoggedInPartnerId { get; set; }  
    }
}
