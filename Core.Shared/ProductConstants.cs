using System.Collections.Generic;

namespace Fanex.ExternalAccount.Core.Shared
{
    public static class ProductConstants
    {
        public const int Sportsbook = 1;
        public const int VirtualSports = 5;
        public const int Keno = 8;
        public const int ProgressiveBonus = 10;
        public const int AllBet = 21;
        public const int SAGaming = 39;
        public const int SexyGaming = 45;
        public const int GTCasino = 46;
        public const int WM = 51;
        public const int ION = 52;
        public const int BigGame = 57;
        public const int BBin = 47;
        public const int Lottery = 28;
        public const int TableGames = 40;
        public const int RNGLottery = 44;
        public const int RNGWar = 50;
        public const int DummyLiveCasinoProductId = 41;
        public const int SabaVirtualSports = 54;
        public const int PragmaticPlay = 32;
        public const int SpadeGaming = 36;
        public const int CardClub = 34;
        public const int PGSoft = 55;
        public const int Joker = 56;
        public const int GDG = 59;
        public const int Cashout = 25;
        public const int FGG = 70;
        public const int MGRNGCasino = 73;
        public const int Live22 = 79;
        public const int FastSpin = 87;
        public const int VGaming = 66;
        public const int FaChai = 89;
        public const int ASKMESLOT = 103;

        public const int DummyRNGProductId = 53;

        public static readonly List<int> ProductListForTooltipsRNGCasino = new List<int>
        {
            RNGWar
        };

        public static readonly List<int> ProductListHasProductDetail = new List<int>
        {
            PragmaticPlay,
            SexyGaming,
            SpadeGaming,
            AllBet,
            Sportsbook,
            MGRNGCasino,
            Live22,
            FastSpin,
            VGaming,
            FaChai,
            ASKMESLOT
        };
    }
}