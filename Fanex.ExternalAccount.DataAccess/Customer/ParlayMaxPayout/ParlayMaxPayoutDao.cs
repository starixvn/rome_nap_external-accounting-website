namespace Fanex.ExternalAccount.DataAccess.Customer.ParlayMaxPayout
{
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.ParlayMaxPayout;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.ParlayMaxPayout;
    using Fanex.Tarus.Customer.Criteria.ParlayMaxPayout;
    using System.Collections.Generic;

    public interface IParlayMaxPayoutDao
    {
        List<ParlayMaxPayoutEntityModel> Select(MaxPayoutSelectCriteria criteria);

        Result Check(MaxPayoutCheckCriteria criteria);

        Result Update(MaxPayoutUpdateCriteria criteria);
    }

    public class ParlayMaxPayoutDao : DAOBase, IParlayMaxPayoutDao
    {
        public List<ParlayMaxPayoutEntityModel> Select(MaxPayoutSelectCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                List<ParlayMaxPayoutEntityModel> result = new List<ParlayMaxPayoutEntityModel>();
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    result.Add(new ParlayMaxPayoutEntityModel
                    {
                        SportType = Utils.ToInt32(reader["SportType"]),
                        Bettype = Utils.ToInt32(reader["BetType"]),
                        SingleMatchParlay = Utils.ToDecimal(reader["SingleMatchParlay"]),
                        UplineSingleMatchParlay = Utils.ToDecimal(reader["Upline_SingleMatchParlay"]),
                        IsAllowUnlimitedSingleMatchParlay = Utils.ToBoolean(reader["SingleMatchParlay_IsAllowUnlimit"]),
                        Combo2 = Utils.ToDecimal(reader["Combo2"]),
                        UplineCombo2 = Utils.ToDecimal(reader["Upline_Combo2"]),
                        IsAllowUnlimitedCombo2 = Utils.ToBoolean(reader["Combo2_IsAllowUnlimit"]),
                        Combo35 = Utils.ToDecimal(reader["Combo35"]),
                        UplineCombo35 = Utils.ToDecimal(reader["Upline_Combo35"]),
                        IsAllowUnlimitedCombo35 = Utils.ToBoolean(reader["Combo35_IsAllowUnlimit"]),
                        ComboOver6 = Utils.ToDecimal(reader["ComboOver6"]),
                        UplineComboOver6 = Utils.ToDecimal(reader["Upline_ComboOver6"]),
                        IsAllowUnlimitedComboOver6 = Utils.ToBoolean(reader["ComboOver6_IsAllowUnlimit"])
                    });
                }

                return result;
            }
        }

        public Result Check(MaxPayoutCheckCriteria criteria) => ExecuteNonQuery(criteria);

        public Result Update(MaxPayoutUpdateCriteria criteria) => ExecuteNonQuery(criteria);

        private List<ParlayMaxPayoutEntityModel> FakeData()
        {
            List<ParlayMaxPayoutEntityModel> data = new List<ParlayMaxPayoutEntityModel>();
            data.Add(new ParlayMaxPayoutEntityModel {
                SportType = 1,
                Bettype = 9,
                SingleMatchParlay = 3000,
                Combo2 =40000,
                Combo35=600000,
                ComboOver6=1000000,
                UplineCombo2=553000,
                UplineCombo35=1844000,
                UplineComboOver6=7375000,
                UplineSingleMatchParlay = 369000,
                IsAllowUnlimitedSingleMatchParlay = true,
                IsAllowUnlimitedCombo2 = true,
                IsAllowUnlimitedCombo35 = true,
                IsAllowUnlimitedComboOver6 = true
            });


            data.Add(new ParlayMaxPayoutEntityModel
            {
                SportType = 99,
                Bettype = 9,
                SingleMatchParlay = 800,
                Combo2 = 15000,
                Combo35 = 25000,
                ComboOver6 = 50000,
                UplineCombo2 = 369000,
                UplineCombo35 = 553000,
                UplineComboOver6 = 1844000,
                UplineSingleMatchParlay = 7375000,
                IsAllowUnlimitedSingleMatchParlay = true,
                IsAllowUnlimitedCombo2 = true,
                IsAllowUnlimitedCombo35 = true,
                IsAllowUnlimitedComboOver6 = true,
            });
            return data;
        }

    }
}
