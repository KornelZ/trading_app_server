using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Enums
{
    public enum ErrorValue
    {
        NoError = 0,
        ServerError = 1,
        AmountGreaterThanStock = 2,
        EntityExists = 3,
        TransactionAlreadyFinished = 4
    }
}