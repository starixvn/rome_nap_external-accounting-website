using System;
using System.IO;

namespace Fanex.Puffin.Website.Test
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var filePath = "D:/img1.jpg";
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                var base64 = Convert.ToBase64String(buffer);
            }
        }
    }
}