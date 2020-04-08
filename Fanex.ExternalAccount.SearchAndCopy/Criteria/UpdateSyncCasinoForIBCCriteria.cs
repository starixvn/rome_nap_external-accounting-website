namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    public class UpdateSyncCasinoForIBCCriteria : UpdateSyncBaseForIBCCriteria
    {
        public UpdateSyncCasinoForIBCCriteria(bool disabledCS)
        {
            DisabledCS = disabledCS;
        }

        public bool SyncCS { get; } = true;

        public bool DisabledCS { get; }

        public override string GetSettingKey() => "Acc_UpdCSStatus";

        public override bool IsValid() => true;
    }
}
