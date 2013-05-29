// -----------------------------------------------------------------------
// <copyright file="TicketControlMode.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Admin.Common
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class Enums
    {
        public enum TicketControlMode
        {
            SingleTicket = 0,
            NumberGame = 1,
            EarlyKO = 2,
            TicketListing = 3,
            MixParlay = 4
        }
    }
}
