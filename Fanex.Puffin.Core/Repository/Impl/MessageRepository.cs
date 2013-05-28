using System;
using Fanex.Data;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Core.Models;
using Fanex.Puffin.Core.Repository.Interface;
using Fanex.Puffin.Core.Services.Impl;

namespace Fanex.Puffin.Core.Repository.Impl
{
    public class MessageRepository : ServiceBase, IMessageRepository
    {

        string CorrectSpace(string scontent)
        {
            if (scontent == null)
                return string.Empty;
            if (scontent.Length == 0)
                return scontent;
            int index = scontent.IndexOf("@@");

            if (index == -1) return scontent;

            if (index == 0)
            {
                if (scontent.Substring(0, 1).Trim().Length > 0)
                    scontent = scontent.Replace("@@", "@@ ");
            }
            else
            {
                if (scontent.Substring(index - 1, 1).Trim().Length > 0)
                    scontent = scontent.Replace("@@", " @@");


            }
            return scontent;
        }

        public int SetMessage(Message Msg, int[] TargetId, string[] TargetName, int MatchId, string[] TransId)
        {
            try
            {
                SetMessageCriteria criteria = new SetMessageCriteria();
                criteria.Creator = Msg.CreatorId;
                criteria.CreatorName = Msg.CreatorName;
                criteria.MatchId = MatchId;

                criteria.Content_en = CorrectSpace(Msg.Content_en);
                criteria.Content_ch = CorrectSpace(Msg.Content_ch);
                criteria.Content_cs = CorrectSpace(Msg.Content_cs);
                criteria.Content_it = CorrectSpace(Msg.Content_it);
                criteria.Content_jp = CorrectSpace(Msg.Content_jp);
                criteria.Content_th = CorrectSpace(Msg.Content_th);
                criteria.Content_ko = CorrectSpace(Msg.Content_ko);
                criteria.Content_es = CorrectSpace(Msg.Content_es);

                // for insert check
                if ((Msg.MessageId == 0) && (TargetId.Length < 1))
                    return 0;

                using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var ds = objectDb.ExecuteDataSet(criteria.GetSpParams());
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            Msg.MessageId = Convert.ToInt32(ds.Tables[0].Rows[0]["MessageId"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return Msg.MessageId;
        }

        public void AddTargt2Message(int MessageId, int[] TargetId, string[] TargetName, string[] TransID)
        {
            try
            {
                for (int i = 0; i < TargetId.Length; i++)
                {
                    SetUsrMessageCriteria criteria = new SetUsrMessageCriteria();
                    criteria.MessageId = MessageId;
                    criteria.TargetId = TargetId[i];
                    criteria.TargetName = TargetName[i];
                    criteria.BettransId = TransID[i];

                    if (TargetId[i] != 0) {
                        using (ObjectDb objDB = new ObjectDb(criteria.GetSettingKey()))
                        {
                            objDB.ExecuteNonQuery(criteria.GetSpParams());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
        }
    }
}
