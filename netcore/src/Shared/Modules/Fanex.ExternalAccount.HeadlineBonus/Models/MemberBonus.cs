namespace Fanex.ExternalAccount.HeadlineBonus.Models
{
    using System;
    using Core.Shared.Extensions;
    using Fanex.ExternalAccount.HeadlineBonus.Constants;
    using Fanex.ExternalAccount.HeadlineBonus.Resources;

    internal class MemberBonus
    {
        public const int PendingBonusCancellableHours = 48;

        public MemberBonus(MemberBonusEntity entity)
        {
            CreatedDate = entity.CreatedDate.ToLongDateTime12h();
            TransId = entity.TransId;
            CustId = entity.CustId;
            Username = entity.Username;
            Amount = entity.Amount;
            PaymentDate = entity.PaymentDate.ToShortDateWithSlash();
            BonusStatus = BonusStatusIdConst.GetBonusStatus(entity.StatusId);
            MemberFeedback = GetMemberFeedback(entity.PaymentDate, entity.IsComplained);
            CanCancelPending = entity.StatusId == BonusStatusIdConst.Pending
                && DateTime.Now.Subtract(entity.CreatedDate).TotalHours <= PendingBonusCancellableHours;
            IsCancelled = entity.StatusId == BonusStatusIdConst.Cancelled;
            IsNotReceived = entity.IsComplained;
        }

        public string CreatedDate { get; }

        public long TransId { get; }

        public int CustId { get; }

        public string Username { get; }

        public decimal Amount { get; }

        public string PaymentDate { get; }

        public string BonusStatus { get; }

        public string MemberFeedback { get; }

        public bool CanCancelPending { get; }

        public bool IsCancelled { get; }

        public bool IsNotReceived { get; }

        private static string GetMemberFeedback(DateTime? paymentDate, bool isComplained)
        {
            if (paymentDate == null)
            {
                return string.Empty;
            }

            return isComplained ? ViewLang.NotReceived : ViewLang.Received;
        }
    }
}
