namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Impl
{
    using System.Collections;
    using System.Configuration;
    using Common.Extensions;
    using Interface;
    using ViewModels;
    using Core.Web;
    using Data;

    public class CustomerDao : ICustomerDao
    {
        public Customer UserInfo(int custId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);

            using (var objDb = new ObjectDb("Age_CustomerSelUserInfo"))
            {
                using (var reader = objDb.ExecuteReader(param))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            return new Customer
                            {
                                CustId = reader.ToInt("custid"),
                                Username = reader.ToString("username"),
                                FirstName = reader.ToString("firstname"),
                                LastName = reader.ToString("lastname"),
                                MobilePhone = reader.ToString("mobilephone"),
                                HomePhone = reader.ToString("phone"),
                                Email = reader.ToString("email"),
                                Fax = reader.ToString("fax"),
                                RoleId = reader.ToInt("roleid"),
                                SyncBingo = reader.ToBool("syncBingo"),
                                SyncCasino = reader.ToBool("syncCasino"),
                                SyncLiveCasino = reader.ToBool("syncLiveCasino"),
                                HideGoldDeluxe = reader.ToBool("HideGoldDeluxe") || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0",
                                CurrencyName = reader.ToString("currencyName")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int UserGroupId(int custId) => SessionManager.Get<int>("EASGroupId");
    }
}