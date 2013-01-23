namespace Fanex.ExternalAccount.DTO.Statement
{
    using System;

    /// <summary>
    /// Summary description for Statement
    /// </summary>
    [Serializable]
    public class Statement
    {
        public DateTime AccDate { get; set; }

        public int RefNo { get; set; }

        public double TotalAmt { get; set; }

        public string TransType { get; set; }

        public string TransDesc { get; set; }
    }
}