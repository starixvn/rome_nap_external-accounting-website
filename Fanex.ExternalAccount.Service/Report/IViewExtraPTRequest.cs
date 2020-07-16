namespace Fanex.ExternalAccount.Service.Interface.Report
{
    public interface IViewExtraPTRequest
    {
        string GroupName { get; }

        int[] GetSelectedProductIds();

        string GetProductName(int productId);
    }
}