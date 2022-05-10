namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Impl
{
    using System.Collections;
    using System.Configuration;
    using Common.Extensions;
    using Interface;
    using ViewModels;
    using Core.Web;
    using Data;
    using ExternalAccount.Core.Shared;
    using Config = Fanex.ExternalAccount.Utilities.Config;

    public class CustomerDao : ICustomerDao
    {
        public Customer UserInfo(int custId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("settingstatus", Config.SettingStatus);

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
                                HideCasino = reader.ToBool("HideSabaCasino"),
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