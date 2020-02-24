using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BC.Integration.APICalls
{


    public class BlueCherryException : Exception
    {
        public BlueCherryException()
        {
            //adding comment
        }

        public BlueCherryException(string message)
            : base(message)
        {
        }

        public BlueCherryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}