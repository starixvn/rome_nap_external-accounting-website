namespace Fanex.ExternalAccount.DTO.Report
{
    using System;
    using System.Collections.Generic;

    public class SuperWinLossDetailByProductDTO
    {
        private const int percentageFormat = 100;

        public SuperWinLossDetailByProductDTO()
        {
        }

        public SuperWinLossDetailByProductDTO(
            int custId,
            string username,
            int productId,
            string productName,
            decimal turnover,
            decimal netTurnover,
            decimal grossComm,
            decimal memberWinlost,
            decimal memberComm,
            decimal memberTotal,
            decimal agWinlost,
            decimal agComm,
            decimal agTotal,
            decimal magWinlost,
            decimal magComm,
            decimal magTotal,
            decimal sagWinlost,
            decimal sagComm,
            decimal sagTotal,
            decimal company)
        {
            CustId = custId;
            Username = username;
            ProductId = productId;
            ProductName = productName;
            Turnover = turnover;
            NetTurnover = netTurnover;
            GrossComm = grossComm;
            MemberWinlost = memberWinlost;
            MemberComm = memberComm;
            MemberTotal = memberTotal;
            AgWinlost = agWinlost;
            AgComm = agComm;
            AgTotal = agTotal;
            MAgWinlost = magWinlost;
            MAgComm = magComm;
            MAgTotal = magTotal;
            SAgWinlost = sagWinlost;
            SAgComm = sagComm;
            SAgTotal = sagTotal;
            Company = company;
        }


        public List<string> ProductNames { get; set; }

        public decimal PositionTaking { get; set; }

        public string PositionTakingText => $"{String.Format("{0:0.#}", PositionTaking * percentageFormat)}% PT";

        public int CustId { get; }

        public string Username { get; }

        public int ProductId { get; }

        public string ProductName { get; }

        public decimal Turnover { get; }

        public decimal NetTurnover { get; }

        public decimal GrossComm { get; }

        public decimal MemberWinlost { get; }

        public decimal MemberComm { get; }

        public decimal MemberTotal { get; }

        public decimal AgWinlost { get; }

        public decimal AgComm { get; }

        public decimal AgTotal { get; }

        public decimal MAgWinlost { get; }

        public decimal MAgComm { get; }

        public decimal MAgTotal { get; }

        public decimal SAgWinlost { get; }

        public decimal SAgComm { get; }

        public decimal SAgTotal { get; }

        public decimal Company { get; }
    }
}
