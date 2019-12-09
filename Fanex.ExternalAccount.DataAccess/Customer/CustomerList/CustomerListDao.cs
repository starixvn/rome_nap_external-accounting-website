namespace Fanex.ExternalAccount.DataAccess.Customer.CustomerList
{
    using Fanex.Data;
    using Fanex.ExternalAccount.Controllers.ViewModels.CustomerList;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList;
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.CustomerList;
    using Fanex.NAP.Framework.Data;
    using System.Linq;

    public interface ICustomerListDao
    {
        SearchCustomerListViewModel SearchCustomerList(SearchCustomerListBaseCriteria criteria, bool IsViewIPVN);

        CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria);
    }

    public class CustomerListDao : ICustomerListDao
    {
        private IDynamicRepository dynamicRepository { get; set; }

        public CustomerListDao() : this(null) { }

        public CustomerListDao(IDynamicRepository repository)
        {
            dynamicRepository = repository ?? new DynamicRepository();
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public SearchCustomerListViewModel SearchCustomerList(SearchCustomerListBaseCriteria criteria, bool IsViewIPVN)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var result = new SearchCustomerListViewModel();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    result.CustomerList.Add(new CustomerListDTO
                    {
                        CustId = Utils.ToInt32(reader["CustId"]),
                        UserName = Utils.ToString(reader["UserName"]),
                        Closed = Utils.ToBoolean(reader["Closed"]),
                        ClosedReason = Utils.ToByte(reader["ClosedReason"]),
                        CreateDate = Utils.ToDateTime(reader["CreateDate"]),
                        Discount1x2 = Utils.ToInt32(reader["Discount1x2"]),
                        DiscountCS = Utils.ToInt32(reader["DiscountCS"]),
                        DoubleCommLosing = Utils.ToBoolean(reader["DoubleCommLosing"]),
                        Enabled = Utils.ToBoolean(reader["Enabled"]),
                        FirstName = Utils.ToString(reader["FirstName"]),
                        GroupA = Utils.HasColumn(reader, "GroupA") ? Utils.ToInt32(reader["GroupA"]) : 0,
                        GroupB = Utils.HasColumn(reader, "GroupB") ? Utils.ToInt32(reader["GroupB"]) : 0,
                        GroupC = Utils.HasColumn(reader, "GroupC") ? Utils.ToInt32(reader["GroupC"]) : 0,
                        GroupD = Utils.HasColumn(reader, "GroupD") ? Utils.ToInt32(reader["GroupD"]) : 0,
                        LastLoginIP = Utils.ToString(reader["LastLoginIP"]),
                        LastName = Utils.ToString(reader["LastName"]),
                        LimitDays = Utils.HasColumn(reader, "LimitDays") ? Utils.ToInt32(reader["LimitDays"]) : 0,
                        MinDate = Utils.HasColumn(reader, "MinDate") ? Utils.ToNullAbleDateTime(reader["MinDate"]) : null,
                        NickName = Utils.HasColumn(reader, "NickName") ? Utils.ToString(reader["NickName"]) : null,
                        RoleId = Utils.ToInt32(reader["RoleId"]),
                        Suspended = Utils.ToBoolean(reader["Suspended"]),
                        UplineDoubleCommLosing = Utils.ToBoolean(reader["UplineDoubleCommLosing"]),
                        UserGroup = Utils.HasColumn(reader, "ugroup") ? Utils.ToString(reader["ugroup"]) : null,
                        Discount = Utils.HasColumn(reader, "Discount") ? Utils.ToDecimal(reader["Discount"]) : 0,
                        PlayerDiscount= Utils.HasColumn(reader, "PlayerDiscount") ? Utils.ToDecimal(reader["PlayerDiscount"]) : 0,
                        PlayerDiscount1x2 = Utils.HasColumn(reader, "PlayerDiscount1x2") ? Utils.ToDecimal(reader["PlayerDiscount1x2"]) : 0,
                        PlayerDiscountCS = Utils.HasColumn(reader, "PlayerDiscountCS") ? Utils.ToDecimal(reader["PlayerDiscountCS"]) : 0
                    });
                }

                reader.Close();
                result.TotalRows = Utils.ToInt32(objDb.GetOuputParams()["totalrows"]);
            }

            return result;
        }

        public CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = new CustomerStatusDTO();
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                if (reader.Read())
                {
                    result.HasSecurityCode = Utils.ToBoolean(reader["HasSecurityCode"]);
                    result.UplineHasSecurityCode = Utils.ToBoolean(reader["UplineHasSecurityCode"]);
                    result.DisableTransfer = Utils.ToBoolean(reader["DisableTransfer"]);
                }

                reader.NextResult();
                if (reader.Read())
                {
                    result.SyncedCasino = Utils.ToBoolean(reader["SyncCasino"]);
                    result.UplineSyncedCasino = Utils.ToBoolean(reader["uplineSyncCasino"]);
                    result.DisableCasino = Utils.ToBoolean(reader["DisableCasino"]);
                    result.HideGoldDeluxe = !ProductSettings.IsShowGoldDeluxe || Utils.ToBoolean(reader["HideGoldDeluxe"]);
                    result.DisableGoldDeluxe = Utils.ToBoolean(reader["DisableGoldDeluxe"]);
                    result.UplineHideGoldDeluxe = Utils.ToBoolean(reader["uplineHideGoldDeluxe"]);
                    result.HideKeno = !ProductSettings.IsShowKeno || Utils.ToBoolean(reader["HideKeno"]);
                    result.DisableKeno = Utils.ToBoolean(reader["DisableKeno"]);
                }

                reader.NextResult();
                while (reader.Read())
                {
                    int productId = Utils.ToInt32(reader["ProductID"]);
                    if (productId == ProductConstants.VirtualSports)
                    {
                        result.HideVirtualSport = Utils.ToBoolean(reader["isHidden"]);
                        result.DisableVirtualSport = Utils.ToBoolean(reader["isDisabled"]);
                        continue;
                    }

                    if (!result.IsDCSProductExisted(productId))
                    {
                        result.DCSProductsSetting.Add(new CustomerStatusDCS
                        {
                            ProductID = productId,
                            ProductName = ProductSettings.SiteProducts[productId]?.Name,
                            IsDisabled = Utils.ToBoolean(reader["isDisabled"]),
                            IsDisabledUpline = Utils.ToBoolean(reader["isDisabledUpline"]),
                            IsHidden = !ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]),
                            IsSynced = Utils.ToBoolean(reader["isSync"]),
                            IsSyncedUpline = Utils.ToBoolean(reader["isSyncedUpline"])
                        });
                    }
                }

                return result;
            }
        }
    }
}
