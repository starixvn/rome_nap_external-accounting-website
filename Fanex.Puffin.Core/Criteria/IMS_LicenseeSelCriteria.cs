// -----------------------------------------------------------------------
// <copyright file="IMS_LicenseeSelCriteria.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.4.23">Created</change>
// </history>

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    public class IMS_LicenseeSelCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "IMS_LicenseeSel";
        }

        public override bool IsValid() { return true; }
    }
}