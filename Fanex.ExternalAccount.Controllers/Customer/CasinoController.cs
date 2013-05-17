// --------------------------------------------------------------------------
// <copyright file="CasinoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.15">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// RNG Casino controller
    /// </summary>
    public class CasinoController : BaseController
    {
        [Authorization(Function = eFunctions.Casino, Policy = ePolicy.Read)]
        public ActionResult EditCasino(int custId, int uplineId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Casino, Policy = ePolicy.Read)]
        public ActionResult EditCasinoPost(int custId, int uplineId)
        {
            var custInfo = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            var casinoSetting = new CasinoCustomer();
            var positionTaking = new List<GameKindDetail>(); 
            var casinoGame = new List<GameTypeDetail>();
            var miniGame = new List<GameTypeDetail>();
            var casinoStakes = new List<Stakes>();
            var miniStakes = new List<Stakes>();

            if (!custInfo.CasinoSync)
            {
                casinoSetting = CasinoManager.Instance.GetCustSettingCS(uplineId);
                positionTaking = CasinoManager.Instance.GetGameKinds(uplineId);
            }
            else
            {
                casinoSetting = CasinoManager.Instance.GetCustSettingCS(custId);
                positionTaking = CasinoManager.Instance.GetGameKinds(custId);
            }

            if (positionTaking != null && positionTaking.Count == 2 && custInfo.CasinoSync)
            {
                casinoGame = CasinoManager.Instance.GetGameType(custId, positionTaking[0].CodeID);
                miniGame = CasinoManager.Instance.GetGameType(custId, positionTaking[1].CodeID);
            }
            else if (positionTaking != null && positionTaking.Count == 2 && !custInfo.CasinoSync)
            {
                casinoGame = CasinoManager.Instance.GetGameType(uplineId, positionTaking[0].CodeID);
                miniGame = CasinoManager.Instance.GetGameType(uplineId, positionTaking[1].CodeID);
            }

            if (casinoGame != null && casinoGame.Count > 0 && custInfo.CasinoSync)
            {
                casinoGame.ForEach(item =>
                {
                    casinoStakes.Add(
                        new Stakes 
                        { 
                            GameTypeName = item.GameTypeName, 
                            StakeLevels = CasinoManager.Instance.GetCustStakeLevels(custId, item.GameTypeID) 
                        });
                });
            }
            else if (casinoGame != null && casinoGame.Count > 0 && !custInfo.CasinoSync)
            {
                casinoGame.ForEach(item =>
                {
                    casinoStakes.Add(
                        new Stakes { 
                            GameTypeName = item.GameTypeName, 
                            StakeLevels = CasinoManager.Instance.GetCustStakeLevels(uplineId, item.GameTypeID) 
                        });
                });
            }

            if (miniGame != null && miniGame.Count > 0 && custInfo.CasinoSync)
            {
                miniGame.ForEach(item =>
                {
                    miniStakes.Add(
                        new Stakes { 
                            GameTypeName = item.GameTypeName, 
                            StakeLevels = CasinoManager.Instance.GetCustStakeLevels(custId, item.GameTypeID) 
                        });
                });
            }
            else if (miniGame != null && miniGame.Count > 0 && !custInfo.CasinoSync)
            {
                miniGame.ForEach(item =>
                {
                    miniStakes.Add(
                        new Stakes { GameTypeName = item.GameTypeName, 
                            StakeLevels = CasinoManager.Instance.GetCustStakeLevels(uplineId, item.GameTypeID) 
                        });
                });
            }

            return this.JsonBase(
                new CasinoObject
                {
                    CustomerInfo = custInfo,
                    CasinoInfo = casinoSetting,
                    CasinoGame = casinoStakes,
                    MiniGame = miniStakes,
                    Pts = positionTaking
                },
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        [Authorization(Function = eFunctions.Casino, Policy = ePolicy.Update)]
        public ActionResult UpdateCasinoPost(int custId, int roleId, int uplineId, int uplineRoleId, string customer)
        {
            var ret = CasinoManager.Instance.Update(custId, roleId, uplineId, uplineRoleId, CurrentContext.Ip4Address, CurrentSession.UserName, FromSystem, JsonConvert.DeserializeObject<CasinoObject>(customer));

            return this.JsonBase(new { ErrorCode = ret.IsSuccess ? 0 : 1, ErrorMessage = ret.ErrorMessage });
        }
    }
}