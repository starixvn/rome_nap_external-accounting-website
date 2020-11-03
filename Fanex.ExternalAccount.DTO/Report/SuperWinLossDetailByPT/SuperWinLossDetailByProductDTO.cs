namespace Fanex.ExternalAccount.DTO.Report
{
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByProductDTO
    {
        private const int percentageFormat = 100;

        public List<string> ProductNames { get; set; }

        public decimal PositionTaking { get; set; }

        public string PositionTakingText => $"{String.Format("{0:0.#}", PositionTaking * percentageFormat)}% PT";

        public int CustId { get; set; }

        public string Username { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Turnover { get; set; }

        public decimal NetTurnover { get; set; }

        public decimal GrossComm { get; set; }

        public decimal MemberWinlost { get; set; }

        public decimal PlayerComm { get; set; }

        public decimal MemberTotal { get; set; }

        public decimal AgWinlost { get; set; }

        public decimal AgComm { get; set; }

        public decimal AgTotal { get; set; }

        public decimal MAgWinlost { get; set; }

        public decimal MAgComm { get; set; }

        public decimal MAgTotal { get; set; }

        public decimal SAgWinlost { get; set; }

        public decimal SAgComm { get; set; }

        public decimal SAgTotal { get; set; }

        public decimal Company { get; set; }
    }
}
