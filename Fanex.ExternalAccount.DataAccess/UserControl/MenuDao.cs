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
            var ds = dynamicRepository.FetchMultiple<Menu, Function>(criteria);
            if (ds.Item1.Any() && ds.Item2.Any())
            {
                return new UserCache
                {
                    MenuList = ds.Item1.ToList(),
                    PolicyList = ds.Item2.ToList()
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