Fanex.ExtAcc.Customer.ShowEditMaster = function (
    custId,
    title,
    transferCondition,
    syncCasino,
    disableCasino,
    hideVirtualSport,
    disableVirtualSport,
    superSyncCasino,
    isHideKeno,
    disableKeno,
    isHideGoldDeluxe,
    disableGoldDeluxe,
    isHideColossusBets,
    disableColossusBets,
    isHideCricket,
    disableCricket,
    productSettingsJson) {
    this.TransferCondition = transferCondition;
    Fanex.ExtAcc.Customer.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.HideVirtualSports = hideVirtualSport;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
    this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;
    this.HideCricket = showCricket == 0 || isHideCricket;
    var productSettings = JSON.parse(unescape(productSettingsJson));

    this.ResetFlags();

    itemsMaster = {
        sportbook: {
            name: "Sportsbook",
            className: "iconEnableEdit",
            callback: function () {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=3&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        ptSportbook: {
            name: "PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        minPTSportbook: {
            name: "Min PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMinPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Min PT ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        betSetting: {
            name: "Bet Setting",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 560;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, height, false, false, 1);
            }
        },
        commission: {
            name: "Commission",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = false;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 871, 250, false, false, 1);
            }
        },
        separator1: "-----"
    };

    var has3rdParty = false;
    if (superSyncCasino) {
        has3rdParty = true;
        if (syncCasino) {
            if (disableCasino) {
                itemsMaster.casino = {
                    name: "Casino",
                    className: "iconDisableEdit",
                    callback: function () {
                        top.shouldCalculatePopupHeight = true;
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1150, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                    }
                };
            } else {
                itemsMaster.casino = {
                    name: "Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        top.shouldCalculatePopupHeight = true;
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1150, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                    }
                };
            }
        } else {
            itemsMaster.casino = {
                name: "Casino",
                className: "iconUnsyncEdit",
                callback: function () {
                    top.shouldCalculatePopupHeight = true;
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1150, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                }
            };
        }
    }

    var ret = this.CustomerInfo();

    function addEditProductMasterContextMenuItem(productId, productKey, productName, isDisabled, isSynced) {
        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

        itemsMaster[productKey] = {
            name: productName,
            className: !isSynced ? "iconUnsyncEdit" : (isDisabled == true ? "iconDisableEdit" : "iconEnableEdit"),
            callback: function () {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                    custId,
                    4,
                    3,
                    productId,
                    productName);
            }
        };
    }

    if (!hideVirtualSport) {
        has3rdParty = true;
        addEditProductMasterContextMenuItem(5, 'virtualSport', 'Virtual Sports', disableVirtualSport);
    }

    if (!isHideKeno) {
        has3rdParty = true;
        itemsMaster.keno = {
            name: "RNG Keno",
            className: disableKeno ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit RNG Keno Setting <font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
            }
        };
    }

    if (!this.HideGoldDeluxe) {
        has3rdParty = true;

        itemsMaster.goldDeluxe = {
            name: "Gold Deluxe",
            className: disableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&level=3" + "&uplineId=" + Fanex.ExtAcc.Customer.superId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, height, false, false, 1);
            }
        }
    }

    if (!this.HideColossusBet) {
        has3rdParty = true;

        itemsMaster.colossusBets = {
            name: "Colossus Bets",
            className: disableColossusBets == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                top.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&srecommend=" + Fanex.ExtAcc.Customer.superId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=3&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets Setting <font color="#6dcff6">' + title + '</font>', 900, height, false, false, 1);
            }
        }
    }
   
    if (!this.HideCricket) {
        has3rdParty = true;
        addEditProductMasterContextMenuItem(20, 'cricket', 'Cricket', disableCricket);
    }

    for (var i = 0, max = productSettings.length; i < max; i++) {
        has3rdParty = true;
        var productSetting = productSettings[i];

        addEditProductMasterContextMenuItem(
            productSetting.productId,
            productSetting.productKey,
            productSetting.productName,
            productSetting.isDisabled,
            productSetting.isSynced);
    }

    if (has3rdParty)
        itemsMaster.separator2 = "-----";
    itemsMaster.editInfo = {
        name: "Edit Info",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                500,
                    height,
                false);
        }
    };
    itemsMaster.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                415,
                height,
                false,
                false);
        }
    };

    if (this.hasSecurityCode) {
        var self = this;
        itemsMaster.securityCode = {
            name: "Security Code",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var params = {
                    custId: custId,
                    roleId: 3,
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
    }

    if (transferCondition == false) {
        itemsMaster.transferCondition = {
            name: "Transfer Condition",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('Customer/EditTransferCondition/?RoleId=3&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                    600,
                    300,
                    false,
                    false,
                    1);
            }
        };
    }

    if ($.contextMenu) $.contextMenu('destroy');

    try {
        $.contextMenu({
            selector: '.editMaster',
            items: itemsMaster,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    } catch (e) { }
};

Fanex.ExtAcc.Customer.GetMasterList = function (
    srecommend,
    allow,
    custId,
    dataSource,
    superSyncCasino,
    isHideKeno,
    isHideVirtualSport,
    isHideGoldDeluxe,
    isHideColossusBets,
    isHideCricket) {
    Fanex.ExtAcc.BlockUI();

    this.superId = srecommend;
    this.CurrentLevel = Fanex.ExtAcc.Customer.Master;
    this.AllowDoubleComm = allow;
    this.AllowDoubleCommMaster = allow;

    this.superSyncCasino = superSyncCasino;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideVirtualSports = isHideVirtualSport;
    this.HideGoldDeluxe = isHideGoldDeluxe;
    this.HideColossusBet = isHideColossusBets;
    this.HideCricket = isHideCricket;
    this.hasSecurityCode = false;

    var url = resolveClientUrl('Customer/MasterListPost');
    var data = this.GetParams();
    this.custId = custId == 'undefined' ? null : custId;
    data.custId = data.username == '' ? null : custId;

    Fanex.ExtAcc.Customer.ShowCustomerLimitDateFilter();

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
            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' +
                cell.Value.custid + '" class="menuInactive" data-unsyncedproducts="' + Fanex.ExtAcc.Customer.GetUnsyncedProductIds(cell) + '"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList("
                + cellValue.custid + ","
                + cellValue.DoubleCommLosing + ",null,null,"
                + cellValue.synccasino + ","
                + cellValue.HideKeno + ","
                + cellValue.HideVirtualSports + ","
                + cellValue.HideGoldDeluxe + ","
                + cellValue.HideColossusBet + ","
                + cellValue.HideCricket + ")";

            var len = cellValue.username.length;
            var title = len >= 15 ? ' title="' + cellValue.username + '"' : '';

            return '<div' + title + '><a href="' + func + '">' + cellValue.username + '</a></div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            var title = escape(cellValue.username);
            var hideKeno = showKenoSetting == 'false' || cellValue.HideKeno == true;
            var hideGoldDeluxe = showGoldDeluxeSetting == 0 || !cellValue.SyncGoldDeluxeUpline;
            var hideColossusBets = showColossusBets == 0 || cellValue.HideColossusBet || isHideColossusBets;
            var hideCricket = showCricket == 0 || cellValue.HideCricket || isHideCricket || !cellValue.IsIMBC;

            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMaster("
                + cellValue.custid + ",'"
                + title + "',"
                + cellValue.disableTransfer + ","
                + cellValue.synccasino + ","
                + cellValue.disablecasino + ","
                + cellValue.HideVirtualSports + ","
                + cellValue.DisableVirtualSports + ","
                + superSyncCasino + ","
                + hideKeno + ","
                + cellValue.DisableKeno + ","
                + hideGoldDeluxe + ","
                + cellValue.DisableGoldDeluxe + ","
                + hideColossusBets + ","
                + cellValue.DisableColossusBet + ","
                + hideCricket + ","
                + cellValue.DisableCricket + ",'"
                + escape(JSON.stringify(cellValue.productSettings)) + "')";

            return '<div onclick="' + func + '" id="edit-' + cellValue.custid + '" class="editMaster"></div>';
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
            var css = '';
            var custId = cell.Value.custid;

            if (!Fanex.ExtAcc.Customer.AllowDoubleComm) {
                css = 'style="cursor:default !important"';
            }

            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ')" id="dbl-' + custId + '" class="allowDblComm" ' + css + '></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ')" id="dbl-' + custId + '" class="disallowDblComm" ' + css + '></div>';
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
        Name: 'groupd',
        HeaderText: 'Group D',
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
        if (!(dataSource.records > 0 && eval(dataSource.rows)[0].IsMyanmarOdds)) {
            this.DataFields.splice(14, 1);
        }
        else {
            Fanex.ExtAcc.Customer.DataFields.splice(10, 1);
            Fanex.ExtAcc.Customer.DataFields.splice(9, 1);
            Fanex.ExtAcc.Customer.DataFields.splice(8, 1);
        }

        this.RenderMasterList(dataSource);
    } else {
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!(result.records > 0 && eval(result.rows)[0].IsMyanmarOdds)) {
                Fanex.ExtAcc.Customer.DataFields.splice(14, 1);
            }
            else {
                Fanex.ExtAcc.Customer.DataFields.splice(10, 1);
                Fanex.ExtAcc.Customer.DataFields.splice(9, 1);
                Fanex.ExtAcc.Customer.DataFields.splice(8, 1);
            }

            Fanex.ExtAcc.Customer.RenderMasterList(result);
        });
    }
};

