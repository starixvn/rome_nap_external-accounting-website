using Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino;
using Fanex.ExternalAccount.DTO.Customer.Casino;

namespace Fanex.ExternalAccount.Service.Impl.Customer.Casino
{
    public class CasinoManager
    {
        private static readonly CasinoManager instance = new CasinoManager();
        private readonly CasinoDao casinoDao = new CasinoDao();

        public static CasinoManager Instance => instance ?? new CasinoManager();


        public bool UpdateCSStatus(
            int custid,
            bool syncCS,
            bool disabledCS,
            int userId,
            string ip,
            int subAccId,
            string lang)
        {
            return casinoDao.UpdateCSStatus(custid, syncCS, disabledCS, userId, ip, subAccId, lang);
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
            string subUsername)
        {
            string errMsg;
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

            if (!status)
            {
                return false;
            }

            var cust = new CasinoLibrary.SBAgent.Customer();
            return cust.UpdateCustEnableCasino(custid, !disabledCS, isUpdDownline, ip, userId,
                subAccId == 0 ? username : subUsername, 3);
        }

        public CasinoCustomer GetCustInfoIBC(int custid, int uplineId)
        {
            return casinoDao.GetCustInfoIBC(custid, uplineId);
        }

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
        {
            return casinoDao.DoUpdateCSStatusWithoutSync(custid, syncCS, disabledCS, isUpdDownline, userId, ip, subAccId,
                lang, out errMsg);
        }
    }
}