Validators.validateInput = function (value) {
    if (value && value != "") {
        value = value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');

        if (value != "")
            return true;

        return false;
    }

    return false;
};

Fanex.ExtAcc.AssociationAccount = {
    DataFields: null,
    URL: '',

    $this: null,
    options: null,
    cachedParams: {
        userName: "",
        pageSize: 50,
        pageIndex: 0
    },

    Init: function () {
        $this = this;

        $this.DataFields = [
            { HeaderText: 'No.', Name: 'No', Width: 40, HeaderAlign: 'center', CellAlign: 'center' },
            { HeaderText: 'Username', Name: 'Username', HeaderAlign: 'center', Width: 200, CellAlign: 'left' },
            { HeaderText: 'Level', Name: 'Level', CellAlign: 'center', Width: 160 },
            {
                HeaderText: 'Status', Name: 'Status', Width: 100, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return !cell.Value.Enabled ? 'Disabled' : cell.Value.Closed ? 'Closed' : cell.Value.Suspended ? 'Suspended' : 'Open';
                }
            },
            { HeaderText: 'Association Date', Name: 'Date', CellAlign: 'center', Width: 200 }
        ];

        $('#Username').bind('keypress', function (e) {
            var code = e.keyCode ? e.keyCode : e.which;

            if (code === 13) {
                $this.NewSearch();
            }
        });

        $('#btnSearch').click(function (e) {
            $this.NewSearch();
        });
    },

    RenderGrid: function (params) {
        try {
            $.ajax({
                url: Fanex.ExtAcc.AssociationAccount.URL,
                dataType: 'json',
                type: 'post',
                data: params
            })
                .success(function (data) {
                    var rows = data.Accounts;
                    var bodyRows = [];
                    for (var i = 0; i < rows.length; i++) {
                        var index = (params.pageSize * params.pageIndex) + i + 1;
                        bodyRows.push({
                            Columns: [
                                { Name: "No", Value: index },
                                { Name: 'Username', Value: rows[i].Username },
                                { Name: 'Level', Value: rows[i].LevelName },
                                { Name: 'Status', Value: rows[i] },
                                { Name: 'Date', Value: rows[i].DateString }
                            ]
                        });
                    }
                    Fanex.ExtAcc.AssociationAccount.options = {
                        columns: Fanex.ExtAcc.AssociationAccount.DataFields,
                        bodyRows: bodyRows,
                        autoCalculateGridExpandHeight: true,
                        marginRight: 10,
                        showDistinctRow: false,
                        allowTruncate: true,
                        showTitle: true,
                        titleInfo: {
                            text: "Number of associated accounts (This group/Total): " + data.AssociationAccountText
                        },
                        showPager: true,
                        customPager: true,
                        pagerOption: {
                            itemsPerPageArray: [50, 100, 500],
                            currentPage: data.PageIndex,
                            totalItem: data.TotalRows,
                            itemsPerPage: Fanex.ExtAcc.AssociationAccount.cachedParams.pageSize,
                            pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                                Fanex.ExtAcc.AssociationAccount.cachedParams.pageIndex = currentPageIndex;
                                Fanex.ExtAcc.AssociationAccount.cachedParams.pageSize = itemsPerPage;
                                Fanex.ExtAcc.AssociationAccount.Search();
                            }
                        }
                    };
                    $('#jqgrid-user').fanexGrid(Fanex.ExtAcc.AssociationAccount.options);
                    Fanex.ExtAcc.UnBlockUI();
                });
        }
        catch (e) {
            if (typeof console !== 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    NewSearch: function () {
        if (!Validators.validateInput($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'This field is required', 0, 0, 41, -10, 20);
            return false;
        }
        Fanex.ExtAcc.AssociationAccount.cachedParams.userName = $('#Username').val();
        Fanex.ExtAcc.AssociationAccount.cachedParams.pageIndex = 0;
        Fanex.ExtAcc.AssociationAccount.cachedParams.pageSize = 50;
        $this.Search();
    },

    Search: function () {
        Fanex.ExtAcc.BlockUI();
        var username = Fanex.ExtAcc.AssociationAccount.cachedParams.userName;
        var pageSize = Fanex.ExtAcc.AssociationAccount.cachedParams.pageSize;
        var pageIndex = Fanex.ExtAcc.AssociationAccount.cachedParams.pageIndex;

        var params = {
            CustomerName: username.encodeHtml(),
            pageSize: pageSize,
            pageIndex: pageIndex
        };

        Fanex.ExtAcc.AssociationAccount.URL = resolveClientUrl("AssociationAccount/Get");

        this.RenderGrid(params);
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.AssociationAccount.Init();
});