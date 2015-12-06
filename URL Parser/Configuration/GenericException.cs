using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace URL_Parser.Configuration
{
    public class GenericException : Exception
    {
        public GenericException()
        {
        }

        public GenericException(string message)
            : base(message)
        {
        }

        public GenericException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}