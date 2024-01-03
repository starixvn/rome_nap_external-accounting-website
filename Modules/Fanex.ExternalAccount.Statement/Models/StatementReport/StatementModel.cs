namespace Fanex.ExternalAccount.Statement.Models.StatementReport
{
    using System;

    [Serializable]
    public class StatementModel
    {
        public DateTime StatementDate { get; set; }

        public string StatementDateString { get; set; }

        public DataColumnModel OpeningBalance { get; set; }

        public DataColumnModel Turnover { get; set; }

        public DataColumnModel VoidCancelled { get; set; }

        public DataColumnModel Outstanding { get; set; }

        public DataColumnModel WinLoss { get; set; }

        public DataColumnModel Commission { get; set; }

        public DataColumnModel GrandPrize { get; set; }

        public DataColumnModel TransferAmount { get; set; }

        public DataColumnModel AccumulatedTotal { get; set; }

        public string Description { get; set; }

        public string TransactionType { get; set; }

        public string ExchangeRate { get; set; }

        public string Creator { get; set; }
    }
}
