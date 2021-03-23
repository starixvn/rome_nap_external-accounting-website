namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Common.Extensions;
    using Core.Shared;
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using Data;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.ExternalAccount.Utilities;
    using Interface.Customer;
    using ViewModel.Customer.Suspend;

    public class CustomerDao : DAOBase, ICustomerDao
    {
        public Customer GetCustomerByUserName(string userName, string groupName, int userId)
        {
            var param = new Hashtable
            {
                { "username", userName },
                { "groupname", groupName },
                { "userid", userId }
            };

            using (var reader = ExecDataReader("EAS_CustomerSelAllInfo", param))
            {
                if (reader?.Read() == true)
                {
                    return new Customer
                    {
                        CustId = reader.ToInt("custid")
                    };
                }

                return null;
            }
        }

        public bool IsValidUsername(string userName, int uplineId)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

#pragma warning disable S1192 // String literals should not be duplicated
            var parameters = new Hashtable { { "username", userName }, { "isexists", false } };
#pragma warning restore S1192 // String literals should not be duplicated

            var result = DbHelper.ExecuteDbResult("Age_CustomerCheckExists", parameters);

            return !(bool)result.Outputs["isexists"];
        }

        public Customer UserInfo(int custId, byte settingStatus)
        {
#pragma warning disable S1192 // String literals should not be duplicated
            var param = new Hashtable { { "custid", custId }, { "settingstatus", settingStatus} };
#pragma warning restore S1192 // String literals should not be duplicated

            using (var reader = ExecDataReader("Age_CustomerSelUserInfo", param))
            {
                if (reader?.Read() == true)
                {
                    return new Customer
                    {
                        CustId = reader.ToInt("custid"),
#pragma warning disable S1192 // String literals should not be duplicated
                        RoleId = reader.ToInt("roleid"),
#pragma warning restore S1192 // String literals should not be duplicated
                        Username = Utils.ToString(reader["username"]),
                        FirstName = Utils.ToString(reader["firstname"]),
                        LastName = Utils.ToString(reader["lastname"]),
                        MobilePhone = Utils.ToString(reader["mobilephone"]),
                        Phone = Utils.ToString(reader["phone"]),
                        Email = Utils.ToString(reader["email"]),
                        Fax = Utils.ToString(reader["fax"]),

                        Remark = Utils.ToString(reader["remark"])
                    };
                }

                return null;
            }
        }

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
        {
            var param = new Hashtable
            {
                {"custid", customer.CustId},
                {"firstname", customer.FirstName??string.Empty},
                {"lastname", customer.LastName??string.Empty},
                {"phone", customer.Phone??string.Empty},
                {"mobilephone", customer.MobilePhone??string.Empty},
                {"email", customer.Email??string.Empty},
                {"fax", customer.Fax??string.Empty},
                {"discasino", 0},
                {"ip", ip},
                {"userid", userId},
                {"subaccid", subAccId},
                {"language", language},
                {"remark", customer.Remark??string.Empty},
                {"errorcode", 0},
                {"errormsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

        public Result ResetPassword(ResetPasswordCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

#pragma warning disable S1192 // String literals should not be duplicated
            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
#pragma warning restore S1192 // String literals should not be duplicated
                result.ResultSet);
        }

        public Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
                result.ResultSet);
        }

        public Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria)
        {
            var result = new Result();
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                result.ErrorCode = outputs["errcode"].ToString();
                result.ErrorMessage = outputs["errmsg"].ToString();
            }

            return result;
        }

        public List<SingleUpdateAffectedDownline> GetSingleUpdateSuspendAffectedDownlines(GetSingleSuspendCustomerInfoCriteria criteria)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var listAffectedDownline = new List<SingleUpdateAffectedDownline>();
                objDb.AssignParamValues(criteria.GetSpParams());
                var reader = objDb.ExecuteReader();
                while (reader.Read())
                {
                    listAffectedDownline.Add(new SingleUpdateAffectedDownline(
                        Utilities.Utils.ToInt32(reader["roleid"]),
#pragma warning disable S1192 // String literals should not be duplicated
                        Utilities.Utils.ToInt32(reader["cntAffected"]),
                        Utilities.Utils.ToInt32(reader["cntUnchanged"])));
#pragma warning restore S1192 // String literals should not be duplicated
                }

                return listAffectedDownline;
            }
        }

        public Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }
        }

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
        {
            var listDownlines = new List<UnsuspendDownlineDetail>();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    listDownlines.Add(new UnsuspendDownlineDetail(Utilities.Utils.ToInt32(reader["roleid"]), Utilities.Utils.ToString(reader["username"])));
                }

                reader.Close();
                var outputs = objDb.GetOuputParams();
                return new SuspendedCustomerDetailViewModel(Utilities.Utils.ToInt32(outputs["totalrows"]), listDownlines);
            }
        }

        public List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
        {
            var result = new List<CustomerSuspendStatusRow>();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    result.Add(new CustomerSuspendStatusRow
                    {
                        CustId = Utilities.Utils.ToInt32(reader["custid"]),
                        RoleId = criteria.RoleId,
                        UserName = Utilities.Utils.ToString(reader["username"]),
                        Suspend = Utilities.Utils.ToBoolean(reader["suspended"])
                    });
                }

                reader.NextResult();
                while (reader.Read())
                {
                    var custid = Utilities.Utils.ToInt32(reader["custid"]);
                    var row = result.FirstOrDefault(m => m.CustId == custid);
                    if (row != null)
                    {
                        var roleid = Utilities.Utils.ToInt32(reader["roleid"]);
                        switch (roleid)
                        {
                            case Role.Master:
                                row.AffectedMaster = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedMaster = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;
                            case Role.Agent:
                                row.AffectedAgent = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedAgent = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;
                            default:
                                row.AffectedMember = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedMember = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public void KickoutCustomer(KickoutCustomerCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                db.ExecuteNonQuery();
            }
        }

#pragma warning disable S1192 // String literals should not be duplicated
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
#pragma warning disable S1541 // Methods and properties should not be too complex
        public CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria)
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = new CustomerStatusDTO();
                CustomerStatusDCS sportsbook2 = null;
                CustomerStatusDCS liveCasino = null;
                CustomerStatusDCS rngLottery = null;
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

                    if (productId == ProductConstants.Sportsbook2)
                    {
                        if (sportsbook2 == null)
                        {
                            sportsbook2 = new CustomerStatusDCS
                            {
                                ProductID = productId,
                                ProductName = ProductSettings.SiteProducts[productId]?.Name,
                                IsDisabled = Utils.ToBoolean(reader["isDisabled"]),
                                IsDisabledUpline = Utils.ToBoolean(reader["isDisabledUpline"]),
                                IsHidden = !ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]),
                                IsSynced = Utils.ToBoolean(reader["isSync"]),
                                IsSyncedUpline = Utils.ToBoolean(reader["isSyncedUpline"])
                            };
                        }
                        else
                        {
                            sportsbook2.IsDisabled = sportsbook2.IsDisabled && Utils.ToBoolean(reader["isDisabled"]);
                            sportsbook2.IsSynced = sportsbook2.IsSynced && Utils.ToBoolean(reader["isSync"]);
                        }

                        continue;
                    }

                    if (productId == ProductConstants.LiveCasino)
                    {
                        if (liveCasino == null)
                        {
                            liveCasino = new CustomerStatusDCS
                            {
                                ProductID = productId,
                                ProductName = ProductSettings.SiteProducts[productId]?.Name,
                                IsDisabled = Utils.ToBoolean(reader["isDisabled"]),
                                IsDisabledUpline = Utils.ToBoolean(reader["isDisabledUpline"]),
                                IsHidden = !ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]),
                                IsSynced = Utils.ToBoolean(reader["isSync"]),
                                IsSyncedUpline = Utils.ToBoolean(reader["isSyncedUpline"])
                            };
                        }
                        else
                        {
                            if(!Utils.ToBoolean(reader["isHidden"]))
                            {
                                liveCasino.IsDisabled = liveCasino.IsDisabled || Utils.ToBoolean(reader["isDisabled"]);
                                liveCasino.IsDisabledUpline = liveCasino.IsDisabledUpline || Utils.ToBoolean(reader["isDisabledUpline"]);
                            }
                            
                            liveCasino.IsHidden = liveCasino.IsHidden && (!ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]));
                        }

                        continue;
                    }

                    if (productId == ProductConstants.RNGLottery)
                    {
                        if (rngLottery == null)
                        {
                            rngLottery = new CustomerStatusDCS
                            {
                                ProductID = productId,
                                ProductName = ProductSettings.SiteProducts[productId]?.Name,
                                IsDisabled = Utils.ToBoolean(reader["isDisabled"]),
                                IsDisabledUpline = Utils.ToBoolean(reader["isDisabledUpline"]),
                                IsHidden = !ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]),
                                IsSynced = Utils.ToBoolean(reader["isSync"]),
                                IsSyncedUpline = Utils.ToBoolean(reader["isSyncedUpline"])
                            };
                        }
                        else
                        {
                            if (!Utils.ToBoolean(reader["isHidden"]))
                            {
                                rngLottery.IsDisabled = rngLottery.IsDisabled || Utils.ToBoolean(reader["isDisabled"]);
                                rngLottery.IsDisabledUpline = rngLottery.IsDisabledUpline || Utils.ToBoolean(reader["isDisabledUpline"]);
                            }

                            rngLottery.IsHidden = rngLottery.IsHidden && (!ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]));
                        }

                        continue;
                    }

                    if (!result.IsDCSProductExisted(productId))
                    {
                        result.DCSProductsSetting.Add(new CustomerStatusDCS
                        {
                            ProductID = productId,
                            ProductName = ProductSettings.SiteProducts.ContainsKey(productId) ? ProductSettings.SiteProducts[productId].Name : string.Empty,
                            IsDisabled = Utils.ToBoolean(reader["isDisabled"]),
                            IsDisabledUpline = Utils.ToBoolean(reader["isDisabledUpline"]),
                            IsHidden = !ProductSettings.IsShowProduct(productId) || Utils.ToBoolean(reader["isHidden"]),
                            IsSynced = Utils.ToBoolean(reader["isSync"]),
                            IsSyncedUpline = Utils.ToBoolean(reader["isSyncedUpline"])
                        });
                    }
                }

                result.DCSProductsSetting.Add(sportsbook2);
                if (liveCasino != null)
                {
                    result.DCSProductsSetting.Add(liveCasino);
                }

                if (rngLottery != null)
                {
                    result.DCSProductsSetting.Add(rngLottery);
                }

                return result;
            }
        }
#pragma warning restore S1192 // String literals should not be duplicated

        public bool CheckLimitNewDownline(CheckLimitNewDownlineCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = db.GetOuputParams();

                return Utils.ToBoolean(outputs["isallowed"]);
            }
        }
    }
}