namespace Fanex.ExternalAccount.DataAccess.Customer.CustomerList
{
    using System;
    using Fanex.Data;
    using Fanex.ExternalAccount.Controllers.ViewModels.CustomerList;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.NAP.Framework.Data;

    public interface ICustomerListDao
    {
        SearchCustomerListViewModel SearchCustomerList(SearchCustomerListBaseCriteria criteria);
    }

    public class CustomerListDao : ICustomerListDao
    {
        private IDynamicRepository _dynamicRepository { get; set; }

        public CustomerListDao() : this(null) { }

        public CustomerListDao(IDynamicRepository repository)
        {
            _dynamicRepository = repository ?? new DynamicRepository();
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public SearchCustomerListViewModel SearchCustomerList(SearchCustomerListBaseCriteria criteria)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var result = new SearchCustomerListViewModel();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    const int decimalPrecision = 4;
                    result.CustomerList.Add(new CustomerListDTO
                    {
                        CustId = Utils.ToInt32(reader["CustId"]),
                        UserName = Utils.ToString(reader["UserName"]),
                        Closed = Utils.ToBoolean(reader["Closed"]),
                        ClosedReason = Utils.ToByte(reader["ClosedReason"]),
                        LockedAccess = new CustomerLockedAccess(Utils.ToByte(reader["Locked"])),
                        CreateDate = Utils.ToStringExt(Utils.ToDateTime(reader["CreateDate"])),
                        Discount1x2 = Utils.ToDecimal(reader["Discount1x2"]).Format(decimalPrecision),
                        DiscountCS = Utils.ToDecimal(reader["DiscountCS"]).Format(decimalPrecision),
                        DoubleCommLosing = Utils.ToBoolean(reader["DoubleCommLosing"]),
                        Enabled = Utils.ToBoolean(reader["Enabled"]),
                        FirstName = Utils.ToString(reader["FirstName"]),
                        GroupA = Utils.HasColumn(reader, "GroupA") ? Utils.ToDecimal(reader["GroupA"]).Format(decimalPrecision) : 0m,
                        GroupB = Utils.HasColumn(reader, "GroupB") ? Utils.ToDecimal(reader["GroupB"]).Format(decimalPrecision) : 0m,
                        GroupC = Utils.HasColumn(reader, "GroupC") ? Utils.ToDecimal(reader["GroupC"]).Format(decimalPrecision) : 0m,
                        GroupD = Utils.HasColumn(reader, "GroupD") ? Utils.ToDecimal(reader["GroupD"]).Format(decimalPrecision) : 0m,
                        LastLoginIP = Utils.ToString(reader["LastLoginIP"]),
                        LastName = Utils.ToString(reader["LastName"]),
                        LimitDays = Utils.HasColumn(reader, "LimitDays") ? Utils.ToInt32(reader["LimitDays"]) : 0,
                        NoOfDownlines = Utils.HasColumn(reader, "Downlines") ? Utils.ToInt32(reader["Downlines"]) : 0,
                        MinDate = Utils.ToStringExt(Utils.HasColumn(reader, "MinDate") ? Utils.ToDateTime(reader["MinDate"]) : DateTime.MinValue),
                        NickName = Utils.HasColumn(reader, "NickName") ? Utils.ToString(reader["NickName"]) : null,
                        RoleId = Utils.ToInt32(reader["RoleId"]),
                        Suspended = Utils.ToBoolean(reader["Suspended"]),
                        UplineDoubleCommLosing = Utils.ToBoolean(reader["UplineDoubleCommLosing"]),
                        UserGroup = Utils.HasColumn(reader, "ugroup") ? Utils.ToString(reader["ugroup"]) : null,
                        Discount = Utils.HasColumn(reader, "Discount") ? Utils.ToDecimal(reader["Discount"]).Format(decimalPrecision) : 0,
                        PlayerDiscount = Utils.HasColumn(reader, "PlayerDiscount") ? Utils.ToDecimal(reader["PlayerDiscount"]).Format(decimalPrecision) : 0m,
                        PlayerDiscount1x2 = Utils.HasColumn(reader, "PlayerDiscount1x2") ? Utils.ToDecimal(reader["PlayerDiscount1x2"]).Format(decimalPrecision) : 0m,
                        PlayerDiscountCS = Utils.HasColumn(reader, "PlayerDiscountCS") ? Utils.ToDecimal(reader["PlayerDiscountCS"]).Format(decimalPrecision) : 0m,
                        SubGroup = Utils.HasColumn(reader, "subgroup") ? Utils.ToString(reader["subgroup"]) : null,
                        Currency = Utils.ToString(reader["currency"])
                    });
                }

                reader.Close();
                result.PageIndex = Utils.ToInt32(objDb.GetOuputParams()["pageindex"]);
                result.TotalRows = Utils.ToInt32(objDb.GetOuputParams()["totalrows"]);
            }

            return result;
        }
    }
}
