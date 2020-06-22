namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Common.Extensions;
    using DTO.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public partial class ReportDao
    {
        private const string eASRptWinLossDetailIBCBetLiveCasinoCom = "EAS_Rpt_WinLossExtraDetail_Com";

        public List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(
            string groupName,
            int customerId,
            string productIds,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {ParameterNameConstants.GroupName, groupName},
                {ParameterNameConstants.FromDate, fromDate},
                {ParameterNameConstants.ToDate, toDate},
                {ParameterNameConstants.BalanceUpDay, balanceUpDay},
                {ParameterNameConstants.ProductIds, productIds}
            };
            if (customerId != 0)
            {
                parameters.Add(ParameterNameConstants.CustomerId, customerId);
            }
            var winLossDetailIBCBetLiveCasino =
                GetCommonWinLossDetailIBCBetLiveCasino(eASRptWinLossDetailIBCBetLiveCasinoCom, parameters);

            return winLossDetailIBCBetLiveCasino;
        }

        private List<WinLossDetailIBCBetLiveCasino> GetCommonWinLossDetailIBCBetLiveCasino(string spName, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailIBCBetLiveCasino>();

            using (var reader = ExecDataReader(spName, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailIBCBetLiveCasinoEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailIBCBetLiveCasino CreateWinLossDetailIBCBetLiveCasinoEntity(IDataReader reader) => new WinLossDetailIBCBetLiveCasino
        {
            Username = reader.To<string>(ColumnNameConstants.UserName),
            BetCount = reader.ToInt(ColumnNameConstants.BetCount),
            Turnover = reader.ToDouble(ColumnNameConstants.Turnover),
            NetTurnover = reader.ToDouble(ColumnNameConstants.NetTurnover),
            GrossComm = reader.ToDouble(ColumnNameConstants.GrossComm),
            MemberWinlost = reader.ToDouble(ColumnNameConstants.MemberWinlost),
            PlayerComm = reader.ToDouble(ColumnNameConstants.PlayerComm),
            MemberTotal = reader.ToDouble(ColumnNameConstants.MemberTotal),
            SAgWinlost = reader.ToDouble(ColumnNameConstants.SAgWinlost),
            SAgComm = reader.ToDouble(ColumnNameConstants.SAgComm),
            SAgTotal = reader.ToDouble(ColumnNameConstants.SAgTotal),
            ExtWinlost = reader.ToDouble(ColumnNameConstants.ExtWinlost),
            ExtComm = reader.ToDouble(ColumnNameConstants.ExtComm),
            ExtTotal = reader.ToDouble(ColumnNameConstants.ExtTotal),
            NetCompany = reader.ToDouble(ColumnNameConstants.NetCompany),
        };
    }

    internal static class ParameterNameConstants
    {
        public const string CustomerId = "custid";
        public const string GroupName = "groupname";
        public const string FromDate = "fromdate";
        public const string ToDate = "todate";
        public const string BalanceUpDay = "movebetday";
        public const string ProductIds = "productids";
    }

    internal static class ColumnNameConstants
    {
        public const string UserName = "Username";
        public const string BetCount = "BetCount";
        public const string Turnover = "Turnover";
        public const string NetTurnover = "TurnoverNoDraw";
        public const string GrossComm = "GrossComm";
        public const string MemberWinlost = "MemberWinlost";
        public const string PlayerComm = "PlayerComm";
        public const string MemberTotal = "MemberTotal";
        public const string SAgWinlost = "SAgWinlost";
        public const string SAgComm = "SAgComm";
        public const string SAgTotal = "SAgTotal";
        public const string ExtWinlost = "ExtWinloss";
        public const string ExtComm = "ExtComm";
        public const string ExtTotal = "ExtTotal";
        public const string NetCompany = "NetCompany";
    }
}