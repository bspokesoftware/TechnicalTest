using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalTest.Data.Services
{
    public enum TransferErrorCode
    {
        NotFound,
        ValidationFailed,
        BusinessRuleViolation
    }
    public class TransferErrors : Exception
    {
    }
}
