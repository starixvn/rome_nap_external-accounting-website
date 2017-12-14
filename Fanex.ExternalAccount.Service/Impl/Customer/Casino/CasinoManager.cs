namespace Fanex.ExternalAccount.Service.Impl.Customer.Casino
{
    using CasinoLibrary.SBAgent;
    using DataAccess.Impl.Customer.Casino;
    using DTO.Customer.Casino;

    public class CasinoManager
    {
        private static readonly CasinoManager instance = new CasinoManager();
        private readonly CasinoDao _casinoDao = new CasinoDao();

        public static CasinoManager Instance => instance;

        public bool UpdateCSStatus(
            int custid,
            bool syncCS,
            bool disabledCS,
            int userId,
            string ip,
            int subAccId,
            string lang)
            => _casinoDao.UpdateCSStatus(custid, syncCS, disabledCS, userId, ip, subAccId, lang);

        public bool UpdateCSStatus(
            int custid,
            bool syncCS,
            bool disabledCS,
            bool isUpdDownline,
            int userId,
            string ip,
            int subAccId,
            string lang,
            string username,
            string subUsername)
        {
            var errMsg = string.Empty;
            return UpdateCSStatus(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId, lang, username,
                subUsername, out errMsg);
        }

        public bool UpdateCSStatus(
            int custid,
            bool syncCS,
            bool disabledCS,
            bool isUpdDownline,
            int userId,
            string ip,
            int subAccId,
            string lang,
            string username,
            string subUsername,
            out string errMsg)
        {
            var status = DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId,
                lang, out errMsg);

            var cust = new Customer();
            return status ? cust.UpdateCustEnableCasino(custid, !disabledCS, isUpdDownline, ip, userId,
                subAccId == 0 ? username : subUsername, 3) : status;
        }

        public CasinoCustomer GetCustInfoIBC(int custid, int uplineId) => _casinoDao.GetCustInfoIBC(custid, uplineId);

        private bool DoUpdateCSStatusWithoutSync(
            int custid,
            bool syncCS,
            bool disabledCS,
            bool isUpdDownline,
            int userId,
            string ip,
            int subAccId,
            string lang,
            out string errMsg)
            => _casinoDao.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId,
            lang, out errMsg);
    }
}