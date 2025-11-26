namespace Fanex.ExternalAccount.Statement.Models.StatementReport
{
    using System;

    //[Serializable]
    public class StatementModel 
    {
        public StatementModel() { }
        public StatementModel(StatementModelDTO statement)
        {
            StatementDate = statement.StatementDate;
            OpeningBalance = new DataColumnModel { HasData = statement.OpeningBalance.HasValue, Value = statement.OpeningBalance };
            Turnover = new DataColumnModel { HasData = statement.Turnover.HasValue, Value = statement.Turnover };
            VoidCancelled = new DataColumnModel { HasData = statement.VoidCancelled.HasValue , Value = statement.VoidCancelled };
            Outstanding = new DataColumnModel { HasData = statement.Outstanding.HasValue, Value = statement.Outstanding };
            WinLoss = new DataColumnModel { HasData = statement.WinLoss.HasValue, Value = statement.WinLoss };
            Commission = new DataColumnModel { HasData = statement.Commission.HasValue, Value = statement.Commission };
            GrandPrize = new DataColumnModel { HasData = statement.GrandPrize.HasValue, Value = statement.GrandPrize };
            TransferAmount = new DataColumnModel { HasData = statement.TransferAmount.HasValue, Value = statement.TransferAmount };
            AccumulatedTotal = new DataColumnModel { HasData = statement.AccumulatedTotal.HasValue, Value = statement.AccumulatedTotal };
            Description = statement.Description;
            TransactionType = statement.TransactionType;
            ExchangeRate = statement.ExchangeRate;
            Creator = statement.Creator;
        }
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

    public class StatementModelDTO
    {
        public DateTime StatementDate { get; set; }
        
        public double? OpeningBalance { get; set; }

        public double? Turnover { get; set; }

        public double? VoidCancelled { get; set; }

        public double? Outstanding { get; set; }

        public double? WinLoss { get; set; }

        public double? Commission { get; set; }

        public double? GrandPrize { get; set; }

        public double? TransferAmount { get; set; }

        public double? AccumulatedTotal { get; set; }
        
        public string Description { get; set; }

        public string TransactionType { get; set; }

        public string ExchangeRate { get; set; }

        public string Creator { get; set; }
    }
}
