namespace Fanex.ExternalAccount.DTO.Report
{
    public class SuperWinLossDetailMCByProductDTO : SuperWinLossDetailByProductDTO
    {
        public SuperWinLossDetailMCByProductDTO(
            int custId,
            string username,
            int currencyId,
            string currencyName,
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
            decimal? company) : base(custId, username, productId, productName,
                turnover, netTurnover, grossComm, memberWinlost, memberComm, memberTotal, agWinlost, agComm, agTotal, magWinlost, magComm, magTotal, sagWinlost, sagComm, sagTotal, company)
        {
            CurrencyId = currencyId;
            CurrencyName = currencyName;
        }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }
    }

}
