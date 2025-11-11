var ConfigureGrid = /** @class */ (function () {
    function ConfigureGrid() {
        this.DefaultGroupHeaderColumns = [
            { HeaderText: "KO Time (GMT-4)", Name: "KOTime", MergeRowNumber: 2 },
            { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
            { HeaderText: "AvgStake", Name: "AvgStake", MergeRowNumber: 2 },
            { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
            { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
            {
                HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>",
                Name: "NetTurnover",
                MergeRowNumber: 2
            },
            { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
            { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 3 },
            { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
            { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
            { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 3 },
            { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
        ];
        this.DefaultColumns = [
            { HeaderText: "KO Time (GMT-4)", Name: "KOTime", Width: 170, Sort: false, HeaderAlign: "center" },
            { HeaderText: "Event", Name: "Event", Width: 250, Sort: false, HeaderAlign: "center" },
            { HeaderText: "Avg Stake", Name: "AvgStake", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
            { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Company", Name: "Company", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
        ];
    }
    ConfigureGrid.prototype.GenerateFakeData = function (i) {
        var datetime = new Date(new Date().getTime() + Math.floor(Math.random() * 1000000));
        var formattedDate = moment(datetime).format('MMM DD, YYYY');
        var formattedTime = moment(datetime).format('hh:mm A');
        return {
            KoTime: formattedDate + " \n " + formattedTime,
            Event: "Match Win Loss Report " + i,
            AvgStake: Math.floor(Math.random() * 1000) + 1,
            BetCount: Math.floor(Math.random() * 1000) + 1,
            Turnover: Math.floor(Math.random() * 1000000) + 1,
            NetTurnover: Math.floor(Math.random() * 1000000) + 1,
            GrossComm: Math.floor(Math.random() * 1000000) + 1,
            MemberWinlost: Math.floor(Math.random() * 1000000) + 1,
            PlayerComm: Math.floor(Math.random() * 1000000) + 1,
            MemberTotal: Math.floor(Math.random() * 1000000) + 1,
            AgentWinlost: Math.floor(Math.random() * 1000000) + 1,
            AgentComm: Math.floor(Math.random() * 1000000) + 1,
            AgentTotal: Math.floor(Math.random() * 1000000) + 1,
            MasterWinlost: Math.floor(Math.random() * 1000000) + 1,
            MasterComm: Math.floor(Math.random() * 1000000) + 1,
            MasterTotal: Math.floor(Math.random() * 1000000) + 1,
            SuperWinlost: Math.floor(Math.random() * 1000000) + 1,
            SuperComm: Math.floor(Math.random() * 1000000) + 1,
            SuperTotal: Math.floor(Math.random() * 1000000) + 1,
            Company: "Company"
        };
    };
    ConfigureGrid.prototype.GetFakeSummaryReportData = function (count) {
        for (var i = 0; i < count; i++) {
            this.data.push(this.GenerateFakeData(i));
        }
        return this.data;
    };
    ConfigureGrid.prototype.ConvertDotNetJsonDateToJsDate = function (jsonDate) {
        var regex = /^\/Date\((\d+)\)\/$/;
        var match = regex.exec(jsonDate);
        var datetime = null;
        if (match !== null) {
            datetime = new Date(parseInt(match[1]));
        }
        var formattedDate = moment(datetime).format('MMM DD, YYYY');
        var formattedTime = moment(datetime).format('hh:mm A');
        return formattedDate + "\n" + formattedTime;
    };
    ConfigureGrid.prototype.BuildEventName = function (leagueName, homeTeam, awayTeam, isNeutral) {
        return isNeutral ? "<b>" + leagueName + "</b>\n" + homeTeam + "(N)\n" + awayTeam + "(N)" : "$<b>" + leagueName + "</b>\n" + homeTeam + "\n" + awayTeam;
    };
    ConfigureGrid.prototype.GetBodyRowsSummaryReport = function (reportData) {
        var bodyData = [];
        console.log("reportData", reportData);
        // const reportData = this.GetFakeSummaryReportData(count);
        for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
            var element = reportData_1[_i];
            var HomeName = element.HomeName, AwayName = element.AwayName, LeagueName = element.LeagueName, IsNeutral = element.IsNeutral;
            bodyData.push({
                Columns: [
                    { Name: "KOTime", Value: this.ConvertDotNetJsonDateToJsDate(element["KOTime"]) },
                    { Name: "Event", Value: this.BuildEventName(LeagueName, HomeName, AwayName, IsNeutral) },
                    { Name: "AvgStake", Value: element["StakeAvg"] },
                    { Name: "BetCount", Value: element["BetCount"] },
                    { Name: "Turnover", Value: element["Turnover"] },
                    { Name: "NetTurnover", Value: element["NetTurnover"] },
                    { Name: "GrossComm", Value: element["GrossComm"] },
                    { Name: "MemberWinLoss", Value: element["MemberWinLoss"] },
                    { Name: "MemberComm", Value: element["MemberComm"] },
                    { Name: "MemberTotal", Value: element["MemberTotal"] },
                    { Name: "AgentWinLoss", Value: element["AgentWinLoss"] },
                    { Name: "AgentComm", Value: element["AgentComm"] },
                    { Name: "AgentTotal", Value: element["AgentTotal"] },
                    { Name: "MasterWinLoss", Value: element["MasterWinLoss"] },
                    { Name: "MasterComm", Value: element["MasterComm"] },
                    { Name: "MasterTotal", Value: element["MasterTotal"] },
                    { Name: "SuperWinLoss", Value: element["SuperWinLoss"] },
                    { Name: "SuperComm", Value: element["SuperComm"] },
                    { Name: "SuperTotal", Value: element["SuperTotal"] },
                    { Name: "Company", Value: element["CompanyComm"] }
                ]
            });
        }
        console.log("bodyData", bodyData);
        return bodyData;
    };
    ConfigureGrid.prototype.GetFooterRowsSummaryReport = function () {
        var randomData = this.GenerateFakeData("");
        var footerRows = [
            {
                Columns: [
                    { Name: "KoTime", Value: "Total @BaseCurrency", MergeColNumber: 2 },
                    { Name: "AvgStake", Value: randomData["AvgStake"] },
                    { Name: "BetCount", Value: randomData["BetCount"] },
                    { Name: "Turnover", Value: randomData["NetTurnover"] },
                    { Name: "NetTurnover", Value: randomData["NetTurnover"] },
                    { Name: "GrossComm", Value: randomData["GrossComm"] },
                    { Name: "MemberWinlost", Value: randomData["MemberWinlost"] },
                    { Name: "PlayerComm", Value: randomData["PlayerComm"] },
                    { Name: "MemberTotal", Value: randomData["MemberTotal"] },
                    { Name: "AgentWinlost", Value: randomData["AgentWinlost"] },
                    { Name: "AgentComm", Value: randomData["AgentComm"] },
                    { Name: "AgentTotal", Value: randomData["AgentTotal"] },
                    { Name: "MasterWinlost", Value: randomData["MasterWinlost"] },
                    { Name: "MasterComm", Value: randomData["MasterComm"] },
                    { Name: "MasterTotal", Value: randomData["MasterTotal"] },
                    { Name: "SuperWinlost", Value: randomData["SuperWinlost"] },
                    { Name: "SuperComm", Value: randomData["SuperComm"] },
                    { Name: "SuperTotal", Value: randomData["SuperTotal"] },
                    { Name: "Company", Value: randomData["Company"] }
                ]
            },
            {
                Columns: [
                    { Name: "KoTime", Value: "System Parlay", MergeColNumber: 2 },
                    { Name: "AvgStake", Value: randomData["AvgStake"] },
                    { Name: "BetCount", Value: randomData["BetCount"] },
                    { Name: "Turnover", Value: randomData["NetTurnover"] },
                    { Name: "NetTurnover", Value: randomData["NetTurnover"] },
                    { Name: "GrossComm", Value: randomData["GrossComm"] },
                    { Name: "MemberWinlost", Value: randomData["MemberWinlost"] },
                    { Name: "PlayerComm", Value: randomData["PlayerComm"] },
                    { Name: "MemberTotal", Value: randomData["MemberTotal"] },
                    { Name: "AgentWinlost", Value: randomData["AgentWinlost"] },
                    { Name: "AgentComm", Value: randomData["AgentComm"] },
                    { Name: "AgentTotal", Value: randomData["AgentTotal"] },
                    { Name: "MasterWinlost", Value: randomData["MasterWinlost"] },
                    { Name: "MasterComm", Value: randomData["MasterComm"] },
                    { Name: "MasterTotal", Value: randomData["MasterTotal"] },
                    { Name: "SuperWinlost", Value: randomData["SuperWinlost"] },
                    { Name: "SuperComm", Value: randomData["SuperComm"] },
                    { Name: "SuperTotal", Value: randomData["SuperTotal"] },
                    { Name: "Company", Value: randomData["Company"] }
                ]
            },
            {
                Columns: [
                    { Name: "KoTime", Value: "Outright", MergeColNumber: 2 },
                    { Name: "AvgStake", Value: randomData["AvgStake"] },
                    { Name: "BetCount", Value: randomData["BetCount"] },
                    { Name: "Turnover", Value: randomData["NetTurnover"] },
                    { Name: "NetTurnover", Value: randomData["NetTurnover"] },
                    { Name: "GrossComm", Value: randomData["GrossComm"] },
                    { Name: "MemberWinlost", Value: randomData["MemberWinlost"] },
                    { Name: "PlayerComm", Value: randomData["PlayerComm"] },
                    { Name: "MemberTotal", Value: randomData["MemberTotal"] },
                    { Name: "AgentWinlost", Value: randomData["AgentWinlost"] },
                    { Name: "AgentComm", Value: randomData["AgentComm"] },
                    { Name: "AgentTotal", Value: randomData["AgentTotal"] },
                    { Name: "MasterWinlost", Value: randomData["MasterWinlost"] },
                    { Name: "MasterComm", Value: randomData["MasterComm"] },
                    { Name: "MasterTotal", Value: randomData["MasterTotal"] },
                    { Name: "SuperWinlost", Value: randomData["SuperWinlost"] },
                    { Name: "SuperComm", Value: randomData["SuperComm"] },
                    { Name: "SuperTotal", Value: randomData["SuperTotal"] },
                    { Name: "Company", Value: randomData["Company"] }
                ]
            },
            {
                Columns: [
                    { Name: "KOTime", Value: "Grand Total in @BaseCurrency", MergeColNumber: 2 },
                    { Name: "AvgStake", Value: randomData["AvgStake"] },
                    { Name: "BetCount", Value: randomData["BetCount"] },
                    { Name: "Turnover", Value: randomData["NetTurnover"] },
                    { Name: "NetTurnover", Value: randomData["NetTurnover"] },
                    { Name: "GrossComm", Value: randomData["GrossComm"] },
                    { Name: "MemberWinlost", Value: randomData["MemberWinlost"] },
                    { Name: "PlayerComm", Value: randomData["PlayerComm"] },
                    { Name: "MemberTotal", Value: randomData["MemberTotal"] },
                    { Name: "AgentWinlost", Value: randomData["AgentWinlost"] },
                    { Name: "AgentComm", Value: randomData["AgentComm"] },
                    { Name: "AgentTotal", Value: randomData["AgentTotal"] },
                    { Name: "MasterWinlost", Value: randomData["MasterWinlost"] },
                    { Name: "MasterComm", Value: randomData["MasterComm"] },
                    { Name: "MasterTotal", Value: randomData["MasterTotal"] },
                    { Name: "SuperWinlost", Value: randomData["SuperWinlost"] },
                    { Name: "SuperComm", Value: randomData["SuperComm"] },
                    { Name: "SuperTotal", Value: randomData["SuperTotal"] },
                    { Name: "Company", Value: randomData["Company"] }
                ]
            }
        ];
        return footerRows;
    };
    return ConfigureGrid;
}());
//# sourceMappingURL=ConfigureGrid.js.map