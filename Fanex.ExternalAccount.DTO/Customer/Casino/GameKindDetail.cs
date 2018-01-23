namespace Fanex.ExternalAccount.DTO.Customer.Casino
{
    public class GameKindDetail : GameTypeDetail
    {
        public GameKindDetail()
        {
        }

        public GameKindDetail(int codeID, string codeNo, string codeName, string codeSName)
        {
            CodeID = codeID;
            CodeNo = codeNo;
            CodeName = codeName;
            CodeSName = codeSName;
        }

        public int CodeID { get; set; }

        public string CodeNo { get; set; }

        public string CodeName { get; set; }

        public string CodeSName { get; set; }
    }
}
