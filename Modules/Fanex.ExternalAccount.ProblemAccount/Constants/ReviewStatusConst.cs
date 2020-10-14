using Fanex.ExternalAccount.ProblemAccount.Resources;

namespace Fanex.ExternalAccount.ProblemAccount.Constants
{
    public static class ReviewStatusConst
    {
        public const byte All = 2;

        public const byte Reviewed = 1;

        public const byte NotReviewed = 0;

        public static string GetReviewStatusName(byte status)
        {
            switch (status)
            {
                case All:
                    return ViewLang.All;

                case Reviewed:
                    return ViewLang.Reviewed;

                case NotReviewed:
                    return ViewLang.NotReviewed;

                default:
                    return string.Empty;
            }
        }
    }
}
