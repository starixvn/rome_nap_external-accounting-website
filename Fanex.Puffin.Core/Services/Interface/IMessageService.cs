using System.Collections.Generic;
using System.Data;
using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Services.Interface
{
    public interface IMessageService
    {
        Result SendPrivateMsgToUsers(DataTable dtMsg, string matchId, Message pmsg, ref List<string> listTransIdDone);
    }
}
