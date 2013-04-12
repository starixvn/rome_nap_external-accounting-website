// --------------------------------------------------------------------------
// <copyright file="BingoManager.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.12">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.Customer.Bingo
{
    using Fanex.ExternalAccount.DataAccess.Impl.Customer.Bingo;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoManager
    {
        private static BingoManager instance = new BingoManager();
        private readonly BingoDao bingo = new BingoDao();

        public static BingoManager Instance
        {
            get
            {
                return BingoManager.instance ?? new BingoManager();
            }
        }

        public int UpdateBingoStatus(int custId, int userId, int subAccId, bool isSynBingo, bool disabledBingo, bool isEnableDownline, string ip, string languageKey, ref string errMsg)
        {
            return this.bingo.UpdateBingoStatus(custId, userId, subAccId, isSynBingo, disabledBingo, isEnableDownline, ip, languageKey, ref errMsg);
        }
    }
}
