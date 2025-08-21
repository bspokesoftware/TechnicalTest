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
    public class TransferException : Exception
    {
        public TransferErrorCode ErrorCode { get; }

        public TransferException(TransferErrorCode errorCode, string message) : base(message) => ErrorCode = errorCode;

        public static TransferException NotFound(string message) => new TransferException(TransferErrorCode.NotFound, message);
        public static TransferException ValidationFailed(string message) => new TransferException(TransferErrorCode.ValidationFailed, message);
        public static TransferException BusinessRuleViolation(string message) => new TransferException(TransferErrorCode.BusinessRuleViolation, message);

    }
}
