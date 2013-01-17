// -----------------------------------------------------------------------
// <copyright file="Transfer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
            get;
            set;
        }

        public bool Weekly
        {
            get
            {
                return Daily ? false : true;
            }
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