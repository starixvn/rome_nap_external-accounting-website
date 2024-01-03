if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.TotalBetMC = {
    hideFilter: false,
    filters: null,
    leagueName: "Please select",
    viewBy: "Headline",
    Init: function () {
        var $this = this;
        $(".icon-filter").click(this.showHideFilter);
        this.initLeaguesDropdown();
        $("#ddl-base-currency").combobox();
        $("#base-currency input").attr('readonly', 'true').val($("#ddl-base-currency")[0].options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });

        $("#ddl-view-by").combobox({
            selected: function () { Fanex.ExtAcc.TotalBetMC.viewBy = $("#ddl-view-by option:selected").text(); }
        });
        $("#view-by input").attr('readonly', 'true').val($("#ddl-view-by")[0].options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });

        this.initEventsDropdown([]);

        $("#Submit").click(function (e) { $this.getReport(); });
        $(".btn-refresh").click(function (e) { $this.refresh(); });
    },
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.hideFilter = !$this.hideFilter;
    },
    getEvents: function () {
        Fanex.ExtAcc.TotalBetMC.leagueName = $("#ddl-leagues").fanexSingleSelect("getSelectedTexts", true);
        var leagueId = $("#ddl-leagues").fanexSingleSelect("getItems", true);

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBetMC/GetEvents"),
            type: "POST",
            data: { leagueId: parseInt(leagueId) },
            dataType: "json",
            cache: false,
            success: function (result) {
                this.initEventsDropdown(result);
            },
            error: function () { },
            complete: function () { Fanex.ExtAcc.UnBlockUI(); }
        });
    },
    initLeaguesDropdown: function () {
        var leagues = JSON.parse($("#inp-leagues").val());
        var $ddl = $("#ddl-leagues");
        var items = [];
        leagues.forEach(function (e, i) {
            items.push({
                id: e.LeagueId,
                value: e.LeagueId,
                text: e.LeagueName,
                checked: false,
                enable: true,
                items: []
            })
        });

        var $this = this;
        $("#ddl-leagues").fanexSingleSelect({
            showFilter: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Leagues",
            useCustomCheckAllText: false,
            customCheckAllText: "All Leagues",
            isCheckAll: false,
            listData: items,
            multiple: false,
            onCheck: function () { $this.getEvents(); }
        });
    },
    initEventsDropdown: function (events) {
        var $ddl = $("#ddl-events");
        var items = [];
        events.forEach(function (e, i) {
            items.push({
                id: e.MatchId,
                value: e.MatchId,
                text: e.MatchName,
                checked: true,
                enable: true,
                items: []
            });
        });

        $("#ddl-events").fanexMultiSelect({
            showFilter: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Events",
            useCustomCheckAllText: true,
            customCheckAllText: "All Events",
            isCheckAll: true,
            listData: items,
            onCheck: function () { }
        });
    },
    getReport: function () {
        Fanex.ExtAcc.BlockUI();

        var viewMode = $("#ddl-view-by").val();
        var baseCurrency = $("#ddl-base-currency").val();

        var league = $("#ddl-leagues").fanexSingleSelect("getItems", true);
        var events = $("#ddl-events").fanexMultiSelect("getItems", true);
        var param = {
            viewMode: viewMode,
            league: league,
            leagueName: $("#ddl-leagues").fanexSingleSelect("getSelectedTexts", true),
            events: events,
            baseCurrency: baseCurrency
        };

        if (!Fanex.ExtAcc.TotalBetMC.validateParams(param)) {
            Fanex.ExtAcc.UnBlockUI();
            return;
        }

        Fanex.ExtAcc.TotalBetMC.filters = param;

        if (AutoRefresh.isInit) {
            AutoRefresh.stop();
        }

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBetMC/GetData"),
            type: "POST",
            data: param,
            dataType: "json",
            cache: false,
            success: function (result) {
                Fanex.ExtAcc.TotalBetMC.renderReport(result);

                if (AutoRefresh.isInit) {
                    AutoRefresh.resetTime();
                } else {
                    AutoRefresh.init(Fanex.ExtAcc.TotalBetMC.refresh);
                }
            },
            error: function () { },
            complete: function () { Fanex.ExtAcc.UnBlockUI(); }
        });
    },
    refresh: function (callback) {
        if ($('.tbl-total-bet').length === 0 && window.top.countDownTimeout !== null) {
            clearTimeout(window.top.countDownTimeout);
            return;
        }

        if (!Fanex.ExtAcc.TotalBetMC.filters) {
            return;
        }

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBetMC/GetData"),
            type: "POST",
            data: Fanex.ExtAcc.TotalBetMC.filters,
            dataType: "json",
            cache: false,
            success: function (result) {
                Fanex.ExtAcc.TotalBetMC.renderReport(result);

                if (callback) {
                    callback();
                }
            },
            error: function () { }
        });
    },
    renderReport: function (report) {
        var headers = [
            {
                Columns: [
                    { HeaderText: "Time", Name: "Time", HeaderAlign: "center" },
                    { HeaderText: "Event", Name: "Event" },
                    { HeaderText: "Full Time", Name: "FullTime_HDP", MergeColNumber: 2, ThClass: 'col-full-time' },
                    { HeaderText: "1st Half", Name: "_1stHalf_HDP", MergeColNumber: 2, ThClass: 'col-1st-half' }
                ]
            },
            {
                Columns: [
                    { HeaderText: Fanex.ExtAcc.TotalBetMC.filters.leagueName, Name: "Time", HeaderAlign: "left", MergeColNumber: 2, MergeRowNumber: 2, ThClass: 'row-league-title league-name' },
                    { HeaderText: "Handicap", Name: "FullTime_HDP", MergeRowNumber: 2, ThClass: 'row-league-title' },
                    { HeaderText: "Over/Under", Name: "FullTime_OU", MergeRowNumber: 2, ThClass: 'row-league-title' },
                    { HeaderText: "Handicap", Name: "_1stHalf_HDP", MergeRowNumber: 2, ThClass: 'row-league-title' },
                    { HeaderText: "Over/Under", Name: "_1stHalf_OU", MergeRowNumber: 2, ThClass: 'row-league-title' }
                ]
            },
        ];

        var arrColData = [
            { HeaderText: Fanex.ExtAcc.TotalBetMC.filters.leagueName, Name: "Time", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Event", Name: "Event", Width: 400 },
            { HeaderText: "Handicap", Name: "FullTime_HDP", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Over/Under", Name: "FullTime_OU", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Handicap", Name: "_1stHalf_HDP", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Over/Under", Name: "_1stHalf_OU", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" }];

        var bodyData = [];
        report.forEach(function (v, i) {
            var isLive = v.LiveIndicator === 2;
            var rowClass = isLive ? 'row-live-match' : '';
            var expandButton = isLive && v.IsLatest && v.TotalScore > 0
                ? ' <span class="tgg-score icon-plus" match-id="' + v.MatchId + '" collapsed="1"></span>'
                : '';
            var time = isLive
                ? '<span class="match-score">Live!</span> ' + v.LiveHomeScore + '-' + v.LiveAwayScore + expandButton
                : v.ShowTime;
            var event = v.HomeName + (v.IsNeutral ? ' (N)' : '') + " -vs- " + v.AwayName;

            bodyData.push({
                CssClass: isLive && !v.IsLatest ? 'hide' : '',
                CustomAttr: 'match-id="' + v.MatchId + '" latest="' + (v.IsLatest ? 1 : 0) + '"' + (isLive ? ' live="1"' : ''),
                Columns: [
                    { Name: "Time", TdClass: rowClass, CustomCellRender: function () { return time; } },
                    { Name: "Event", TdClass: rowClass, Value: event },
                    { Name: "FullTime_HDP", TdClass: rowClass, Value: v.Show_FullTime_HDP ? v.HDPFT : '' },
                    { Name: "FullTime_OU", TdClass: rowClass, Value: v.Show_FullTime_OU ? v.OUFT : '' },
                    { Name: "_1stHalf_HDP", TdClass: rowClass, Value: v.Show_1stHalf_HDP ? v.HDPHT : '' },
                    { Name: "_1stHalf_OU", TdClass: rowClass, Value: v.Show_1stHalf_OU ? v.OUHT : '' },
                ]
            });
        });

        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: headers,
            columns: arrColData,
            numberOfFrozenColumn: 0,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showFooter: false,
            gridExpandHeight: 0,
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            numOfPaddingForGridExpandHeight: 0,
            bodyRows: bodyData
        };

        $("#jqgrid").fanexGrid(gridOption);
        workaroundForGridOnPhone();

        $(".tgg-score").click(function () {
            var $this = $(this);
            var matchId = $this.attr('match-id');
            var isCollapse = $this.attr('collapsed');
            if (isCollapse === '1') {
                $this.attr('collapsed', '0');
                $("tr[match-id='" + matchId + "'][live='1'][latest='0']").removeClass('hide');
                $this.removeClass('icon-plus').addClass('icon-minus');
            } else {
                $this.attr('collapsed', '1');
                $("tr[match-id='" + matchId + "'][live='1'][latest='0']").addClass('hide');
                $this.removeClass('icon-minus').addClass('icon-plus');
            }

            $("#jqgrid").fanexGrid('resizeGrid');
        });
    },
    validateParams: function (param) {
        if (param.league === '' || param.league === '0' || param.league.length === 0) {
            Validators.show($("#ddl-leagues")[0], 'Required field!', undefined, undefined, undefined, undefined, undefined, 3000);

            return false;
        }

        if (param.events === '' || param.events === '0' || param.events.length === 0) {
            Validators.show($("#ddl-events")[0], 'Required field!', undefined, undefined, undefined, undefined, undefined, 3000);
            return false;
        }

        return true;
    }
};

$(document).ready(function () {
    if (window.top.countDownTimeout !== null) {
        clearTimeout(window.top.countDownTimeout);
    }

    Fanex.ExtAcc.TotalBetMC.Init();
});