var SuperWinLossDetailBySubGroup = /** @class */ (function () {
    function SuperWinLossDetailBySubGroup($, topWindow) {
        this.$ = $;
        this.topWindow = topWindow;
        this.productControls = new ProductControls($);
        this.dateTimeControls = new DateTimeControls($);
    }
    SuperWinLossDetailBySubGroup.prototype.Init = function () {
        var _this = this;
        this.cacheElements();
        this.bindEvents();
        this.productControls.Init();
        this.initSubGroupDropdown();
        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    SuperWinLossDetailBySubGroup.prototype.cacheElements = function () {
        this.iconFilter = this.$("#span-iconFilter");
        this.filterContainer = this.$("#filter-container");
        this.ddlSubGroup = this.$("#ddl-subGroup");
        this.btnExportExcel = this.$("#exporttoexcel");
        this.gridContainer = this.$("#mainGrid");
        this.grid = this.$("#jqgrid");
        this.msgNoInfo = this.$("#msgNoinfo");
        this.cacheFormData = {
            pageSize: 200,
            groupId: this.$("#hidden-groupId").val(),
            groupName: this.$("#hidden-groupName").val()
        };
    };
    SuperWinLossDetailBySubGroup.prototype.bindEvents = function () {
        var _this = this;
        this.iconFilter.click(function () { return _this.showHideFilter(); });
        this.$("#btn-submit").click(function () {
            _this.setCacheFormData();
            _this.getSuperWinLossDetailBySubGroupReport();
        });
        this.btnExportExcel.click(function () { return _this.exportExcel(); });
        Fanex.ExtAcc.OnResize = function () { return _this.resizeReportGrid; };
    };
    SuperWinLossDetailBySubGroup.prototype.showHideFilter = function () {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.filterContainer.addClass("hide");
        }
        else {
            this.iconFilter.addClass("active");
            this.filterContainer.removeClass("hide");
        }
        this.resizeReportGrid();
    };
    SuperWinLossDetailBySubGroup.prototype.resizeReportGrid = function () {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    SuperWinLossDetailBySubGroup.prototype.setCacheFormData = function () {
        var fromtoDateParams = this.dateTimeControls.GetFromDateToDate();
        this.cacheFormData.productIds = this.productControls.GetProductIds().join(",");
        this.cacheFormData.productDetails = this.productControls.GetResourceIds().join(",");
        var subGroup = this.ddlSubGroup.val();
        this.cacheFormData.subGroup = subGroup === "All" ? null : subGroup;
        this.cacheFormData.fromDate = fromtoDateParams.fromdate;
        this.cacheFormData.toDate = fromtoDateParams.todate;
        this.cacheFormData.pageIndex = 0;
    };
    SuperWinLossDetailBySubGroup.prototype.getSuperWinLossDetailBySubGroupReport = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        var urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/GetData?") + this.$.param(this.cacheFormData);
        $.getJSON(urlAndQueryString, function (data) {
            if (data.totalItems) {
                _this.renderSuperWinLossDetailBySubGroup(data);
            }
            else {
                _this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    };
    SuperWinLossDetailBySubGroup.prototype.renderSuperWinLossDetailBySubGroup = function (data) {
        var rewardHeader = this.getRewardHeaderText(data);
        this.gridOptions = this.gridOptions || this.initGridOptions(rewardHeader);
        setPagerOptionOnMobile(this.gridOptions);
        this.gridOptions.titleInfo.text = this.buildGridTitle();
        this.gridOptions.columns = this.initGridHeaderColumns(rewardHeader);
        this.gridOptions.bodyRows = this.buildBodyRows(data.superWinLossDetails);
        this.gridOptions.footerRows = [this.buildGrandTotalColumns(data.total)];
        this.gridOptions.pagerOption.currentPage = this.cacheFormData.pageIndex;
        this.gridOptions.pagerOption.itemsPerPage = this.cacheFormData.pageSize;
        this.gridOptions.pagerOption.totalItem = data.totalItems;
        this.gridContainer.removeClass("hide");
        this.btnExportExcel.removeClass("hide");
        this.msgNoInfo.removeAttr("style");
        this.grid.fanexGrid(this.gridOptions);
        this.addTitleToSubGroupUsernameColumn();
        workaroundForGridOnPhone();
    };
    SuperWinLossDetailBySubGroup.prototype.initGridOptions = function (rewardHeader) {
        var _this = this;
        return {
            showTitle: true,
            titleInfo: { text: "" },
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.initGridGroupHeaderColumns(rewardHeader) }],
            columns: this.initGridHeaderColumns(rewardHeader),
            numberOfFrozenColumn: this.topWindow.$(top.window).width() < 768 ? 1 : 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: 256,
            numOfPaddingForGridExpandHeight: 40,
            showDistinctRow: false,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 200],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                    _this.cacheFormData.pageIndex = pageIndex;
                    _this.cacheFormData.pageSize = pageSize;
                    _this.getSuperWinLossDetailBySubGroupReport();
                }
            },
            showFooter: true,
            showTree: true,
            marginRight: 10,
            bottomBorderWidth: 0,
            sortOption: {
                customGridSort: function (rows, sortField, sortDirection, customSortFunction, sortType) { return _this.getSortedBodyRows(rows, sortField, sortDirection, customSortFunction, sortType); },
                afterGridSorted: function (rows, sortField, sortDirection) {
                    _this.addTitleToSubGroupUsernameColumn();
                    return rows;
                }
            },
            allowTruncate: true
        };
    };
    SuperWinLossDetailBySubGroup.prototype.initGridGroupHeaderColumns = function (rewardHeader) {
        var initMergeRow = function (text, name) {
            return { HeaderText: text, Name: name, MergeRowNumber: 2 };
        };
        var initMergeColumn = function (text, name) {
            return { HeaderText: text, Name: name, MergeColNumber: 3 };
        };
        return [
            initMergeRow("Sub Group/Username", "colSubGroupUsername"),
            initMergeRow("Turnover", "colTurnover"),
            initMergeRow("Net Turnover", "colNetTurnover"),
            initMergeRow("GrossComm", "colGrossComm"),
            initMergeColumn("Member", "colMember"),
            initMergeColumn("Agent", "colAgent"),
            initMergeColumn("Master", "colMaster"),
            initMergeColumn("Super", "colSuper"),
            initMergeRow("Company", "colCompany"),
            initMergeRow(rewardHeader, "colRewardAmount")
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.initGridHeaderColumns = function (rewardHeader) {
        var initGridHeaderColumn = function (text, name) {
            return {
                HeaderText: text,
                Name: name,
                Sort: true,
                Width: 130,
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: "0,0.00",
                FormatNegative: "$"
            };
        };
        return [
            { HeaderText: "Sub Group/Username", Name: "colSubGroupUsername", Sort: true, SortDirection: "asc", SortType: "string", Width: 150, ShowCollapseExpand: true },
            initGridHeaderColumn("Turnover", "colTurnover"),
            initGridHeaderColumn("Net Turnover", "colNetTurnover"),
            initGridHeaderColumn("GrossComm", "colGrossComm"),
            initGridHeaderColumn("Win/Loss", "colMember"),
            initGridHeaderColumn("Comm", "colMemberComm"),
            initGridHeaderColumn("Total", "colMemberTotal"),
            initGridHeaderColumn("Win/Loss", "colAgent"),
            initGridHeaderColumn("Comm", "colAgentComm"),
            initGridHeaderColumn("Total", "colAgentTotal"),
            initGridHeaderColumn("Win/Loss", "colMaster"),
            initGridHeaderColumn("Comm", "colMasterComm"),
            initGridHeaderColumn("Total", "colMasterTotal"),
            initGridHeaderColumn("Win/Loss", "colSuper"),
            initGridHeaderColumn("Comm", "colSuperComm"),
            initGridHeaderColumn("Total", "colSuperTotal"),
            initGridHeaderColumn("Company", "colCompany"),
            initGridHeaderColumn("" + rewardHeader, "colRewardAmount")
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.buildGridTitle = function () {
        var title = ["Super Win Loss Detail By Sub Group"];
        if (this.cacheFormData.fromDate === this.cacheFormData.toDate) {
            title.push("on");
            title.push(this.cacheFormData.fromDate);
        }
        else {
            title.push("from");
            title.push(this.cacheFormData.fromDate);
            title.push("to");
            title.push(this.cacheFormData.toDate);
        }
        return title.join(" ");
    };
    SuperWinLossDetailBySubGroup.prototype.getRewardHeaderText = function (data) {
        if (data.superWinLossDetails && data.superWinLossDetails.length > 0) {
            return "Reward (" + data.superWinLossDetails[0].rewardCurrency + ")";
        }
        ;
        return "Reward";
    };
    SuperWinLossDetailBySubGroup.prototype.buildBodyRows = function (superWinLossDetailBySubGroups) {
        var _this = this;
        var bodyRows = [];
        this.$.each(superWinLossDetailBySubGroups, function (i, winlossSubGroup) {
            var subBobyRows = [];
            _this.$.each(winlossSubGroup.superWinLossDetails, function (j, winloss) {
                subBobyRows.push(_this.buildUsernameColumns(winloss));
            });
            bodyRows.push(_this.buildSubGroupColumns(winlossSubGroup, subBobyRows));
        });
        return bodyRows;
    };
    SuperWinLossDetailBySubGroup.prototype.buildSubGroupColumns = function (winlossSubGroup, subBobyRows) {
        return {
            Columns: this.buildGridColumns(winlossSubGroup.subGroup, winlossSubGroup),
            SubRows: subBobyRows,
            CssClass: "sub-row-class"
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildUsernameColumns = function (winloss) {
        return {
            Columns: this.buildGridColumns(winloss.username, winloss)
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildGrandTotalColumns = function (totalWinloss) {
        return {
            Columns: this.buildGridColumns("Grand Total", totalWinloss)
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildGridColumns = function (subGroupOrUsername, winloss) {
        return [
            { Name: "colSubGroupUsername", Value: subGroupOrUsername, CssClass: "cellSubGroupOrUsername" },
            { Name: "colTurnover", Value: winloss.turnover },
            { Name: "colNetTurnover", Value: winloss.netTurnover },
            { Name: "colGrossComm", Value: winloss.grossComm },
            { Name: "colMember", Value: winloss.memberWinLoss },
            { Name: "colMemberComm", Value: winloss.memberComm },
            { Name: "colMemberTotal", Value: winloss.memberTotal },
            { Name: "colAgent", Value: winloss.agentWinLoss },
            { Name: "colAgentComm", Value: winloss.agentComm },
            { Name: "colAgentTotal", Value: winloss.agentTotal },
            { Name: "colMaster", Value: winloss.masterWinLoss },
            { Name: "colMasterComm", Value: winloss.masterComm },
            { Name: "colMasterTotal", Value: winloss.masterTotal },
            { Name: "colSuper", Value: winloss.superWinLoss },
            { Name: "colSuperComm", Value: winloss.superComm },
            { Name: "colSuperTotal", Value: winloss.superTotal },
            { Name: "colCompany", Value: winloss.company },
            { Name: "colRewardAmount", Value: winloss.rewardAmount }
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.btnExportExcel.addClass("hide");
        this.msgNoInfo.css("display", "block");
    };
    SuperWinLossDetailBySubGroup.prototype.exportExcel = function () {
        var urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/ExportExcel?") + this.$.param(this.cacheFormData);
        this.topWindow.exportExcel(urlAndQueryString, document);
    };
    SuperWinLossDetailBySubGroup.prototype.initSubGroupDropdown = function () {
        var _this = this;
        this.ddlSubGroup.combobox();
        this.ddlSubGroup.next("input.ui-autocomplete-input")
            .val(this.ddlSubGroup.find("option:selected").text())
            .attr("readonly", "true")
            //.css("width", "150px")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    SuperWinLossDetailBySubGroup.prototype.getSortedBodyRows = function (rows, sortField, sortDirection, customSortFunction, sortType) {
        if (!sortField || !rows.length) {
            return rows;
        }
        var sortColIndex = this.getSortedColumnIndex(rows, sortField);
        var compareFunction;
        if (sortType === "string") {
            compareFunction = sortDirection === "asc"
                ? this.compareStringASC
                : this.compareStringDESC;
        }
        else {
            compareFunction = sortDirection === "asc"
                ? this.compareNumberASC
                : this.compareNumberDESC;
        }
        // sort by sub group
        rows.sort(function (row1, row2) {
            return compareFunction(row1.Columns[sortColIndex].Value, row2.Columns[sortColIndex].Value);
        });
        // sort by username
        this.$.each(rows, function (index, row) {
            row.SubRows.sort(function (subrow1, subrow2) {
                return compareFunction(subrow1.Columns[sortColIndex].Value, subrow2.Columns[sortColIndex].Value);
            });
        });
        return rows;
    };
    SuperWinLossDetailBySubGroup.prototype.getSortedColumnIndex = function (rows, sortField) {
        var firstRowColumns = rows[0].Columns;
        for (var i = 0, max = firstRowColumns.length; i < max; i++) {
            if (firstRowColumns[i].Name === sortField) {
                return i;
            }
        }
        return -1;
    };
    SuperWinLossDetailBySubGroup.prototype.compareStringASC = function (text1, text2) {
        var uppercaseText1 = text1.toUpperCase();
        var uppercaseText2 = text2.toUpperCase();
        if (uppercaseText1 === "UNASSIGNED SUPERS") {
            return 1;
        }
        if (uppercaseText2 === "UNASSIGNED SUPERS") {
            return -1;
        }
        if (uppercaseText1 < uppercaseText2) {
            return -1;
        }
        if (uppercaseText1 > uppercaseText2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareStringDESC = function (text1, text2) {
        var uppercaseText1 = text1.toUpperCase();
        var uppercaseText2 = text2.toUpperCase();
        if (uppercaseText1 === "UNASSIGNED SUPERS") {
            return 1;
        }
        if (uppercaseText2 === "UNASSIGNED SUPERS") {
            return -1;
        }
        if (uppercaseText1 > uppercaseText2) {
            return -1;
        }
        if (uppercaseText1 < uppercaseText2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareNumberASC = function (number1, number2) {
        if (number1 < number2) {
            return -1;
        }
        if (number1 > number2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareNumberDESC = function (number1, number2) {
        if (number1 > number2) {
            return -1;
        }
        if (number1 < number2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.addTitleToSubGroupUsernameColumn = function () {
        $(".cellSubGroupOrUsername").each(function () {
            var $this = $(this);
            $this.attr("title", $this.text());
        });
    };
    return SuperWinLossDetailBySubGroup;
}());
$(function () {
    new SuperWinLossDetailBySubGroup($, window.top).Init();
});
//# sourceMappingURL=Fanex.ExtAcc.SuperWinLossDetailBySubGroup.js.map