Fanex.ExtAcc.Customer.RenderMasterList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
        this.hasSecurityCode = dataSource.hasSecurityCode;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];
    var rowsLength = rows.length;

    if (rowsLength > 0) {
        this.hasSecurityCode = rows[0].UplineHasSecurityCode || rows[0].HasSecurityCode;

        var firstMasterId = rows[0].custid;
        this.UplineProductSettings = this.GetProductSettings(dataSource.customerProductStatus[firstMasterId], true);
    } else {
        this.UplineProductSettings = [];
    }

    this.DataSource = rows;
    var isMyanmarOdds = rowsLength > 0 ? rows[0].IsMyanmarOdds : false;
    for (var i = 0; i < rowsLength; i++) {
        var currentRow = rows[i];
        var cssRow = !currentRow.enabled ? 'rowDisabled' : currentRow.closed ? Fanex.ExtAcc.Customer.GetClosedClassName(currentRow) : currentRow.suspended ? 'rowSuspended' : '';

        currentRow.productSettings = this.GetProductSettings(dataSource.customerProductStatus[currentRow.custid], true);

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
            MergeColNumber: (isMyanmarOdds ? 4 : 6)
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

    for (var i = 0; i < bodyRows.length; i++) {
        if (!isMyanmarOdds) {
            bodyRows[i].Columns.splice(14, 1);
        }
        else {
            bodyRows[i].Columns.splice(11, 1);
            bodyRows[i].Columns.splice(10, 1);
            bodyRows[i].Columns.splice(9, 1);
        }
    }

    var currentPage = 0;
    currentPage = (rowsLength <= Fanex.ExtAcc.Customer.ItemsPerPage || (parseInt(rowsLength / Fanex.ExtAcc.Customer.ItemsPerPage) < Fanex.ExtAcc.Customer.CurrentPage)) ? 0 : Fanex.ExtAcc.Customer.CurrentPage;
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
            totalItem: rowsLength,
            itemsPerPage: Fanex.ExtAcc.Customer.ItemsPerPage,
            afterPagedFunction: function (options) {
                Fanex.ExtAcc.Customer.CurrentPage = options.currentPage;
                Fanex.ExtAcc.Customer.ItemsPerPage = options.itemsPerPage;
                Fanex.ExtAcc.Customer.DisableMultipleContextMenu();
            }
        },
        marginRight: 12,
        showDistinctRow: false,
        allowTruncate: true
    };

    this.Render(options);

    $('#customerListGrid').css('visibility', 'visible');

    $('#breadcrumbs')
        .empty()
        .append('<div class="level-line"></div><div class="level-active"><a  href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a></div>')
        .show();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }
};