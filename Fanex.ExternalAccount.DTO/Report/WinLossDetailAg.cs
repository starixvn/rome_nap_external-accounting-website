// -----------------------------------------------------------------------
// <copyright file="WinLossDetailAg.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DTO.Report
{
    using System;

    /// <summary>
    /// Summary description for WinLossDetailAg
    /// </summary>
    [Serializable]
    public class WinLossDetailAg
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public double Turnover { get; set; }

        public double GrossComm { get; set; }

        public double MemberWinlost { get; set; }

        public double PlayerComm { get; set; }

        public double MemberTotal { get; set; }

        public double AgWinlost { get; set; }

        public double AgComm { get; set; }

        public double AgTotal { get; set; }

        public double Company { get; set; }
    }
}