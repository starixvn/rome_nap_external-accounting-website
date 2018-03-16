Fanex.ExtAcc.Customer.ShowEditSuper = function (
    custId,
    title,
    hideGoldDeluxe,
    hideSportsbook2,
    hideCockfighting,
    hideColossusBets,
    hideCricket,
    productSettingsJson) {
    this.TransferCondition = false;
    this.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.CurrentLevel = this.Super;
    this.HideGoldDeluxe = hideGoldDeluxe;
    this.HideSportsbook2 = hideSportsbook2;
    this.HideCockfighting = hideCockfighting;
    this.HideCricket = hideCricket;
    var productSettings = JSON.parse(unescape(productSettingsJson));

    this.ResetFlags();

    var items = {
        sportbook: {
            name: "Sportsbook",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('SportsbookSetting/EditMultiplePT' + '/?roleId=4&groupName=' + $('#selectedGroupName').html() + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                    900, 700, false, false, 1);
                
            }
        },
        separator0: "-----",
        racing: {
            name: "Racing",
            className: "iconViewEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Super?id=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 1050, 620, false, null, 1);                               
            }
        },
        separator1: "-----",
        casino: {
            name: "Casino",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Super/EditSuper?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=0&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 1064, 700, false, null, 1);                
            }
        },
        liveCasino: {
            name: "Live Casino",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "LiveCasinoSetting/Setting/EditSetting?uplineId=0&uplineLevel=0&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=4&formType=2" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);                
            }
        }
    }

    function addEditProductSuperContextMenuItem(productId, productKey, productName) {
        items[productKey] = {
            name: productName,
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                    Fanex.ExtAcc.Customer.UserId,
                    5,
                    4,
                    productId,
                    productName);
            }
        }
    }

    addEditProductSuperContextMenuItem(5, 'virtualSport', 'Virtual Sports');

    if (showKenoSetting == 'true') {
        items.keno = {
            name: "Keno",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditSuper?custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Keno Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 880, 700, false, false, 1);                
            }
        }
    }

    if (!this.HideGoldDeluxe) {
        items.goldDeluxe = {
            name: "Gold Deluxe",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=0&uplineId=0&formType=2&level=4" + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
                
            }
        }
    }

    if (showColossusBets == 1 && !hideColossusBets) {
        items.colossusBets = {
            name: "Colossus Bets",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=4&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Colossus Bets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);                               
            }
        }
    }

    if (!this.HideSportsbook2) {
        addEditProductSuperContextMenuItem(16, 'sportsbook2', 'Sportsbook 2');
    }

    if (!this.HideCockfighting) {
        addEditProductSuperContextMenuItem(19, 'cockFighting', 'Cockfighting');
    }

    if (!this.HideCricket) {
        addEditProductSuperContextMenuItem(20, 'cricket', 'Cricket');
    }

    for (var i = 0, max = productSettings.length; i < max; i++) {
        var productSetting = productSettings[i];

        addEditProductSuperContextMenuItem(productSetting.productId, productSetting.productKey, productSetting.productName);
    }

    items.separator4 = "-----";

    items.editInfo = {
        name: "Edit Info",
        height: 25,
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                500,
                height,
                false);
        }
    };

    items.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                415,
                height,
                false,
                false);
        }
    };

    var self = this;
    items.securityCode = {
        name: "Security Code",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var params = {
                custId: Fanex.ExtAcc.Customer.UserId,
                roleId: 4,
                username: Fanex.ExtAcc.Customer.TitleEditInfo,
                groupName: Fanex.ExtAcc.Customer.GroupName().groupName
            };
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
            Fanex.ExtAcc.Form.ShowFrame(
                resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?' + $.param(params)),
                'Reset security code: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                650,
                height,
                false,
                false);
        }
    };

    items.transferCondition = {
        name: "Transfer Condition",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/EditTransferCondition/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                600,
                250,
                false,
                false,
                1);
        }
    };

    if (showBetSetting == '1') {
        items.betSetting = {
            name: "Edit Betting Limit",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.UserId + '/' + userId + '/' + $('#selectedGroupName').html()), 'Edit Bet Setting', 900, 660, false, null, 1, 'bettingLimitPopup');
            }
        };
    }

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editInfo',
        items: items,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.GetSuperList = function (custId, dataSource) {
    Fanex.ExtAcc.BlockUI();

    this.CurrentLevel = this.Super;
    this.AllowDoubleComm = true;

    var url = resolveClientUrl('Customer/SuperListPost');

    var data = this.GetParams();
    this.custId = custId == 'undefined' ? null : custId;
    data.custId = data.username == '' ? null : custId;

    if (custId == undefined) {
        this.directSuperId = null;
        this.directMasterId = null;
        this.directAgentId = null;
    }

    this.DataFields = [{
        HeaderText: 'No.',
        Name: 'RowNumber',
        Width: 30,
        CellAlign: 'center'
    }, {
        HeaderText: '',
        Name: 'Check',
        Width: 30,
        CellAlign: 'center',
        CustomHeaderRender: function (cell) {
            return '<div id="editMult" class="editMult"></div>';
        },
        CustomCellRender: function (cell) {
            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            var custId = cellValue.custid;
            var hideKeno = showKenoSetting == 'false' || cellValue.HideKeno == true;

            var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList("
                + custId + ","
                + cellValue.DoubleCommLosing + ",null,null,"
                + cellValue.synccasino + ","
                + cellValue.SyncLiveCasino + ","
                + hideKeno + ","
                + cellValue.HideVirtualSports + ","
                + cellValue.HideGoldDeluxe + ","
                + cellValue.HideColossusBet + ","
                + cellValue.HideMuayThai + ","
                + cellValue.HideCockFighting + ","
                + cellValue.HideCricket + ")";

            var len = cellValue.username.length;
            var title = len >= 15 ? ' title="' + cellValue.username + '"' : '';

            return '<div' + title + '><a id="usr-' + custId + '" href="' + func + '">' + cellValue.username + '</a></div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditSuper("
                + cellValue.custid + ",'"
                + escape(cellValue.username) + "',"
                + cellValue.HideGoldDeluxe + ","
                + cellValue.HideMuayThai + ","
                + cellValue.HideCockFighting + ","
                + cellValue.HideColossusBet + ","
                + cellValue.HideCricket + ",'"
                + escape(JSON.stringify(cellValue.productSettings)) + "')";

            return '<div onclick="' + func + '" id="edit-' + cellValue.custid + '" class="editInfo"></div>';
        }
    }, {
        HeaderText: 'Status',
        Name: 'Status',
        Width: 75,
        CellAlign: 'center',
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            return !cellValue.enabled ? 'Disabled' : cellValue.closed ? 'Closed' : cellValue.suspended ? 'Suspended' : 'Open';
        }
    }, {
        HeaderText: 'Double Comm',
        Name: 'DoubleCommLosing',
        Width: 80,
        CustomCellRender: function (cell) {
            var custId = cell.Value.custid;
            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ', true)" id="dbl-' + custId + '" class="allowDblComm"></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ', true)" id="dbl-' + custId + '" class="disallowDblComm"></div>';
        }
    }, {
        HeaderText: 'First Name',
        Name: 'firstname',
        Width: 120
    }, {
        HeaderText: 'Last Name',
        Name: 'lastname',
        Width: 120
    }, {
        HeaderText: 'Group A',
        Name: 'groupa',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group B',
        Name: 'groupb',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group C',
        Name: 'groupc',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group D',
        Name: 'groupd',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: '1X2',
        Name: 'discount1x2',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of 1x2 bettype" '
    }, {
        HeaderText: 'Others',
        Name: 'discountcs',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all other bettypes" '
    }, {
        HeaderText: 'Game',
        Name: 'discountnumber',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Number Game" '
    }, {
        HeaderText: '% Odds',
        Name: 'discountmo',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Myanmar Odds" '
    }, {
        HeaderText: 'Created Date',
        Name: 'createdate',
        CellAlign: 'center',
        Width: 160,
        FormatDateTime: 'MM/DD/YYYY hh:mm:ss A'
    }, {
        HeaderText: 'Login IP',
        Name: 'lastloginip',
        CellAlign: 'center',
        Width: 100,
        CustomCellRender: function (cell) {
            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
        }
    }];

    if (dataSource) {
        this.RenderSuperList(dataSource);
    } else {
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            Fanex.ExtAcc.Customer.RenderSuperList(result);
        });
    }
};

