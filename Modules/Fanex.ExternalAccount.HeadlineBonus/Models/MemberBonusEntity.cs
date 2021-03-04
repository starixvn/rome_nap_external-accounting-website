namespace Fanex.ExternalAccount.HeadlineBonus.Models
{
    using System;

    internal class MemberBonusEntity
    {
        public MemberBonusEntity(
            DateTime createdDate,
            long transId,
            string superName,
            int custId,
            string username,
            decimal amount,
            DateTime? paymentDate,
            byte statusId,
            bool isComplained)
        {
            CreatedDate = createdDate;
            TransId = transId;
            SuperName = superName;
            CustId = custId;
            Username = username;
            Amount = amount;
            PaymentDate = paymentDate;
            StatusId = statusId;
            IsComplained = isComplained;
        }

        public DateTime CreatedDate { get; }

        public long TransId { get; }

        public string SuperName { get; }

        public int CustId { get; }

        public string Username { get; }

        public decimal Amount { get; }

        public DateTime? PaymentDate { get; }

        public byte StatusId { get; }

        public bool IsComplained { get; }
    }
}
