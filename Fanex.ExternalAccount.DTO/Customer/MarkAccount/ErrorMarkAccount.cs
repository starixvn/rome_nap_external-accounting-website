namespace Fanex.ExternalAccount.DTO.Customer.MarkAccount
{
    public class ErrorMarkAccount
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public bool Isfinished { get; set; }
    }
}
