using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAccess.Exceptions
{
    public class ForeignKeyConstraintException : Exception
    {
        public ForeignKeyConstraintException()
        {
        }

        public ForeignKeyConstraintException(string? message) : base(message)
        {
        }

        public ForeignKeyConstraintException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ForeignKeyConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
