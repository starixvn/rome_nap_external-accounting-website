// -----------------------------------------------------------------------
// <copyright file="BaseModel.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using System;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Admin.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
