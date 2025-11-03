namespace Fanex.ExternalAccount.DTO.DataRange
{
    using System;

    public class CustomerDataRange
    {
        public byte Type { get; set; }

        public DateTime? MinDate { get; set; }

        public int? LimitDays { get; set; }
    }
}
