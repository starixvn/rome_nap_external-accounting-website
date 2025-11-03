
namespace Fanex.ExternalAccount.DTO.AutoTransfer
{
    public class ToastContentDto
    {
        public ToastContentDto(string title,string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; set; }
        public string Body { get; set; }
    }
}
