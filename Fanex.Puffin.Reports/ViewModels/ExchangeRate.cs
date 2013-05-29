// -----------------------------------------------------------------------
// <copyright file="ExchangeRate.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Fanex.Puffin.Report.ViewModels
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ExchangeRate
    {
        public int Currency { get; set; }

        public string CurrencyName { get; set; }

        public double BasePerUnit { get; set; }

        public double UnitsPerBase { get; set; }
    }
}
