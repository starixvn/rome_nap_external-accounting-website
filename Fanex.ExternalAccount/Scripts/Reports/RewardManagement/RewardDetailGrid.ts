class RewardDetailGrid extends BaseReportGrid {
    public static readonly DefaultColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", Width: 170, Sort: true, HeaderAlign: "center", ColumnName2: "Created Date" },
        { HeaderText: "Program", Name: "ProgramName", Width: 250, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Program" },
        { HeaderText: "Username", Name: "MemberUsername", Width: 160, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Member Username" },
        { HeaderText: "Amount", Name: "MemberAmount", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member Amount" },
        { HeaderText: "Reward Currency", Name: "MemberCurrency", Width: 150, Sort: true, HeaderAlign: "center", ColumnName2: "Member Reward Currency" },
        { HeaderText: "Payment Status", Name: "MemberPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Status" },
        { HeaderText: "Payment Date", Name: "MemberPaymentDate", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Date" },
        { HeaderText: "Feedback", Name: "MemberFeedBack", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Member Feedback" },

        { HeaderText: "Username", Name: "AgentUsername", Width: 160, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Agent Username" },
        { HeaderText: "Payment Status", Name: "AgentPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Status" },
        { HeaderText: "Payment Date", Name: "AgentPaymentDate", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Date" },

        { HeaderText: "Username", Name: "MasterUsername", Width: 160, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Master Username" },
        { HeaderText: "Payment Status", Name: "MasterPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Status" },
        { HeaderText: "Payment Date", Name: "MasterPaymentDate", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Date" },

        { HeaderText: "Username", Name: "SuperUsername", Width: 160, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Super Username" },
        { HeaderText: "Payment Status", Name: "SuperPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Status" },
        { HeaderText: "Payment Date", Name: "SuperPaymentDate", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Date" },
    ]
    public static readonly GroupHeaderColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", MergeRowNumber: 2 },
        { HeaderText: "Program", Name: "ProgramName", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberUsername", MergeColNumber: 6 },
        { HeaderText: "Agent", Name: "AgentUsername", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterUsername", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperUsername", MergeColNumber: 3 },
    ];

    constructor( options: IGridComponentConfig ) {
        super(options);
        this.gridOptions = {
            customizeHeader: true,
            columns: RewardDetailGrid.DefaultColumns,
            groupHeaderRows: [ { Columns: RewardDetailGrid.GroupHeaderColumns } ],
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: `<span id=${options.copyBtnId} title="Copy" class="icon-copy"></span><span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            },
            gridExpandHeight: 260,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 10,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: ( pageIndex, pageSize ) => {
                }
            },
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        }
        this.initSelectColumnsDropdown(RewardDetailGrid.DefaultColumns, [ 0, 1, 2 ]);
        this.initCopyBtn(options.copyBtnId);
    }

    buildBodyRows( reportData: any ): Array<any> {
        const bodyData = [];

        function StatusColor( status: boolean ) {
            return status ? "green" : "red";
        }

        if (reportData) {
            for (const element of reportData) {
                const {
                    CreatedDate, ProgramName,
                    MemberUsername, MemberAmount, MemberCurrency, MemberPaymentStatus, MemberPaymentStatusMapped, MemberPaymentDateString, MemberFeedBackMapped,
                    AgentUsername, AgentPaymentStatus, AgentPaymentStatusMapped, AgentPaymentDateString,
                    MasterUsername, MasterPaymentStatus, MasterPaymentStatusMapped, MasterPaymentDateString,
                    SuperUsername, SuperPaymentStatus, SuperPaymentStatusMapped, SuperPaymentDateString
                } = element
                bodyData.push({
                    Columns: [
                        { Name: "CreatedDate", Value: this.getDateTime(CreatedDate), CellAlign: "center" },
                        { Name: "ProgramName", Value: ProgramName },
                        { Name: "MemberUsername", Value: MemberUsername },
                        { Name: "MemberAmount", Value: MemberAmount },
                        { Name: "MemberCurrency", Value: MemberCurrency, CellAlign: "center" },
                        { Name: "MemberPaymentStatus", Value: MemberPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MemberPaymentStatus) },
                        { Name: "MemberPaymentDate", Value: MemberPaymentDateString, CellAlign: "center" },
                        { Name: "MemberFeedBack", Value: MemberFeedBackMapped, CellAlign: "center", CssClass: MemberFeedBackMapped === "Not Received" ? "red" : "" },
                        { Name: "AgentUsername", Value: AgentUsername },
                        { Name: "AgentPaymentStatus", Value: AgentPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(AgentPaymentStatus) },
                        { Name: "AgentPaymentDate", Value: AgentPaymentDateString, CellAlign: "center" },
                        { Name: "MasterUsername", Value: MasterUsername },
                        { Name: "MasterPaymentStatus", Value: MasterPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MasterPaymentStatus) },
                        { Name: "MasterPaymentDate", Value: MasterPaymentDateString, CellAlign: "center" },
                        { Name: "SuperUsername", Value: SuperUsername },
                        { Name: "SuperPaymentStatus", Value: SuperPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(SuperPaymentStatus) },
                        { Name: "SuperPaymentDate", Value: SuperPaymentDateString, CellAlign: "center" }
                    ]
                });
            }
        }

        this.gridBodyRowsCache = bodyData;
        return bodyData;
    }

    buildFooterRows( reportData: any ): Array<any> {
        return undefined;
    }

    getDefaultColumns(): Array<any> {
        return RewardDetailGrid.DefaultColumns;
    }

    getGridOptions(): any {
        return this.gridOptions
    }

    getGroupHeaderColumns(): Array<any> {
        return RewardDetailGrid.GroupHeaderColumns;
    }

    getDateTime( date: Date ): string {
        if (!date) return "";
        return moment(date).format("MM/DD/YYYY hh:mm:ss A");
    }
}