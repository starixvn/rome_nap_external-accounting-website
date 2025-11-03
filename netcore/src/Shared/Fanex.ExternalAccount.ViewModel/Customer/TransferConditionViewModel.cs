namespace Fanex.ExternalAccount.ViewModel
{
    using Core.Shared;
    using Fanex.ExternalAccount.DTO.Customer;
    using Utilities;

    public class TransferConditionViewModel
    {
        public TransferConditionViewModel(Transfer transfer, bool isSuperInAutomateTransfer, int roleId)
        {
            Transfer = transfer;
            IsSuperInAutomateTransfer = isSuperInAutomateTransfer;
            if (isSuperInAutomateTransfer)
            {
                TransferConditionWarning = Role.IsSuper(roleId) ?
                    ViewLang.TransferConditionsWarning_Super :
                    ViewLang.TransferConditionsWarning_Master;
            }
        }

        public Transfer Transfer { get; }

        public bool IsSuperInAutomateTransfer { get; }

        public string TransferConditionWarning { get; }
    }
}
