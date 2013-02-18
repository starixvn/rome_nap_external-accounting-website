namespace Fanex.ExternalAccount.DTO.Statement
{
    using System;

    /// <summary>
    /// Summary description for Statements
    /// </summary>
    public class Statement
    {
        public string AccDate { get; set; }

        public long RefNo { get; set; }

        public double TotalAmt { get; set; }

        public string TransType { get; set; }

        public string TransDesc { get; set; }
    }
}