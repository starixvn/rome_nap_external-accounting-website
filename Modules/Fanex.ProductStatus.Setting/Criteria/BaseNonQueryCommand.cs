using Fanex.Data.Repository;

namespace Fanex.ProductStatus.Setting.Criteria
{
    public interface IBaseNonQueryCommand : INonQueryCommand
    {
        int ErrCode { get; set; }

        string ErrMsg {  get; set; }

        int MsgType { get; set; }
    }
}
