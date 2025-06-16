declare var resolveClientUrl: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;

class SuperWinLossDetailBySubGroup {
    private $: JQueryStatic;
    private topWindow: any;
    private productControls: ProductControls;
    private dateTimeControls: DateTimeControls;
    private iconFilter: JQuery;
    private filterContainer: JQuery;
    private ddlSubGroup: JQuery;
    private btnExportExcel: JQuery;
    private gridContainer: JQuery;
    private grid: JQuery;
    private msgNoInfo: JQuery;
    private gridOptions: any;
    private cacheFormData: any;

    constructor($: JQueryStatic, topWindow: any) {
        this.$ = $;
        this.topWindow = topWindow;
        this.productControls = new ProductControls($);
        this.dateTimeControls = new DateTimeControls($);
    }

    Init() {
        this.cacheElements();
        this.bindEvents();

        this.productControls.Init();
        this.initSubGroupDropdown();
        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));
    }

    private cacheElements() {
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
    }

    private bindEvents() {
        this.iconFilter.click(() => this.showHideFilter());
        this.$("#btn-submit").click(() => {
            this.setCacheFormData();
            this.getSuperWinLossDetailBySubGroupReport();
        });
        this.btnExportExcel.click(() => this.exportExcel());
        Fanex.ExtAcc.OnResize = () => this.resizeReportGrid;
    }

    private showHideFilter() {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.filterContainer.addClass("hide");
        } else {
            this.iconFilter.addClass("active");
            this.filterContainer.removeClass("hide");
        }

        this.resizeReportGrid();
    }

    private resizeReportGrid() {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }

    private setCacheFormData() {
        const fromtoDateParams = this.dateTimeControls.GetFromDateToDate();

        this.cacheFormData.productIds = this.productControls.GetProductIds().join(",");
        this.cacheFormData.productDetails = this.productControls.GetResourceIds().join(",");

        const subGroup = this.ddlSubGroup.val();
        this.cacheFormData.subGroup = subGroup === "All" ? null : subGroup;

        this.cacheFormData.fromDate = fromtoDateParams.fromdate;
        this.cacheFormData.toDate = fromtoDateParams.todate;
        this.cacheFormData.pageIndex = 0;
    }

    private getSuperWinLossDetailBySubGroupReport() {
        Fanex.ExtAcc.BlockUI();
        const urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/GetData?") + this.$.param(this.cacheFormData)
        $.getJSON(urlAndQueryString, data => {
            if (data.totalItems) {
                this.renderSuperWinLossDetailBySubGroup(data);
            } else {
                this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    }

    private renderSuperWinLossDetailBySubGroup(data) {
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
    }

    private initGridOptions(rewardHeader) {
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
                pagerSelectCallBack: (pageIndex, pageSize) => {
                    this.cacheFormData.pageIndex = pageIndex;
                    this.cacheFormData.pageSize = pageSize;
                    this.getSuperWinLossDetailBySubGroupReport();
                }
            },
            showFooter: true,
            showTree: true,
            marginRight: 10,
            bottomBorderWidth: 0,
            sortOption: {
                customGridSort: (rows, sortField, sortDirection, customSortFunction, sortType) => this.getSortedBodyRows(rows, sortField, sortDirection, customSortFunction, sortType),
                afterGridSorted: (rows, sortField, sortDirection) => {
                    this.addTitleToSubGroupUsernameColumn();
                    return rows;
                }
            },
            allowTruncate: true
        };
    }

    private initGridGroupHeaderColumns(rewardHeader) {
        const initMergeRow = function (text, name) {
            return { HeaderText: text, Name: name, MergeRowNumber: 2 };
        };
        const initMergeColumn = function (text, name) {
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
    }

    private initGridHeaderColumns(rewardHeader) {
        const initGridHeaderColumn = function (text, name) {
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
            initGridHeaderColumn(`${rewardHeader}`, "colRewardAmount")
        ];
    }

    private buildGridTitle() {
        let title = ["Super Win Loss Detail By Sub Group"];
        if (this.cacheFormData.fromDate === this.cacheFormData.toDate) {
            title.push("on");
            title.push(this.cacheFormData.fromDate);
        } else {
            title.push("from");
            title.push(this.cacheFormData.fromDate);
            title.push("to");
            title.push(this.cacheFormData.toDate);
        }

        return title.join(" ");
    }

    private getRewardHeaderText(data) {
        if (data.superWinLossDetails && data.superWinLossDetails.length > 0) {
            return `Reward (${data.superWinLossDetails[0].rewardCurrency})`;
        };

        return "Reward";
    }

    private buildBodyRows(superWinLossDetailBySubGroups) {
        let bodyRows = [];
        this.$.each(superWinLossDetailBySubGroups, (i, winlossSubGroup) => {
            let subBobyRows = [];
            this.$.each(winlossSubGroup.superWinLossDetails, (j, winloss) => {
                subBobyRows.push(this.buildUsernameColumns(winloss));
            });

            bodyRows.push(this.buildSubGroupColumns(winlossSubGroup, subBobyRows));
        });

        return bodyRows;
    }

    private buildSubGroupColumns(winlossSubGroup, subBobyRows) {
        return {
            Columns: this.buildGridColumns(winlossSubGroup.subGroup, winlossSubGroup),
            SubRows: subBobyRows,
            CssClass: "sub-row-class"
        };
    }

    private buildUsernameColumns(winloss) {
        return {
            Columns: this.buildGridColumns(winloss.username, winloss)
        };
    }

    private buildGrandTotalColumns(totalWinloss) {
        return {
            Columns: this.buildGridColumns("Grand Total", totalWinloss)
        };
    }

    private buildGridColumns(subGroupOrUsername: string, winloss) {
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
    }

    private renderNoData() {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.btnExportExcel.addClass("hide");
        this.msgNoInfo.css("display", "block");
    }

    private exportExcel() {
        const urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/ExportExcel?") + this.$.param(this.cacheFormData);
        this.topWindow.exportExcel(urlAndQueryString, document);
    }

    private initSubGroupDropdown() {
        this.ddlSubGroup.combobox();
        this.ddlSubGroup.next("input.ui-autocomplete-input")
            .val(this.ddlSubGroup.find("option:selected").text())
            .attr("readonly", "true")
            //.css("width", "150px")
            .bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));
    }

    private getSortedBodyRows(rows, sortField: string, sortDirection: string, customSortFunction: Function, sortType: string) {
        if (!sortField || !rows.length) {
            return rows;
        }

        const sortColIndex = this.getSortedColumnIndex(rows, sortField);
        let compareFunction;

        if (sortType === "string") {
            compareFunction = sortDirection === "asc"
                ? this.compareStringASC
                : this.compareStringDESC;
        } else {
            compareFunction = sortDirection === "asc"
                ? this.compareNumberASC
                : this.compareNumberDESC
        }

        // sort by sub group
        rows.sort(function (row1, row2) {
            return compareFunction(row1.Columns[sortColIndex].Value, row2.Columns[sortColIndex].Value);
        });

        // sort by username
        this.$.each(rows, (index, row) => {
            row.SubRows.sort(function (subrow1, subrow2) {
                return compareFunction(subrow1.Columns[sortColIndex].Value, subrow2.Columns[sortColIndex].Value);
            });
        });

        return rows;
    }

    private getSortedColumnIndex(rows, sortField: string) {
        const firstRowColumns = rows[0].Columns;

        for (let i = 0, max = firstRowColumns.length; i < max; i++) {
            if (firstRowColumns[i].Name === sortField) {
                return i;
            }
        }

        return -1;
    }

    private compareStringASC(text1, text2) {
        const uppercaseText1 = text1.toUpperCase();
        const uppercaseText2 = text2.toUpperCase();

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
    }

    private compareStringDESC(text1, text2) {
        const uppercaseText1 = text1.toUpperCase();
        const uppercaseText2 = text2.toUpperCase();

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
    }

    private compareNumberASC(number1, number2) {
        if (number1 < number2) {
            return -1;
        }

        if (number1 > number2) {
            return 1;
        }

        return 0;
    }

    private compareNumberDESC(number1, number2) {
        if (number1 > number2) {
            return -1;
        }

        if (number1 < number2) {
            return 1;
        }

        return 0;
    }

    private addTitleToSubGroupUsernameColumn() {
        $(".cellSubGroupOrUsername").each(function () {
            const $this = $(this);
            $this.attr("title", $this.text());
        });
    }
}

$(function () {
    new SuperWinLossDetailBySubGroup($, window.top).Init();
});