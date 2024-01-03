namespace Fanex.ExternalAccount.Statement.Repositories
{
    using System;

    public class DummyDataRepository
    {
        public static DateTime BalanceUpDay
        {
            get
            {
                return new DateTime(2014, 11, 29);
            }
        }

        public static DateTime MoveBetsDay
        {
            get
            {
                return new DateTime(2014, 12, 1);
            }
        }
    }
}
