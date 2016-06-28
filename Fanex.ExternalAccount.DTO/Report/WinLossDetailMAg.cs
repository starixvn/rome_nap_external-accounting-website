// -----------------------------------------------------------------------
// <copyright file="WinLossDetailMAg.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DTO.Report
{
    using System;

    /// <summary>
    /// TODO: Summary description.
    /// </summary>
    public class WinLossDetailMAg
    {
        public int CustId { get; set; }

        public string Username { get; set; }

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

        public double Company { get; set; }
    }
}