Fanex.ExtAcc.Customer.RenderSuperList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];

    this.DataSource = rows;

    for (var i = 0; i < rows.length; i++) {
        var currentRow = rows[i];
        var cssRow = !currentRow.enabled ? 'rowDisabled' : currentRow.closed ? Fanex.ExtAcc.Customer.GetClosedClassName(currentRow) : currentRow.suspended ? 'rowSuspended' : '';

        currentRow.productSettings = this.GetProductSettings(dataSource.customerProductStatus[currentRow.custid], false);

        bodyRows.push({
            Columns: [{
                Name: 'RowNumber',
                Value: currentRow.RowNumber
            }, {
                Name: 'Check',
                Value: currentRow
            }, {
                Name: 'username',
                Value: currentRow
            }, {
                Name: 'Edit',
                Value: currentRow
            }, {
                Name: 'Status',
                Value: currentRow
            }, {
                Name: 'DoubleCommLosing',
                Value: currentRow
            }, {
                Name: 'firstname',
                Value: currentRow.firstname
            }, {
                Name: 'lastname',
                Value: currentRow.lastname
            }, {
                Name: 'groupa',
                Value: currentRow.groupa
            }, {
                Name: 'groupb',
                Value: currentRow.groupb
            }, {
                Name: 'groupc',
                Value: currentRow.groupc
            }, {
                Name: 'groupd',
                Value: currentRow.groupd
            }, {
                Name: 'discount1x2',
                Value: currentRow.discount1x2
            }, {
                Name: 'discountcs',
                Value: currentRow.discountcs
            }, {
                Name: 'discountnumber',
                Value: currentRow.discountnumber
            }, {
                Name: 'discountmo',
                Value: currentRow.IsMyanmarOdds ? (currentRow.discountMyanmarOdds ? currentRow.discountMyanmarOdds : 0) : 'N/A'
            }, {
                Name: 'createdate',
                Value: currentRow.createdate
            }, {
                Name: 'lastloginip',
                Value: currentRow.lastloginip
            }],
            CssClass: cssRow
        });
    }

    var groupHeader = [{
        Columns: [{
            HeaderText: 'No.',
            Name: 'RowNumber',
            MergeRowNumber: 2
        }, {
            HeaderText: '',
            Name: 'Check',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Username',
            Name: 'username',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Edit',
            Name: 'Edit',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Status',
            Name: 'Status',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Double Comm',
            Name: 'DoubleCommLosing',
            MergeRowNumber: 2
        }, {
            HeaderText: 'First Name',
            Name: 'firstname',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Last Name',
            Name: 'lastname',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Commission',
            Name: 'groupa',
            MergeColNumber: 8
        }, {
            HeaderText: 'Created Date',
            Name: 'createdate',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Login IP',
            Name: 'lastloginip',
            MergeRowNumber: 2
        }]
    }]

    var currentPage = 0;
    currentPage = (rows.length <= Fanex.ExtAcc.Customer.ItemsPerPage || (parseInt(rows.length / Fanex.ExtAcc.Customer.ItemsPerPage) < Fanex.ExtAcc.Customer.CurrentPage)) ? 0 : Fanex.ExtAcc.Customer.CurrentPage;
    Fanex.ExtAcc.Customer.CurrentPage = currentPage;
    var numberOfFrozenColumn = 6;
    var widthDevice = top.$(top.window).width();
    if (widthDevice <= 768) {
        numberOfFrozenColumn = 3;
    }
    options = {
        columns: this.DataFields,
        bodyRows: bodyRows,
        autoCalculateGridExpandHeight: true,
        numberOfFrozenColumn: numberOfFrozenColumn,
        bodyRowHeight: isMobile.phone ? 28 : 25,
        fullHeight: !isMobile.phone,
        gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
        customizeHeader: true,
        groupHeaderRows: groupHeader,
        highlightFrozenWidth: 2,
        showPager: true,
        customPager: false,
        pagerOption: {
            itemsPerPageArray: [50, 100, 500],
            currentPage: Fanex.ExtAcc.Customer.CurrentPage,
            totalItem: rows.length,
            itemsPerPage: Fanex.ExtAcc.Customer.ItemsPerPage,
            afterPagedFunction: function (options) {
                Fanex.ExtAcc.Customer.CurrentPage = options.currentPage;
                Fanex.ExtAcc.Customer.ItemsPerPage = options.itemsPerPage;
            }
        },
        marginRight: 12,
        showDistinctRow: false,
        allowTruncate: true
    };

    this.Render(options);

    $('#customerListGrid').css('visibility', 'visible');

    $('#breadcrumbs').empty();

    this.IsLoading = false;
};