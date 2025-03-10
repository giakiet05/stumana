using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAccess.Exceptions
{
    public class CheckConstraintException : Exception
    {
        public CheckConstraintException()
        {
        }

        public CheckConstraintException(string? message) : base(message)
        {
        }

        public CheckConstraintException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }

        protected CheckConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
