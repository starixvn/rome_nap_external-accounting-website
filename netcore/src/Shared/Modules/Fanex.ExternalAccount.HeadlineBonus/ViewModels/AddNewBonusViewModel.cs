namespace Fanex.ExternalAccount.HeadlineBonus.ViewModels
{
    using Fanex.ExternalAccount.Core.Context;

    public class AddNewBonusViewModel
    {
        public string GroupName
            => ExtRequest.Instance.GetGroupName();

        public string Usernames { get; set; }

        public string Amount { get; set; }

        public decimal AmountValue
            => !string.IsNullOrWhiteSpace(Amount) && decimal.TryParse(Amount, out var amountValue)
            ? amountValue
            : 0;

        public bool IsValid()
            => !string.IsNullOrWhiteSpace(Usernames) && AmountValue > 0;
    }
}
