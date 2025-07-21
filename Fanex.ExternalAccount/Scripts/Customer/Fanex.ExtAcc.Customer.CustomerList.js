// tag for later search :D
// Fanex.ExtAcc.Customer.CustomerList.SingleEditSuper
// Fanex.ExtAcc.Customer.CustomerList.SingleEditMaster
// Fanex.ExtAcc.Customer.CustomerList.SingleEditAgent
// Fanex.ExtAcc.Customer.CustomerList.SingleEditMember
Fanex.ExtAcc.Customer.CustomerList = {
    DOMElements: {},

    cachedParams: {
        breadcrumbs: {
            SuperId: 0,
            MasterId: 0,
            AgentId: 0
        },
        pageSize: 50,
        pageIndex: 0,
        uplineId: 0,
        uplineRoleId: 0,
        previousParams: null,
        isDirectMember: 0,
        serviceType: 2
    },

    Init: function (uplineId, uplineRoleId) {
        $(".icon-filter").click(Fanex.ExtAcc.Customer.showHideFilter);
        this.cachedParams.uplineId = uplineId;
        this.cachedParams.uplineRoleId = uplineRoleId;
        this.DOMElements.txtUsername = $('#txtUserName');
        this.DOMElements.selStatus = $('#selStatus');
        this.DOMElements.selServiceType = $('#selServiceType');
        this.DOMElements.lblServiceType=$('label[for="selServiceType"]');
        this.DOMElements.btnSearch = $('#btnSearch');
        this.DOMElements.icoExportExcel = $('#icoExportExcel');
        this.DOMElements.customerListGrid = $('#customerListGrid');
        this.DOMElements.jqgrid = $('#jqgrid');
        this.DOMElements.msgNoinfo = $('#msgNoinfo');
        this.DOMElements.divSuperLimitDate = $('#chkShowSuperLimitDate').parent();
        this.DOMElements.divIsNoDownlines = $('#chkIsNoDownlines').parent();
        this.DOMElements.chkShowSuperLimitDate = $('#chkShowSuperLimitDate');
        this.DOMElements.chkIsNoDownlines = $('#chkIsNoDownlines');
        this.DOMElements.editMult = $('#editMult');
        this.DOMElements.breadcrumbs = $('#breadcrumbs');
        this.DOMElements.selectAllNone = $('#select-all-none');
        this.DOMElements.levelTypeSuperView = $("#ddl-level-type-container-super-view");
        this.DOMElements.levelTypeMasterView = $("#ddl-level-type-container-master-view");
        this.BindingEvents();
        if (isShowCurrency) {
            this.InitCurrencyList();
        } else {
            this.DOMElements.btnSearch.click();
        }
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member && !Fanex.ExtAcc.Customer.isEditDirectMember) {
            this.DOMElements.selServiceType.toggle(Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member);
            this.DOMElements.lblServiceType.toggle(Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member);
        }
    },

    InitCurrencyList: function () {
        var self = this;
        var groupName = $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html();
        $.post(resolveClientUrl('Filter/CurrencyList'), { groupName: groupName }, function (result) {
            var dataList = [];
            $.each(result, function (i, currency) {
                dataList.push({
                    id: "ctm_cur_" + currency.CurrencyId,
                    value: currency.CurrencyId,
                    text: currency.CurrencyName,
                    checked: true,
                    enable: true
                });
            });

            self.DOMElements.$multiSelectCurrency = $("#ddl-currency").fanexMultiSelect({
                minWidth: 140,
                checkAllText: "Select all",
                noneSelectedText: "Select Currencies",
                useCustomCheckAllText: true,
                customCheckAllText: "All Currencies",
                isCheckAll: true,
                listData: dataList
            });

            self.DOMElements.btnSearch.click();
        });
    },

    BindingEvents: function () {
        this.DOMElements.btnSearch.off().click(function () {
            var _this = Fanex.ExtAcc.Customer.CustomerList;
            _this.SubmitSearch(_this.cachedParams.uplineId, _this.cachedParams.uplineRoleId, undefined, 0);
        });

        this.DOMElements.txtUsername.keypress(function (e) {
            var _this = Fanex.ExtAcc.Customer.CustomerList;
            var code = e.keyCode ? e.keyCode : e.which;
            if (code === 13) {
                _this.DOMElements.btnSearch.click()
            }
        });
    },

    SubmitSearch: function (uplineId, uplineRoleId, itemsPerPage, currentIndex) {
        var _this = Fanex.ExtAcc.Customer.CustomerList;
        var pageSize = !isNullOrUndefined(itemsPerPage) ? itemsPerPage : _this.cachedParams.pageSize;
        var pageIndex = !isNullOrUndefined(currentIndex) ? currentIndex : _this.cachedParams.pageIndex;
        var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        var isMember = uplineRoleId == 2 ? true : false;
        var isDirectMember = (uplineRoleId === 3 && $("#ddl-level-type-master-view").find(":selected").val() === "1")
            || (uplineRoleId === 4 && $("#ddl-level-type-super-view").find(":selected").val() === "1");
        var serviceType = _this.DOMElements.selServiceType.val();
        _this.cachedParams.pageIndex = pageIndex;
        _this.cachedParams.pageSize = pageSize;

        Fanex.ExtAcc.BlockUI();
        Fanex.ExtAcc.Customer.CurrentLevel = isDirectMember ? Fanex.ExtAcc.Customer.Member : isSuperList ? Fanex.ExtAcc.Customer.Super : uplineRoleId - 1;
        this.DOMElements.selServiceType.toggle(!isDirectMember && Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member);
        this.DOMElements.lblServiceType.toggle(!isDirectMember && Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member);
        this.DOMElements.divSuperLimitDate.toggle(isSuperList);
        this.DOMElements.divIsNoDownlines.toggle(!isMember);
        Fanex.ExtAcc.Customer.superId = uplineRoleId === Fanex.ExtAcc.Customer.Super ? uplineId : Fanex.ExtAcc.Customer.superId;
        Fanex.ExtAcc.Customer.masterId = uplineRoleId === Fanex.ExtAcc.Customer.Master ? uplineId : Fanex.ExtAcc.Customer.masterId;
        Fanex.ExtAcc.Customer.agentId = uplineRoleId === Fanex.ExtAcc.Customer.Agent ? uplineId : Fanex.ExtAcc.Customer.agentId;
        var params = {
            groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            groupId: $('#ddlGroupName').val(),
            userName: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtUsername.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            status: this.DOMElements.selStatus.val(),
            uplineRoleId: uplineRoleId,
            isShowSuperLimitDate: this.DOMElements.chkShowSuperLimitDate.is(":checked"),
            IsNoDownlines: this.DOMElements.chkIsNoDownlines.is(":checked"),
            uplineId: uplineId,
            isDirectMember: isDirectMember,
            serviceType: serviceType,
            pageSize: pageSize,
            pageIndex: pageIndex + 1
        };

        if (isShowCurrency) {
            params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
        }

        _this.cachedParams.isDirectMember = isDirectMember;
        _this.cachedParams.serviceType = serviceType;
        Fanex.ExtAcc.Customer.isEditDirectMember = isDirectMember;
        _this.cachedParams.previousParams = params;
        _this.Search(params);
    },

    ReloadAfterEdit: function () {
        var _this = Fanex.ExtAcc.Customer.CustomerList;
        _this.Search(_this.cachedParams.previousParams);
    },

    Search: function (params) {
        $.post(resolveClientUrl('CustomerList/Seach'), params, function (result) {
            var _this = Fanex.ExtAcc.Customer.CustomerList;
            var isSuperList = isNullOrUndefinedOrEmpty(params.uplineRoleId);
            var hasSubGroups = result.HasSubGroups;
            _this.cachedParams.uplineId = params.uplineId;
            _this.cachedParams.uplineRoleId = params.uplineRoleId;
            var numberOfFrozenColumn = 6;
            var widthDevice = top.$(top.window).width();
            if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member
                && !Fanex.ExtAcc.Customer.isEditDirectMember
                && _this.DOMElements.selServiceType.val() == '2') {
                _this.DOMElements.selectAllNone.hide();
            }
            else {
                _this.DOMElements.selectAllNone.show();
            }
            
            if (widthDevice <= 768) {
                numberOfFrozenColumn = 3;
            }
            var bodyRows = [];
            var rowLength = result.CustomerList.length;
            for (var i = 0; i < rowLength; i++) {
                var currentRow = result.CustomerList[i];
                var dateLimit = !isNullOrUndefinedOrEmpty(currentRow.MinDate)
                    ? currentRow.MinDate
                    : currentRow.LimitDays !== "" ? currentRow.LimitDays
                        : "";
                var columns = _this.GetGridBodyRows(currentRow, i, params.pageSize, params.pageIndex - 1, params.uplineRoleId, result.HasSubGroups, params.isDirectMember);
                bodyRows.push({
                    Columns: columns,
                    CssClass: Fanex.ExtAcc.Customer.CustomerList.GetCSSRow(currentRow)
                });
            }
            
            gridOptions = {
                groupHeaderRows: _this.GetGridGroupHeader(params.uplineRoleId, result.HasSubGroups, params.isDirectMember),
                columns: _this.GetGridHeader(params.uplineRoleId, result.HasSubGroups, params.isDirectMember),
                bodyRows: bodyRows,
                autoCalculateGridExpandHeight: true,
                numberOfFrozenColumn: numberOfFrozenColumn,
                bodyRowHeight: isMobile.phone ? 28 : 25,
                fullHeight: !isMobile.phone,
                gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
                customizeHeader: true,
                highlightFrozenWidth: 2,
                showPager: true,
                hiddenColumns: (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member
                    && !Fanex.ExtAcc.Customer.isEditDirectMember
                    && _this.DOMElements.selServiceType.val() == '2') ? [1] : [],
                enableHiddenColumns: false,
                customPager: true,
                showNumberColumn: true,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: result.PageIndex - 1,
                    totalItem: result.TotalRows,
                    itemsPerPage: params.pageSize,
                    pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                        Fanex.ExtAcc.Customer.DisableMultipleContextMenu();
                        _this.cachedParams.pageIndex = currentPageIndex;
                        _this.cachedParams.pageSize = itemsPerPage;
                        _this.SubmitSearch(params.uplineId, params.uplineRoleId);
                    }
                },
                marginRight: 12,
                showDistinctRow: false,
                allowTruncate: true
            };

            if (bodyRows.length === 0) {
                _this.DOMElements.msgNoinfo.css('display', 'block');
                _this.DOMElements.jqgrid.css('display', 'none');
                _this.DOMElements.icoExportExcel.css('display', 'none');
            } else {
                _this.DOMElements.msgNoinfo.css('display', 'none');
                _this.DOMElements.jqgrid.css('display', 'block');
                _this.DOMElements.icoExportExcel.show();
                setPagerOptionOnMobile(gridOptions);
                _this.DOMElements.jqgrid.fanexGrid(gridOptions);

                workaroundForGridOnPhone();
            }

            _this.DOMElements.editMult.attr('style', 'cursor: default !important');
            _this.DOMElements.editMult.attr('onclick', '');
            _this.DOMElements.editMult.contextMenu(false);
            _this.RenderBreadScrumbs(params.uplineId, params.uplineRoleId);
            _this.ShowHideLevelTypeFilter(params.uplineRoleId);
            _this.DOMElements.icoExportExcel.off();
            $('#editMult').contextMenu(false);
            if (bodyRows.length > 0) {
                _this.DOMElements.icoExportExcel.click(function () {
                    var params = {
                        groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
                        groupId: $('#ddlGroupName').val(),
                        userName: _this.DOMElements.txtUsername.val(),
                        status: _this.DOMElements.selStatus.val(),
                        uplineRoleId: _this.cachedParams.uplineRoleId,
                        isShowSuperLimitDate: _this.DOMElements.chkShowSuperLimitDate.is(":checked"),
                        IsNoDownlines: _this.DOMElements.chkIsNoDownlines.is(":checked"),
                        uplineId: _this.cachedParams.uplineId,
                        isDirectMember: _this.cachedParams.isDirectMember
                    };

                    if (isShowCurrency) {
                        params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
                    }

                    top.exportExcel(resolveClientUrl("CustomerList/GetExportToExcel?") + top.serialize(params), document);
                });

                var firstCustId = result.CustomerList[0].CustId;
                Fanex.ExtAcc.Customer.AllowDoubleComm = result.CustomerList[0].UplineDoubleCommLosing;
                $.post(resolveClientUrl('CustomerList/GetCustomerStatus'), { custId: firstCustId, groupName: $("#selectedGroupName").html() }, function (custStatusResult) {
                    Fanex.ExtAcc.Customer.ProductGroups = custStatusResult.ProductGroups;
                    Fanex.ExtAcc.Customer.UplineProductSettings = Fanex.ExtAcc.Customer.GetProductSettings(custStatusResult, !isSuperList);
                });
            }
        }).always(function () { Fanex.ExtAcc.UnBlockUI(); });
    },

    GetCSSRow: function (currentRow) {
        if (!currentRow.Enabled) {
            return 'rowDisabled';
        }

        if (currentRow.Closed) {
            return Fanex.ExtAcc.Customer.GetClosedClassName(currentRow);
        }

        if (currentRow.Suspended) {
            return 'rowSuspended';
        }

        if (currentRow.LockedAccess.IsLocked) {
            return currentRow.LockedAccess.ClassName;
        }

        return '';
    },

    GetGridHeader: function (uplineRoleId, hasSubGroups, isDirectMember = false) {
        var columns = [];
        var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
        var showSubGroups = isSuperList && hasSubGroups;
        if (isDirectMember) {
            isSuperList = false;
            isMemberList = true;
            showSubGroups = isSuperList && hasSubGroups;
        }

        columns.push({ HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' });
        columns.push({
            HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
            CustomHeaderRender: function (cell) {
                return '<div id="editMult" class="editMult"></div>';
            },
            CustomCellRender: function (cell) {
                return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.CustId + '" data-username="' + cell.Value.UserName + '" class="menuInactive"></div>';
            }
        });
        columns.push({
            HeaderText: 'Username', Name: 'username', Width: 125,
            CustomCellRender: function (cell) {
                var usernameValue = cell.Value.UserName,
                    shortUsername = usernameValue,
                    tooltipUsername = "";
                if (usernameValue.length >= 15) {
                    shortUsername = usernameValue.substring(0, 15) + "...";
                    tooltipUsername = usernameValue;
                }
                var func = 'javascript:Fanex.ExtAcc.Customer.CustomerList.SubmitSearch(' + cell.Value.CustId + ', ' + (isNullOrUndefinedOrEmpty(uplineRoleId) ? Fanex.ExtAcc.Customer.Super : uplineRoleId - 1) + ', null, 0)';
                var unlockIcon = cell.Value.LockedAccess.IsLocked
                    ? '<span class="locked-icon" onclick="javascript:Fanex.ExtAcc.Customer.UnlockAccess(' + cell.Value.CustId + ',' + cell.Value.LockedAccess.Locked + ')" title="Click to unlock"></span>'
                    : "";

                var cashIcon = cell.Value.ServiceType == '1' ? '<span class="icon-cashservice" style="color: #045ace" />': '';
                return (uplineRoleId === Fanex.ExtAcc.Customer.Agent || isDirectMember) ?
                    '<span title="' + tooltipUsername + '">' + shortUsername + '</span>' + unlockIcon + cashIcon :
                    '<a id="usr-' + cell.Value.CustId + '" href="' + func + '" title="' + tooltipUsername + '">' + shortUsername + '</a>' + unlockIcon;
            }
        });
        if (isShowCurrency) { columns.push({ HeaderText: 'Currency', Name: 'currency', Width: 75 }); }
        columns.push({
            HeaderText: 'Edit', Name: 'Edit', Width: 30,
            CustomCellRender: function (cell) {
                return '<div onclick="javascript:Fanex.ExtAcc.Customer.CustomerList.SingleEdit' + Fanex.ExtAcc.Customer.RoleNameConst[cell.Value.RoleId] + '(' + cell.Value.CustId + ', \'' + cell.Value.UserName + '\')" id="edit-' + cell.Value.CustId + '" class="editInfo"></div>';
            }
        });
        columns.push({
            HeaderText: 'Status', Name: 'Status', Width: 75, CellAlign: 'center',
            CustomCellRender: function (cell) {
                return !cell.Value.Enabled ? 'Disabled' : cell.Value.Closed ? 'Closed' : cell.Value.Suspended ? 'Suspended' : 'Open';
            }
        });
        columns.push({
            HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
            CustomCellRender: function (cell) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.CustId + ', ' + (cell.Value.RoleId === Fanex.ExtAcc.Customer.Super) + ')" id="dbl-' + cell.Value.CustId + '" class="' + (cell.Value.DoubleCommLosing ? 'allowDblComm' : 'disallowDblComm') + '"></div>';
            }
        });
        columns.push(isMemberList ? { HeaderText: 'Nickname', Name: 'NickName', Width: 120 } : undefined);
        columns.push({ HeaderText: 'First Name', Name: 'firstname', Width: 120 });
        columns.push({ HeaderText: 'Last Name', Name: 'lastname', Width: 120 });

        //Member
        columns.push(isMemberList ? { HeaderText: 'User Group', Name: 'ugroup', CellAlign: 'center', Width: 50 } : undefined);
        columns.push(isMemberList ? { HeaderText: 'Group', Name: 'discount', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(isMemberList ? { HeaderText: '1X2', Name: 'discount1x2', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all types of 1x2 bettype" ' } : undefined);
        columns.push(isMemberList ? { HeaderText: 'Others', Name: 'discountcs', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all other bettypes" ' } : undefined);

        columns.push(isMemberList ? { HeaderText: 'Group', Name: 'playerdiscount', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(isMemberList ? { HeaderText: '1X2', Name: 'playerdiscount1x2', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all types of 1x2 bettype" ' } : undefined);
        columns.push(isMemberList ? { HeaderText: 'Others', Name: 'playerdiscountcs', CellAlign: 'center', Width: 60, CustomAttr: ' title="Applies to all other bettypes" ' } : undefined);

        //SMA
        columns.push(!isMemberList ? { HeaderText: 'Group A', Name: 'commision', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" ' } : undefined);
        columns.push(!isMemberList ? { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all types of 1x2 bettype" ' } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center', CustomAttr: ' title="Applies to all other bettypes" ' } : undefined);

        columns.push(showSubGroups ? { HeaderText: 'Assigned Sub Groups', Name: 'AssignedSubGroups', HeaderAlign: 'center', CellAlign: 'left', Width: 160 } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'No. of Downlines', Name: 'NoOfDownlines', CellAlign: 'center', Width: 60 } : undefined);
        columns.push(isSuperList ? { HeaderText: 'Data Available', Name: 'DateLimit', CellAlign: 'center', Width: 100 } : undefined);
        columns.push({ HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 160 });
        columns.push({
            HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
            CustomCellRender: function (cell) {
                return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
            }
        });
        return columns.filter(function (item) { return item !== undefined });
    },
    GetGridGroupHeader: function (uplineRoleId, hasSubGroups, isDirectMember = false) {
        var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
        var showSubGroups = isSuperList && hasSubGroups;
        var columns = [];
        var uplineDiscountLabel = 'Agent Commission';
        if (isDirectMember) {
            isSuperList = false;
            isMemberList = true;
            showSubGroups = isSuperList && hasSubGroups;
            uplineDiscountLabel = uplineRoleId === Fanex.ExtAcc.Customer.Super ? 'Super Commission' : 'Master Commission';
        }

        columns.push({ HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 });
        columns.push({ HeaderText: '', Name: 'Check', MergeRowNumber: 2 });
        columns.push({ HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 });
        if (isShowCurrency) { columns.push({ HeaderText: 'Currency', Name: 'currency', MergeRowNumber: 2 }); }
        columns.push({ HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 });
        columns.push({ HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 });
        columns.push({ HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 });
        columns.push(isMemberList ? { HeaderText: 'Nickname', Name: 'NickName', MergeRowNumber: 2 } : undefined);
        columns.push({ HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 });
        columns.push({ HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 });
        columns.push(!isMemberList ? { HeaderText: 'Commission', Name: 'commision', MergeColNumber: 6 } : undefined);
        columns.push(showSubGroups ? { HeaderText: 'Assigned Sub Groups', Name: 'AssignedSubGroups', MergeRowNumber: 2 } : undefined);
        columns.push(isMemberList ? { HeaderText: 'User Group', Name: 'ugroup', MergeRowNumber: 2 } : undefined);
        columns.push(isMemberList ? { HeaderText: uplineDiscountLabel, Name: 'discount', MergeColNumber: 3 } : undefined);
        columns.push(isMemberList ? { HeaderText: 'Player Commission', Name: 'playerdiscount', MergeColNumber: 3 } : undefined);
        columns.push(!isMemberList ? { HeaderText: 'No. of Downlines', Name: 'NoOfDownlines', MergeRowNumber: 2 } : undefined);
        columns.push(isSuperList ? { HeaderText: 'Data Available', Name: 'DateLimit', MergeRowNumber: 2 } : undefined);
        columns.push({ HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 });
        columns.push({ HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 });
        return [{ Columns: columns.filter(function (item) { return item !== undefined; }) }];
    },
    GetGridBodyRows: function (currentRow, currentIndex, pageSize, pageIndex, uplineRoleId, hasSubGroups, isDirectMember = false) {
        var columns = [];
        var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
        var isCashMember = currentRow.ServiceType == '1';
        var showSubGroups = isSuperList && hasSubGroups;
        if (isDirectMember) {
            isSuperList = false;
            isMemberList = true;
            showSubGroups = isSuperList && hasSubGroups;
        }
        columns.push({ Name: 'RowNumber', Value: currentIndex + pageSize * (pageIndex) });
        columns.push({ Name: 'Check', Value: currentRow });
        columns.push({ Name: 'username', Value: currentRow, TdClass: isCashMember ? 'has-cash' : ''});
        if (isShowCurrency) { columns.push({ Name: 'currency', Value: currentRow.Currency }); }
        columns.push({ Name: 'Edit', Value: currentRow });
        columns.push({ Name: 'Status', Value: currentRow });
        columns.push({ Name: 'DoubleCommLosing', Value: currentRow });
        columns.push(isMemberList ? { Name: 'NickName', Value: currentRow.NickName } : undefined);
        columns.push({ Name: 'firstname', Value: currentRow.FirstName });
        columns.push({ Name: 'lastname', Value: currentRow.LastName });
        columns.push(isMemberList ? { Name: 'ugroup', Value: currentRow.UserGroup } : undefined);
        columns.push(!isMemberList ? { Name: 'commision', Value: currentRow.GroupA } : undefined);
        columns.push(!isMemberList ? { Name: 'groupb', Value: currentRow.GroupB } : undefined);
        columns.push(!isMemberList ? { Name: 'groupc', Value: currentRow.GroupC } : undefined);
        columns.push(!isMemberList ? { Name: 'groupd', Value: currentRow.GroupD } : undefined);

        columns.push(!isMemberList ? { Name: 'discount1x2', Value: currentRow.Discount1x2 } : undefined);
        columns.push(!isMemberList ? { Name: 'discountcs', Value: currentRow.DiscountCS } : undefined);

        columns.push(isMemberList ? { Name: 'discount', Value: currentRow.Discount } : undefined);
        columns.push(isMemberList ? { Name: 'discount1x2', Value: currentRow.Discount1x2 } : undefined);
        columns.push(isMemberList ? { Name: 'discountcs', Value: currentRow.DiscountCS } : undefined);

        columns.push(isMemberList ? { Name: 'playerdiscount', Value: currentRow.PlayerDiscount } : undefined);
        columns.push(isMemberList ? { Name: 'playerdiscount1x2', Value: currentRow.PlayerDiscount1x2 } : undefined);
        columns.push(isMemberList ? { Name: 'playerdiscountcs', Value: currentRow.PlayerDiscountCS } : undefined);

        columns.push(showSubGroups ? { Name: 'AssignedSubGroups', Value: currentRow.SubGroup } : undefined);
        if (!isMemberList) {
            columns.push({ Name: 'NoOfDownlines', Value: currentRow.NoOfDownlines });
        }
        columns.push(isSuperList ? { Name: 'DateLimit', Value: currentRow.MinDate ? moment(currentRow.MinDate).format("MM/DD/YYYY") : currentRow.LimitDays !== "" ? currentRow.LimitDays : "" } : undefined);
        columns.push({ Name: 'createdate', Value: currentRow.CreateDate, FormatDateTime: 'MM/DD/YYYY hh:mm:ss A' });
        columns.push({ Name: 'lastloginip', Value: currentRow.LastLoginIP });
        return columns.filter(function (item) { return item !== undefined });
    },
    RenderBreadScrumbs: function (uplineId, uplineRoleId) {
        var _breadScrumbs = this.cachedParams.breadcrumbs;
        var _this = this;
        if (isNullOrUndefined(uplineRoleId)) {
            this.DOMElements.breadcrumbs.html('');
            return;
        } else if (uplineRoleId === Fanex.ExtAcc.Customer.Super) {
            _breadScrumbs.SuperId = uplineId;
        } else if (uplineRoleId === Fanex.ExtAcc.Customer.Master) {
            _breadScrumbs.MasterId = uplineId;
        }

        var linkSuper = '<a href="javascript:Fanex.ExtAcc.Customer.CustomerList.SubmitSearch(null, null, null, 0)">Super</a>';
        var linkMaster = '<a href="javascript:Fanex.ExtAcc.Customer.CustomerList.SubmitSearch(' + _breadScrumbs.SuperId + ', 4, null, 0)">Master</a>';
        var linkAgent = '<a href="javascript:Fanex.ExtAcc.Customer.CustomerList.SubmitSearch(' + _breadScrumbs.MasterId + ', 3, null, 0)">Agent</a>';
        if (uplineRoleId == Fanex.ExtAcc.Customer.Super)
            this.DOMElements.breadcrumbs.html('<div class="level-line"></div><div class="level-active">' + linkSuper + '</div>');
        else if (uplineRoleId == Fanex.ExtAcc.Customer.Master)
            this.DOMElements.breadcrumbs.html('<div class="level-line"></div><div style="float:left">' + linkSuper + '</div><div class="viewNext">&nbsp;</div><div class="level-active">' + linkMaster + '</div>');
        else if (uplineRoleId == Fanex.ExtAcc.Customer.Agent)
            this.DOMElements.breadcrumbs.html('<div class="level-line"></div><div style="float:left">' + linkSuper + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div class="level-active">' + linkAgent + '</div>');
    },
    ShowHideLevelTypeFilter: function (uplineRoleId) {
        if (uplineRoleId === Fanex.ExtAcc.Customer.Super) {
            this.DOMElements.levelTypeSuperView.removeClass('hidden');
            this.DOMElements.levelTypeMasterView.addClass('hidden');
        } else if (uplineRoleId === Fanex.ExtAcc.Customer.Master) {
            this.DOMElements.levelTypeSuperView.addClass('hidden');
            this.DOMElements.levelTypeMasterView.removeClass('hidden');
        } else {
            this.DOMElements.levelTypeSuperView.addClass('hidden');
            this.DOMElements.levelTypeMasterView.addClass('hidden');
        }
    }
}