namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public abstract class UpdateSyncBaseForIBCCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public string LoginIP { get; set; }
    }
}
