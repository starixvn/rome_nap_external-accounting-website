// -----------------------------------------------------------------------
// <copyright file="CacheRef.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Fanex.Common;
using RemotingRef;

namespace Fanex.Puffin.Core.Services.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class CacheRef
    {
        private static string TWLang(string lang)
        {
            switch (lang)
            {
                case "en-US": return "en";
                case "zh-TW": return "ch"; // Traditional
                case "zh-CN": return "cs"; // Simplified
                case "ja-JP": return "jp";
                case "th-TH": return "th"; // Thai
                default: return "en";
            }
        }

        public static string GetBettype(object bettypeId)
        {
            return GetBettype(bettypeId, null);
        }

        public static string GetBettype(object bettypeId, object betID)
        {
            // resourceId remove first character
            string resourceId = string.Empty;
            if (betID != null && betID.ToString().Length > 1)
            {
                resourceId = betID.ToString().Substring(1);
            }

            return RefData.GetBetTypeName(bettypeId.ToString(), TWLang("en-US"), string.Empty, resourceId);
        }

        public static string GetSportName(object sportID)
        {
            return RefData.GetSportName(Converter.ToString2(sportID), TWLang("en-US"), string.Empty);
        }

        public static string GetTeamName(object teamID)
        {
            return RefData.GetTeamName(Converter.ToString2(teamID), TWLang("en-US"), string.Empty);
        }

        public static string GetLeagueName(object leagueID)
        {
            return RefData.GetLeagueName(Converter.ToString2(leagueID), TWLang("en-US"), string.Empty);
        }

        public static string GetFncIndexName(object indexId)
        {
            return RefData.GetFncIndexName(Converter.ToString2(indexId), TWLang("en-US"));
        }

        public static string GetHorseTeamName(object codeID)
        {
            return RefData.GetHorseTeamName(Converter.ToString2(codeID), TWLang("en-US"));
        }

        public static string GetOddsTypeName(object oddsType)
        {
            return RefData.GetOddsTypeName(Converter.ToString2(oddsType), TWLang("en-US"));
        }

        public static string GetCombinationName(string betteam)
        {
            string name = string.Empty;
            switch (betteam)
            {
                case "110":
                    name = "Trixie";
                    break;
                case "1110":
                    name = "Yankee";
                    break;
                case "11110":
                    name = "Canadian";
                    break;
                case "111110":
                    name = "Heinz";
                    break;
                case "1111110":
                    name = "Super Heinz";
                    break;
                case "11111110":
                    name = "Goliath";
                    break;
            }

            return name;
        }

        public static string GetLiveCasinoResult(int resultId)
        {
            string result = string.Empty;
            /*switch(resultId)
            {
                case 1:
                    result = "Banker";
                    break;
                case 2:
                    result = "Player";
                    break;
                case 3:
                    result = "Tie";
                    break;
                case 4:
                    result = "Big";
                    break;
                case 5:
                    result = "Small";
                    break;
                case 6:
                    result = "B.Pair";
                    break;
                case 7:
                    result = "P.Pair";
                    break;
            }*/
            return RefData.GetCasinoBetItem(Converter.ToString2(resultId), TWLang("en-US"));
        }

        public static string GetRouletteBetItem(int itemId)
        {
            return RefData.GetRouletteBetItem(Converter.ToString2(itemId), TWLang("en-US"));
        }

        public static string GetBettypeNameSportingSolution(object betypeId, string resourceId)
        {
            string bettypeName = string.Empty;
            bettypeName = RefData.GetBetTypeName(Converter.ToString2(betypeId), TWLang("en-US"), string.Empty, resourceId);
            return bettypeName;
        }

        public static string GetCasinoBetItem(object betStatusId)
        {
            return RefData.GetCasinoBetItem(betStatusId.ToString(), TWLang("en-US"), string.Empty);
        }

        public static string GetRouletteBetItem(object itemId)
        {
            return RefData.GetRouletteBetItem(itemId.ToString(), TWLang("en-US"), string.Empty);
        }
    }
}
