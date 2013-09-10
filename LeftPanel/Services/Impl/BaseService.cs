// -----------------------------------------------------------------------
// <copyright file="BaseService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System;
    using System.Data;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseService : ILeftPanelService
    {
        public int Super { get { return 4; } }

        public int Master { get { return 3; } }

        public int Agent { get { return 2; } }

        public int Member { get { return 1; } }

        public virtual CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1)
        {
            throw new NotImplementedException();
        }

        public virtual Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            throw new NotImplementedException();
        }

        public string GetCashColumn(int roleId)
        {
            if (roleId == this.Super)
            {
                return "superCash";
            }
            else if(roleId == this.Master)
            {
                return "masterCash";
            }
            else if(roleId == this.Agent)
            {
                return "agentCash";
            }
            else
            {
                return "cash";
            }
        }

        public string GetYesterdayCashColumn(int roleId)
        {
            if (roleId == this.Super)
            {
                return "yesterday_superCash";
            }
            else if (roleId == this.Master)
            {
                return "yesterday_masterCash";
            }
            else if (roleId == this.Agent)
            {
                return "yesterday_agentCash";
            }
            else
            {
                return "yesterday_cash";
            }
        }

        public string GetUsedCreditColumn(int roleId)
        {
            if (roleId == this.Super)
            {
                return "superUsedCredit";
            }
            else if (roleId == this.Master)
            {
                return "masterUsedCredit";
            }
            else if (roleId == this.Agent)
            {
                return "agentUsedCredit";
            }
            else
            {
                return "superUsedCredit";
            }
        }

        public string GetCustomerCash()
        {
            return "customerCash";
        }

        public string GetYesterdayCustomerCash()
        {
            return "yesterday_customerCash";
        }

        public string GetOutstandingColumn(int roleId)
        {
            var levelName = string.Empty;

            switch (roleId)
            {
                case 4:
                    levelName = "super";
                    break;
                case 3:
                    levelName = "master";
                    break;
                case 2:
                    levelName = "agent";
                    break;
                default:
                    break;
            }

            return roleId != this.Member ? levelName + "PositionTaking" : "outstanding";
        }

        public virtual DataTable GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        public virtual DataRowCollection GetOnlineList(int custid)
        {
            throw new NotImplementedException();
        }
    }
}