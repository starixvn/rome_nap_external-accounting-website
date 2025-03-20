namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Criteria.UserControl.Menu;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Fanex.Data.Repository;
    using Utilities;
    
    public interface IMenuDao
    {
        IEnumerable<Menu> GetAllMenus();

        UserCache GetMenuByUserId(int userId);

        Result CreateMenu(Menu menu, int custId, string ip);

        Result UpdateMenu(Menu menu, int custId, string ip);

        Result UpdateMenuStatus(int id, bool isActive, int custId);

        Result DeleteMenu(int id, int custId);
    }
    
    public class MenuDao : IMenuDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public MenuDao(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<Menu> GetAllMenus()
        {
            var criteria = new GetAllMenusCriteria();
            return dynamicRepository.Fetch<Menu>(criteria) ?? new List<Menu>();
        }

        public UserCache GetMenuByUserId(int userId)
        {
            var criteria = new GetMenuByUserIdCriteria { UserId = userId };
            var ds = dynamicRepository.FetchDataSet(criteria);
            
            if (ds?.Tables.Count == 2)
            {
                var menu = ds.Tables[0];
                var func = ds.Tables[1];

                var menus = (from c in menu.AsEnumerable()
                            select new Menu
                            {
                                MenuId = c.Field<int>("MenuId"),
                                MenuName = c.Field<string>("MenuName"),
                                ParentId = c.Field<int>("ParentId"),
                                ParentName = c.Field<string>("ParentName"),
                                Url = c.Field<string>("Url"),
                                FuncId = c.Field<int>("FuncId"),
                                FuncKey = c.Field<string>("FuncKey"),
                                DisplayOrder = c.Field<int>("DisplayOrder"),
                                IsActive = c.Field<bool>("Active")
                            }).ToList();

                var funcs = (from c in func.AsEnumerable()
                            select new Function
                            {
                                FuncID = c.Field<int>("funcid"),
                                ParentID = c.Field<int>("parentId"),
                                FuncName = c.Field<string>("funcName"),
                                FuncKey = c.Field<string>("funcKey"),
                                Policy = c.Field<EPolicy>("policy"),
                                IsTest = c.Field<bool>("IsTest")
                            }).ToList();

                return new UserCache
                {
                    MenuList = menus,
                    PolicyList = funcs
                };
            }

            return null;
        }

        public Result CreateMenu(Menu menu, int custId, string ip)
        {
            var criteria = new CreateMenuCriteria
            {
                UserId = custId,
                FuncId = menu.FuncId,
                ParentId = menu.ParentId,
                DisplayOrder = menu.DisplayOrder,
                MenuName = menu.MenuName,
                MenuDesc = menu.MenuDesc,
                ParentName = menu.ParentName,
                URL = menu.Url,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateMenu(Menu menu, int custId, string ip)
        {
            var criteria = new UpdateMenuCriteria
            {
                UserId = custId,
                MenuId = menu.MenuId,
                FuncId = menu.FuncId,
                ParentId = menu.ParentId,
                DisplayOrder = menu.DisplayOrder,
                MenuName = menu.MenuName,
                MenuDesc = menu.MenuDesc,
                ParentName = menu.ParentName,
                URL = menu.Url,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateMenuStatus(int id, bool isActive, int custId)
        {
            var criteria = new UpdateMenuStatusCriteria
            {
                UserId = custId,
                MenuId = id,
                Active = isActive
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result DeleteMenu(int id, int custId)
        {
            var criteria = new DeleteMenuCriteria
            {
                UserId = custId,
                MenuId = id
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}