using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanManager.Application.Common.Exceptions
{
    internal class UnauthorizedException: Exception
    {
        public UnauthorizedException()
            : base("Unauthorized access.")
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UnauthorizedException(string name, object key)
            : base($"Unauthorized access to entity \"{name}\" ({key}).")
        {
        }
    }
}
