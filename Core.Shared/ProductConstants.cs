using System.Collections.Generic;

namespace Fanex.ExternalAccount.Core.Shared
{
    public static class ProductConstants
    {
        public const int Sportsbook = 1;
        public const int NumberGame = 3;
        public const int VirtualSports = 5;
        public const int Keno = 8;
        public const int GoldDeluxe = 12;
        public const int Sportsbook2 = 16;
        public const int RNGCasino = 6;
        public const int ProgressiveBonus = 10;
        public const int AllBet = 21;
        public const int SAGaming = 39;
        public const int LiveCasino = 41;
        public const int SexyGaming = 45;
        public const int GTCasino = 46;
        public const int WM = 51;
        public const int ION = 52;
        public const int BigGame = 57;
        public const int BBin = 47;
        public const int VoidBridge = 22;  // Real name is Macau Game
        public const int NG2 = 23;
        public const int FishingWorld = 24;
        public const int Lottery = 28;
        public const int TableGames = 40;
        public const int RNGLottery = 44;
        public const int VirtualGames = 38;
        public const int RNGWar = 50;
        public const int Sabaclub = 42;
        public const int DummyLiveCasinoProductId = 41;
        public const int SabaVirtualSports = 54;
        public const int PragmaticPlay = 32;
        public const int SpadeGaming = 36;
        public const int CardClub = 34;
        public const int PGSoft = 55;
        public const int Joker = 56;
        public const int GDG = 59;
        public const int VGaming = 66;
        public const int Bitcoin = 69;
        public const int Cashout = 25;

        public const int DummyRNGProductId = 53;

        public static readonly List<int> ProductListForTooltipsRNGCasino = new List<int>
        {
            ProductConstants.RNGWar,
            ProductConstants.Sabaclub,
            ProductConstants.VGaming
        };

        public static readonly List<int> ProductListHasProductDetail = new List<int>
        {
            ProductConstants.PragmaticPlay,
            ProductConstants.SexyGaming
        };
    }
}