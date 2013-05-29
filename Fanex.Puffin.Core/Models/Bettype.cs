// -----------------------------------------------------------------------
// <copyright file="Bettype.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using System;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Core.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class Bettypes:BaseModel
    {
        public int Type { get; set; }

        public bool LiveIndicator { get; set; }
    }
}
