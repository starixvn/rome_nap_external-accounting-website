namespace Fanex.Puffin.Core.Models
{
    public class Message
    {
        private string _Content_ch = string.Empty;
        private string _Content_cs = string.Empty;
        private string _Content_en = string.Empty;
        private string _Content_es = string.Empty;
        private string _Content_it = string.Empty;
        private string _Content_jp = string.Empty;
        private string _Content_ko = string.Empty;
        private string _Content_th = string.Empty;
        private string _CreatorName = string.Empty;
        private string _UserName = string.Empty;

        /// <summary>
        /// Message ID
        /// </summary>        
        public int MessageId { get; set; }

        /// <summary>
        /// English Message Content
        /// </summary>
        public string Content_en
        {
            get { return _Content_en; }
            set { _Content_en = value; }
        }

        /// <summary>
        /// Tridional Chinese Message Content
        /// </summary>
        /// 
        public string Content_ch
        {
            get { return _Content_ch; }
            set { _Content_ch = value; }
        }

        /// <summary>
        /// Simple Chinese Message Content
        /// </summary>
        public string Content_cs
        {
            get { return _Content_cs; }
            set { _Content_cs = value; }
        }

        /// <summary>
        /// Japanese Message Content
        /// </summary>
        public string Content_jp
        {
            get { return _Content_jp; }
            set { _Content_jp = value; }
        }

        /// <summary>
        /// Italian Message Content
        /// </summary>
        public string Content_it
        {
            get { return _Content_it; }
            set { _Content_it = value; }
        }

        /// <summary>
        /// Korean Message Content
        /// </summary>
        public string Content_ko
        {
            get { return _Content_ko; }
            set { _Content_ko = value; }
        }

        /// <summary>
        /// Thai Message Content
        /// </summary>
        public string Content_th
        {
            get { return _Content_th; }
            set { _Content_th = value; }
        }

        /// <summary>
        /// Spanish Message Content
        /// </summary>
        public string Content_es
        {
            get { return _Content_es; }
            set { _Content_es = value; }
        }

        /// <summary>
        /// MessageType
        /// </summary>
        public int MessageType { get; set; }

        public int MsgType { get; set; }

        /// <summary>
        /// Is special Message or not
        /// </summary>
        public bool IsSticky { get; set; }

        /// <summary>
        /// Creator ID
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Creator Name
        /// </summary>
        public string CreatorName
        {
            get { return _CreatorName; }
            set { _CreatorName = value; }
        }

        
    }
}
