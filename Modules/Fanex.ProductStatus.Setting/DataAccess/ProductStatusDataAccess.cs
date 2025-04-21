namespace Fanex.ProductStatus.Setting.DataAccess
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ProductStatus.Setting.AppResources;
    using Fanex.ProductStatus.Setting.Criteria;
    using Models;

    public interface ProductStatusDataAccess
    {
        IEnumerable<ProductStatusModel> GetProductStatus(ICriteria criteria);

        Result ExecuteNonQuery(IBaseNonQueryCommand command);

        IEnumerable<ProductStatusUM> GetProductStatusUM(ICriteria criteria);
    }

    internal class ProductStatusDataAccessImpl : ProductStatusDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProductStatusDataAccessImpl() : this(null)
        {
        }

        public ProductStatusDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProductStatusModel> GetProductStatus(ICriteria criteria)
            => dynamicRepository.Fetch<ProductStatusModel>(criteria);

        public Result ExecuteNonQuery(IBaseNonQueryCommand command)
        {
            dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrCode == 0 ? Resources.StatusUpdatedSuccessfully: command.ErrMsg);
        }

        public IEnumerable<ProductStatusUM> GetProductStatusUM(ICriteria criteria)
            => dynamicRepository.Fetch<ProductStatusUM>(criteria);
    }
}