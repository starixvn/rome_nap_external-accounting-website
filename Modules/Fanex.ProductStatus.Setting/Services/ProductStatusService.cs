namespace Fanex.ProductStatus.Setting.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using CommandServices;
    using Criteria;
    using DataAccess;
    using Fanex.ProductStatus.Setting.AppResources;
    using Models;
    using Newtonsoft.Json;
    using Shared;

    public interface ProductStatusService
    {
        IEnumerable<ProductStatusModel> GetProductStatusByGroup(int groupId, int custId, int system, byte settingStatus);

        IEnumerable<ProductStatusModel> GetProductStatus(int productId, int custId, int sportType, int systemId, byte settingStatus);

        IEnumerable<Result> UpdateProductStatus(
            bool isGroup,
            int productId,
            int custId,
            byte custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands,
            byte systemId);

        IEnumerable<Result> MultipleUpdateProductStatus(
            bool isGroup,
            int productId,
            byte custRole,
            int[] custIds,
            string[] custNames,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands,
            byte systemId);
    }

    internal class ProductStatusServiceImpl : ProductStatusService
    {
        private readonly ProductStatusDataAccess statusDataAccess;
        private readonly CriteriaFactoryService criteriaFactory;
        private readonly RefDataService refDataService;
        private readonly IProductProfileService productProfileService;
        private readonly IProductProfileByGroupService productByGroupProfileService;

        public ProductStatusServiceImpl(
            ProductStatusDataAccess statusDataAccess = null,
            CriteriaFactoryService criteriaFactory = null,
            RefDataService refDataService = null,
            IProductProfileService productProfileService = null,
            IProductProfileByGroupService productByGroupProfileService = null)
        {
            this.statusDataAccess = statusDataAccess ?? new ProductStatusDataAccessImpl();
            this.criteriaFactory = criteriaFactory ?? new CriteriaFactoryServiceImpl();
            this.refDataService = refDataService ?? new RefDataServiceImpl();
            this.productProfileService = productProfileService ?? new ProductProfileService();
            this.productByGroupProfileService = productByGroupProfileService ?? new ProductProfileByGroupService();
        }

        private Result UpdateProductStatusByGroup(
            int groupId,
            int custId,
            int actorId,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands)
        {
            var criteria = new UpdateProductStatusByGroupCriteria(custId, JsonConvert.SerializeObject(commands), actorId, ip, groupId, settingStatus);
            var result = statusDataAccess.ExecuteNonQuery(criteria);
            result.ParseDCSProductMessage();

            return result;
        }

        public IEnumerable<ProductStatusModel> GetProductStatusByGroup(int groupId, int custId, int system, byte settingStatus)
        {
            var criteria = new GetProductStatusByGroupIdCriteria(custId, groupId, settingStatus, system);
            var rawData = statusDataAccess.GetProductStatus(criteria).ToList();
            foreach (var item in rawData)
            {
                item.Name = refDataService.GetSportTypeName(item.SportType);
                yield return item;
            }
        }

        public IEnumerable<ProductStatusModel> GetProductStatus(int productId, int custId, int sportType, int systemId, byte settingStatus)
        {
            var criteria = criteriaFactory.CreateSelectCriteria(productId, custId, sportType, settingStatus);
            var rawData = statusDataAccess.GetProductStatus(criteria).ToList();

            return SetProductStatusName(productId, systemId, rawData);
        }

        private IEnumerable<ProductStatusModel> SetProductStatusName(
            int productId,
            int systemId,
            IList<ProductStatusModel> rawData)
        {
            var settingLevel = productProfileService.GetSettingLevel(productId, systemId);

            foreach (var item in rawData)
            {
                item.Name = SetProductStatusName((byte)settingLevel, productId, item.SportType, item.BetType);
                yield return item;
            }
        }

        private string SetProductStatusName(byte settingLevel, int productId, int sportType, int betType)
        {
            if (settingLevel == ProductSettingLevel.Product)
            {
                return refDataService.GetProductName(productId);
            }

            if (settingLevel == ProductSettingLevel.SportType)
            {
                return refDataService.GetSportTypeName(sportType);
            }

            if (settingLevel == ProductSettingLevel.BetType)
            {
                return refDataService.GetBetTypeName(betType);
            }

            return refDataService.GetBetTypeName(betType);
        }

        public IEnumerable<Result> UpdateProductStatus(
            bool isGroup,
            int productId,
            int custId,
            byte custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands,
            byte systemId)
        {
            var result = isGroup
                ? UpdateProductStatusByGroup(productId, custId, actorId, ip, settingStatus, commands)
                : UpdateProductStatus(productId, custId, custRole, actorId, actorName, ip, settingStatus, commands);

            if (!result.IsSuccess() && !result.IsDoNothing())
            {
                return new[] { result };
            }

            var results = new List<Result>();

            if (result.IsSuccess())
            {
                results.Add(result);
            }

            var umMessage = GetUMMessageForEditSingle(isGroup, productId, custId, systemId, commands, settingStatus);

            if (!string.IsNullOrWhiteSpace(umMessage))
            {
                results.Add(new Result(Result.UMNonDCSErrorCode, umMessage));
            }

            return results;
        }

        private Result UpdateProductStatus(
            int productId,
            int custId,
            byte custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands)
            => ProductStatusCommandServiceFactory(productId).UpdateProductStatus(
                productId,
                custId,
                custRole,
                actorId,
                actorName,
                ip,
                settingStatus,
                commands);

        private ProductStatusCommandService ProductStatusCommandServiceFactory(int productId)
            => new DefaultProductStatusCommandService(statusDataAccess, criteriaFactory);

        public IEnumerable<Result> MultipleUpdateProductStatus(
            bool isGroup,
            int productId,
            byte custRole,
            int[] custIds,
            string[] custNames,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands,
            byte systemId)
        {
            var custNameHash = new Dictionary<int, string>();
            Result successResult = null;
            Result errorResult = null;
            var successCustName = new List<string>();
            var errorCustName = new List<string>();
            var multipleResults = new List<MultipleResult>();

            for (int i = 0, custIdsLength = custIds.Length; i < custIdsLength; i++)
            {
                var customerId = custIds[i];
                var customerName = custNames[i];
                custNameHash[customerId] = customerName;

                var result = isGroup
                    ? UpdateProductStatusByGroup(productId, customerId, actorId, ip, settingStatus, commands)
                    : UpdateProductStatus(productId, customerId, custRole, actorId, actorName, ip, settingStatus, commands);

                if (result.IsSuccess())
                {
                    successResult = result;
                    successCustName.Add(customerName);
                }

                if (!result.IsSuccess() && !result.IsDoNothing())
                {
                    errorResult = result;
                    errorCustName.Add(customerName);
                }
            }

            if (successResult != null || errorResult != null)
            {
                multipleResults.Add(successResult != null && successCustName.Count > 0
                    ? new MultipleResult(successResult, 0, string.Join(", ", successCustName))
                    : new MultipleResult(errorResult, 0, string.Join(", ", errorCustName)));
            }

            GetUMMessageForEditMultiple(isGroup, productId, custIds, custNameHash, systemId, commands, settingStatus)
                .GroupBy(r => r.Message).ToList().ForEach(group =>
                {
                    var mulResult = new MultipleResult(
                        new Result(Result.UMNonDCSErrorCode, group.Key),
                        0,
                        string.Join(", ", group.Select(r => r.CustName)));
                    multipleResults.Add(mulResult);
                });

            return multipleResults;
        }

        #region UM Functions

        private string GetUMMessageForEditSingle(bool isGroup, int productId, int custId, byte systemId, IEnumerable<ProductStatusCommand> commands, byte settingStatus)
        {
            var criteria = isGroup
                ? new CheckProducStatusUMCriteria(custId, groupId: productId, settingStatus: settingStatus)
                : new CheckProducStatusUMCriteria(custId, productId: productId, settingStatus: settingStatus);

            var productStatusUM = statusDataAccess
                .GetProductStatusUM(criteria)
                .ToList();

            if (productStatusUM.Count == 0)
            {
                return string.Empty;
            }

            var settingLevel = isGroup ?
                (byte)productByGroupProfileService.GetSettingLevel(productId, systemId, settingStatus)
                : (byte)productProfileService.GetSettingLevel(productId, systemId);

            return BuildUMMessages(productStatusUM, settingLevel, commands);
        }

        private string BuildUMMessages(IEnumerable<ProductStatusUM> productStatusUM, byte settingLevel, IEnumerable<ProductStatusCommand> commands)
        {
            var allProductOrSportType = productStatusUM.Where(m => m.ProductId == 0 || m.SportType == 0);
            var umMessages = new List<string>();
            if (allProductOrSportType != null && allProductOrSportType.Any())
            {
                foreach (var product in allProductOrSportType)
                {
                    umMessages.Add(BuildUMMessage(product, 0));
                }
            }
            else
            {
                foreach (var message in productStatusUM)
                {
                    if (commands.Any(p => p.SportType == message.SportType))
                    {
                        umMessages.Add(BuildUMMessage(message, settingLevel));
                    }
                }
            }

            return umMessages.Count == 0
                ? string.Empty
                : $"{Resources.UMProductStatus}<br/>{string.Join("<br>", umMessages)}";
        }

        private string BuildUMMessage(ProductStatusUM message, byte settingLevel)
            => string.Format(
                Resources.UMProductStatusPattern,
                SetProductStatusName(message, settingLevel),
                message.SettingType == 0 ? Resources.CustomerSettings : Resources.ProductStatus,
                message.BeginTime.ToLongDateWithSlash(),
                message.EndTime.ToLongDateWithSlash());

        private string SetProductStatusName(ProductStatusUM message, byte settingLevel)
        {
            if (message.ProductId == 0)
            {
                return Resources.AllProducts;
            }

            if (message.SportType == 0)
            {
                return refDataService.GetProductName(message.ProductId);
            }

            return SetProductStatusName(settingLevel, message.ProductId, message.SportType, message.BetType);
        }

        private IEnumerable<MultipleResult> GetUMMessageForEditMultiple(
            bool isGroup,
            int productId,
            int[] custIds,
            IDictionary<int, string> custNameHash,
            byte systemId,
            IEnumerable<ProductStatusCommand> commands,
            byte settingStatus)
        {
            var criteria = isGroup
                ? new CheckProducStatusUMCriteria(string.Join(",", custIds), groupId: productId, settingStatus: settingStatus)
                : new CheckProducStatusUMCriteria(string.Join(",", custIds), productId);
            var productStatusUM = statusDataAccess
                .GetProductStatusUM(criteria)
                .GroupBy(m => m.CustId)
                .ToList();

            if (productStatusUM.Count == 0)
            {
                yield break;
            }

            var settingLevel = isGroup ?
                (byte)productByGroupProfileService.GetSettingLevel(productId, systemId, settingStatus)
                : (byte)productProfileService.GetSettingLevel(productId, systemId);

            foreach (var group in productStatusUM)
            {
                var message = BuildUMMessages(group, settingLevel, commands);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    yield return new MultipleResult(new Result(Result.UMNonDCSErrorCode, message), group.Key, custNameHash[group.Key]);
                }
            }
        }

        #endregion UM Functions
    }
}