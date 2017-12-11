namespace Fanex.ExternalAccount.DTO.Customer.Casino
{
    using System.Collections.Generic;

    public class CasinoObject
    {
        public CasinoCustomer CustomerInfo
        {
            get;
            set;
        }

        public CasinoCustomer CasinoInfo
        {
            get;
            set;
        }

        public List<Stakes> CasinoGame
        {
            get;
            set;
        }

        public List<Stakes> MiniGame
        {
            get;
            set;
        }

        public List<GameKindDetail> Pts
        {
            get;
            set;
        }
    }
}
