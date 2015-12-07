#region

using System;

#endregion

namespace URL_Parser.Configuration
{
    [Serializable]
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