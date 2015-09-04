using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.Repo
{
    public class msgBlasterValidationException : Exception
    {
        public msgBlasterValidationException()
            : base("No logged in user")
        {           
        }

        public msgBlasterValidationException(string message) : base(message)
        {            
        }

        public msgBlasterValidationException(string message, Exception inner)
            : base(message, inner)
        {            
        }
    }
}
