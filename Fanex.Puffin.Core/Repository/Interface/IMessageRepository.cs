using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Repository.Interface
{
    public interface IMessageRepository
    {
        /// <summary>
        /// Insert message into table Msg, with IsPublic = 0 (private), MsgType = 1 (message to customer), IsSticky = 0 (default no announcement)
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="TargetId"></param>
        /// <param name="TargetName"></param>
        /// <returns></returns>
        int SetMessage(Message Msg, int[] TargetId, string[] TargetName, int MatchId, string[] TransId);

        /// <summary>
        /// Insert target information of the message inserted into table MsgTarget with TargetID = customerID (member,agent,master,super), TargetName = customer's username, MessageType = 1
        /// </summary>
        /// <param name="MessageId"></param>
        /// <param name="TargetId"></param>
        /// <param name="TargetName"></param>
        /// <returns></returns>
        void AddTargt2Message(int MessageId, int[] TargetId, string[] TargetName, string[] TransID);
    }
}
