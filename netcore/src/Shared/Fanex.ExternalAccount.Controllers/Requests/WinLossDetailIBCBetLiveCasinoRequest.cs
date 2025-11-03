//namespace Fanex.ExternalAccount.Controllers.Requests
//{
//    using Core.Context;
//    using Core.Reports;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    public class WinLossDetailIBCBetLiveCasinoRequest
//    {
//        private const string Seperator = ", ";
//        private const string ShowAll = "showAll";

//        private readonly Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter;
//        private readonly IExtRequest extRequest;

//        public WinLossDetailIBCBetLiveCasinoRequest() : this(null, null)
//        {
//        }

//        public WinLossDetailIBCBetLiveCasinoRequest(
//            Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
//            IExtRequest extRequest)
//        {
//            this.selectedProductQueryGetter = selectedProductQueryGetter ?? new FilterController().AsCollection;
//            this.extRequest = extRequest ?? new ExtRequest();
//        }

//        public int GroupId => extRequest.GetGroupId();
//        public string GroupName => extRequest.GetGroupName();
//        public DateTime FromDate { get; set; }
//        public DateTime ToDate { get; set; }

//        internal string GetProductNames(List<KeyValuePair<string, string>> productList)
//        {
//            var filterParams = selectedProductQueryGetter(GroupId);
//            var formatedProducts = string.Empty;

//            filterParams.ToList().ForEach(o =>
//            {
//                productList.ForEach(p =>
//                {
//                    if (!p.Key.Equals(ShowAll, StringComparison.OrdinalIgnoreCase)
//                        && o.Value
//                        && o.Key.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
//                    {
//                        formatedProducts += p.Value + Seperator;
//                    }
//                });
//            });

//            if (formatedProducts.Length > 0)
//            {
//                formatedProducts = formatedProducts.TrimEnd(Seperator.ToCharArray());
//            }

//            return formatedProducts;
//        }
//    }
//}