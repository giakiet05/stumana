using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAccess.Exceptions
{
    public class UniqueConstraintException : Exception
    {
        public UniqueConstraintException()
        {
        }

        public UniqueConstraintException(string? message) : base(message)
        {
        }

        public UniqueConstraintException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UniqueConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
