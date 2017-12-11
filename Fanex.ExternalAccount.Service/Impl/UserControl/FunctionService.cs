namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Logging.Extensions.Web;

    public class FunctionService : IFunctionService
    {
        private readonly IFunctionDao _funcDao = null;

        public FunctionService(IFunctionDao funcDao = null)
        {
            this._funcDao = funcDao ?? new FunctionDao();
        }

        #region IFunctionService Members

        public Result CreateFunction(
            string name, 
            string key, 
            string description,
            int parentId, 
            string parentName,
            int displayOrder, 
            int custId,
            string ip, 
            string assignedGroupIDs,
            out int functionId)
        {
            functionId = 0;

            try
            {
                return this._funcDao.CreateFunction(name, key, description, parentId, parentName, displayOrder, custId, ip, assignedGroupIDs, out functionId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Result UpdateFunction(
            int id, 
            string name,
            string description,
            int parentId, 
            string parentName, 
            int displayOrder,
            int custId,
            string ip, 
            string oldAssignedGroupIDs,
            string assignedGroupIDs)
        {
            try
            {
                return this._funcDao.UpdateFunction(id, name, description, parentId, parentName, displayOrder, custId, ip, oldAssignedGroupIDs, assignedGroupIDs);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public IEnumerable<Function> GetAllFunctions()
        {
            try
            {
                return this._funcDao.GetAllFunctions();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Function GetFunctionPolicy(int funcId)
        {
            try
            {
                return this._funcDao.GetFunctionPolicy(funcId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            try
            {
                return this._funcDao.UpdateStatus(id, isTest, custId, ip);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion IFunctionService Members
    }
}