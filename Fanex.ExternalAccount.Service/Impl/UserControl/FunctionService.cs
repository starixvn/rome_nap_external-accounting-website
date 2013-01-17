namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;

    public class FunctionService : ServiceBase, IFunctionService
    {
        private readonly IFunctionDao funcDao = null;

        public FunctionService(IFunctionDao funcDao = null)
        {
            this.funcDao = funcDao ?? new FunctionDao();
        }

        #region IFunctionService Members

        public Result CreateFunction(string name, string key, string description, int parentId, string parentName, int displayOrder, int custId, string ip, string assignedGroupIDs, out int functionId)
        {
            functionId = 0;

            try
            {
                return this.funcDao.CreateFunction(name, key, description, parentId, parentName, displayOrder, custId, ip, assignedGroupIDs, out functionId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public Result UpdateFunction(int id, string name, string description, int parentId, string parentName, int displayOrder, int custId, string ip, string oldAssignedGroupIDs, string assignedGroupIDs)
        {
            try
            {
                return this.funcDao.UpdateFunction(id, name, description, parentId, parentName, displayOrder, custId, ip, oldAssignedGroupIDs, assignedGroupIDs);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public IEnumerable<Function> GetAllFunctions()
        {
            try
            {
                return this.funcDao.GetAllFunctions();
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public Function GetFunctionPolicy(int funcId)
        {
            try
            {
                return this.funcDao.GetFunctionPolicy(funcId);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            try
            {
                return this.funcDao.UpdateStatus(id, isTest, custId, ip);
            }
            catch (Exception ex)
            {
                base.ProcessException(ex);
            }

            return null;
        }

        #endregion IFunctionService Members
    }
}