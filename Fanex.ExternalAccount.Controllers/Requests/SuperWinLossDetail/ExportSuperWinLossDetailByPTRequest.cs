namespace Fanex.ExternalAccount.Controllers.Requests
{
    public class ExportSuperWinLossDetailByPTRequest : SuperWinLossDetailByPTRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"SuperWinLossDetailByPT_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}.xls";
    }
}