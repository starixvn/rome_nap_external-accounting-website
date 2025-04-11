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
            decimal? turnover,
            decimal? netTurnover,
            decimal? grossComm,
            decimal? memberWinlost,
            decimal? memberComm,
            decimal? memberTotal,
            decimal? agWinlost,
            decimal? agComm,
            decimal? agTotal,
            decimal? magWinlost,
            decimal? magComm,
            decimal? magTotal,
            decimal? sagWinlost,
            decimal? sagComm,
            decimal? sagTotal,
            decimal? company)
        {
            CustId = custId;
            Username = username;
            ProductId = productId;
            ProductName = productName;
            Turnover = turnover ?? 0;
            NetTurnover = netTurnover ?? 0;
            GrossComm = grossComm ?? 0;
            MemberWinlost = memberWinlost ?? 0;
            MemberComm = memberComm ?? 0;
            MemberTotal = memberTotal ?? 0;
            AgWinlost = agWinlost ?? 0;
            AgComm = agComm ?? 0;
            AgTotal = agTotal ?? 0;
            MAgWinlost = magWinlost ?? 0;
            MAgComm = magComm ?? 0;
            MAgTotal = magTotal ?? 0;
            SAgWinlost = sagWinlost ?? 0;
            SAgComm = sagComm ?? 0;
            SAgTotal = sagTotal ?? 0;
            Company = company ?? 0;
        }


        public List<string> ProductNames { get; set; }

        public decimal PositionTaking { get; set; }

        public string PositionTakingText => $"{String.Format("{0:0.#}", PositionTaking * percentageFormat)}% PT";

        public int CustId { get; }

        public string Username { get; }
        
        public string TotalName { get; set; }

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
