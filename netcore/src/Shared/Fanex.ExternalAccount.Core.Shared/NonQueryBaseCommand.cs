namespace Fanex.ExternalAccount.Core.Shared
{
    using Fanex.Data.Repository;

    public interface INonQueryBaseCommand: INonQueryCommand
    {
        int ErrCode { get; set; }

        string ErrMsg { get; set; }
    }
}

