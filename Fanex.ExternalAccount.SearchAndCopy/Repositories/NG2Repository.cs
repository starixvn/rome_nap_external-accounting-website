namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;

    public interface ING2Repository
    {
        NG2Status GetStatus(ICriteria criteria);
    }

    public class NG2Repository: ING2Repository
    {
        private readonly IDynamicRepository dynamicRepository;

        public NG2Repository()
            : this(null)
        {
        }

        public NG2Repository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public NG2Status GetStatus(ICriteria criteria)
        {
            return dynamicRepository.Get<NG2Status>(criteria);
        }
    }
}
