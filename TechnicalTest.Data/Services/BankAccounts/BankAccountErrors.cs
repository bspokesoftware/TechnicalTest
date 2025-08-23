using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalTest.Data.Services.BankAccounts
{
    public enum BankAccountErrorCode
    {
        NotFound,
        ValidationFailed,
        Conflict
    }
    public  class BankAccountException : Exception
    {
        public BankAccountErrorCode ErrorCode { get; }

        public BankAccountException(BankAccountErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public static BankAccountException NotFound(string message)
            => new (BankAccountErrorCode.NotFound, message);

        public static BankAccountException ValidationFailed(string message)
            => new(BankAccountErrorCode.ValidationFailed, message);
        public static BankAccountException Conflict(string message)
            => new(BankAccountErrorCode.Conflict, message);
    }
}
