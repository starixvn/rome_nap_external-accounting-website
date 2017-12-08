namespace Fanex.ExternalAccount.DTO.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Transfer
    {
        public bool Daily
        {
            get
            {
                return Weekly ? false : true;
            }
        }

        public bool Weekly
        {
            get;
            set;
        }

        public bool Mon
        {
            get;
            set;
        }

        public bool Tue
        {
            get;
            set;
        }

        public bool Wed
        {
            get;
            set;
        }

        public bool Thu
        {
            get;
            set;
        }

        public bool Fri
        {
            get;
            set;
        }

        public bool Sat
        {
            get;
            set;
        }

        public bool Sun
        {
            get;
            set;
        }
    }
}