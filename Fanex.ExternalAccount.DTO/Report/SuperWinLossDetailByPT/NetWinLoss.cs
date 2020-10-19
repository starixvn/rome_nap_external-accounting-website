namespace Fanex.ExternalAccount.DTO.Report
{
    using System.Data;
    using Fanex.Common.Extensions;

    public class NetWinLoss
    {
        public NetWinLoss()
        {

        }

        public NetWinLoss(decimal total)
        {
            Total = total;
        }

        public NetWinLoss(decimal winloss, decimal comm)
        {
            WinLoss = winloss;
            Comm = comm;
            Total = winloss + comm;
        }

        public NetWinLoss(decimal? winloss, decimal? comm, decimal total)
        {
            WinLoss = winloss;
            Comm = comm;
            Total = total;
        }

        public decimal? WinLoss { get; private set; }

        public decimal? Comm { get; private set; }

        public decimal Total { get; private set; }

        public static NetWinLoss Add(NetWinLoss winlossA, NetWinLoss winlossB)
        {
            if (winlossA == null || winlossB == null)
            {
                return winlossA ?? winlossB;
            }

            return new NetWinLoss(
               winloss: winlossA?.WinLoss + winlossB?.WinLoss,
               comm: winlossA?.Comm + winlossB?.Comm,
               total: winlossA.Total + winlossB.Total);
        }

        public static NetWinLoss ReadAgentNetWinLoss(IDataReader reader, UserLevel reportLevel)
            => reportLevel.IsNotSuperLevel() ?
            (reportLevel.IsMasterLevel() ?
            new NetWinLoss(reader.ToDecimal("AgTotal")) :
            new NetWinLoss(reader.ToDecimal("AgWinLoss"), reader.ToDecimal("AgComm"))) :
            null;

        public static NetWinLoss ReadMasterNetWinLoss(IDataReader reader, UserLevel reportLevel)
            => reportLevel.IsMasterOrSuperLevel() ?
            (reportLevel.IsMasterLevel() ?
            new NetWinLoss(reader.ToDecimal("MAgWinLoss"), reader.ToDecimal("MAgComm")) :
            new NetWinLoss(reader.ToDecimal("MAgTotal"))) :
            null;

        public static NetWinLoss ReadSuperNetWinLoss(IDataReader reader, UserLevel reportLevel)
            => reportLevel.IsSuperLevel() ?
            new NetWinLoss(reader.ToDecimal("SAgWinLoss"), reader.ToDecimal("SAgComm")) :
            null;
    }
}