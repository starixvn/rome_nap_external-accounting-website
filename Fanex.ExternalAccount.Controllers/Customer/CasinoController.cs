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
            var casinoStakes = new List<object>();
            var miniStakes = new List<object>();

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
                    casinoStakes.Add(new { gameTypeName = item.GameTypeName, stakeLevels = CasinoManager.Instance.GetCustStakeLevels(custId, item.GameTypeID) });
                });
            }
            else if (casinoGame != null && casinoGame.Count > 0 && !custInfo.CasinoSync)
            {
                casinoGame.ForEach(item =>
                {
                    casinoStakes.Add(new { gameTypeName = item.GameTypeName, stakeLevels = CasinoManager.Instance.GetCustStakeLevels(uplineId, item.GameTypeID) });
                });
            }

            if (miniGame != null && miniGame.Count > 0 && custInfo.CasinoSync)
            {
                miniGame.ForEach(item =>
                {
                    miniStakes.Add(new { gameTypeName = item.GameTypeName, stakeLevels = CasinoManager.Instance.GetCustStakeLevels(custId, item.GameTypeID) });
                });
            }
            else if (miniGame != null && miniGame.Count > 0 && !custInfo.CasinoSync)
            {
                miniGame.ForEach(item =>
                {
                    miniStakes.Add(new { gameTypeName = item.GameTypeName, stakeLevels = CasinoManager.Instance.GetCustStakeLevels(uplineId, item.GameTypeID) });
                });
            }

            return this.JsonBase(
                new
                {
                    customerInfo = custInfo,
                    casinoInfo = casinoSetting,
                    casinoGame = casinoStakes,
                    miniGame = miniStakes,
                    pts = positionTaking
                },
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}