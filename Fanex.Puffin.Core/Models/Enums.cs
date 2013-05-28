// -----------------------------------------------------------------------
// <copyright file="Enums.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Core.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public enum ProductType
    {
        Sportsbook = 0,
        NumberGame = 1,
        MixParlay = 2,
        TicketListing = 3
    }
}
