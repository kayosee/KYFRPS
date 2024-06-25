using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYFRPS.Common
{
    public enum ErrorType : byte
    {
        NoError = 0,
        PasswordInvalid,
        ConnectError,
    }
}
