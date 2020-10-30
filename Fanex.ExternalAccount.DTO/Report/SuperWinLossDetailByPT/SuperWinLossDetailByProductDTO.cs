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

        public double Turnover { get; set; }

        public double NetTurnover { get; set; }

        public double GrossComm { get; set; }

        public double MemberWinlost { get; set; }

        public double PlayerComm { get; set; }

        public double MemberTotal { get; set; }

        public double AgWinlost { get; set; }

        public double AgComm { get; set; }

        public double AgTotal { get; set; }

        public double MAgWinlost { get; set; }

        public double MAgComm { get; set; }

        public double MAgTotal { get; set; }

        public double SAgWinlost { get; set; }

        public double SAgComm { get; set; }

        public double SAgTotal { get; set; }

        public double Company { get; set; }
    }
}
