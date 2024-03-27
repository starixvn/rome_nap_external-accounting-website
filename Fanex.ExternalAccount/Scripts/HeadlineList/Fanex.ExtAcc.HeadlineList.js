if (typeof Fanex === "undefined" || typeof Fanex.ExtAcc === "undefined") {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.HeadlineList = {
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

    },
    sortInfo: {
        sortType: "",
        sortClass: "",
        sortCol: ""
    },
    currentHeadlines: [],
    Init: function (uplineId, uplineRoleId) {
        // $(".icon-filter").click(Fanex.ExtAcc.Customer.showHideFilter);
        // this.cachedParams.uplineId = uplineId;
        // this.cachedParams.uplineRoleId = uplineRoleId;
        // this.DOMElements.txtUsername = $('#txtUserName');
        // this.DOMElements.selStatus = $('#selStatus');
        this.DOMElements.btnSearch = $('#btnSearch');
        this.DOMElements.icoExportExcel = $('#icoExportExcel');
        // this.DOMElements.customerListGrid = $('#customerListGrid');
        this.DOMElements.jqgrid = $('#jqgrid');
        this.DOMElements.msgNoinfo = $('#msgNoinfo');
        // this.DOMElements.divSuperLimitDate = $('#chkShowSuperLimitDate').parent();
        // this.DOMElements.divIsNoDownlines = $('#chkIsNoDownlines').parent();
        // this.DOMElements.chkShowSuperLimitDate = $('#chkShowSuperLimitDate');
        // this.DOMElements.chkIsNoDownlines = $('#chkIsNoDownlines');
        // this.DOMElements.editMult = $('#editMult');
        // this.DOMElements.breadcrumbs = $('#breadcrumbs');

        this.BindingEvents();
        this.DOMElements.btnSearch.click();

    },

    InitCurrencyList: function () {
        var self = this;
        var groupName = $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html();
        $.post(resolveClientUrl('Filter/CurrencyList'), {groupName: groupName}, function (result) {
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
            // var _this = Fanex.ExtAcc.HeadlineList;
            var _this = Fanex.ExtAcc.HeadlineList;
            _this.SubmitSearch(_this.uplineId, _this.uplineRoleId, undefined, 0);
        });

        // this.DOMElements.txtUsername.keypress(function (e) {
        //     var _this = Fanex.ExtAcc.Customer.CustomerList;
        //     var code = e.keyCode ? e.keyCode : e.which;
        //     if (code === 13) {
        //         _this.DOMElements.btnSearch.click()
        //     }
        // });
    },

    SubmitSearch: function (uplineId, uplineRoleId, itemsPerPage, currentIndex) {
        var _this = Fanex.ExtAcc.HeadlineList;
        var pageSize = !isNullOrUndefined(itemsPerPage) ? itemsPerPage : _this.cachedParams.pageSize;
        var pageIndex = !isNullOrUndefined(currentIndex) ? currentIndex : _this.cachedParams.pageIndex;
        // var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        // var isMember = uplineRoleId == 2 ? true : false;
        //
        // _this.cachedParams.pageIndex = pageIndex;
        // _this.cachedParams.pageSize = pageSize;
        //
        // Fanex.ExtAcc.BlockUI();
        // Fanex.ExtAcc.Customer.CurrentLevel = isSuperList ? Fanex.ExtAcc.Customer.Super : uplineRoleId - 1;
        // this.DOMElements.divSuperLimitDate.toggle(isSuperList);
        // this.DOMElements.divIsNoDownlines.toggle(!isMember);
        // Fanex.ExtAcc.Customer.superId = uplineRoleId === Fanex.ExtAcc.Customer.Super ? uplineId : Fanex.ExtAcc.Customer.superId;
        // Fanex.ExtAcc.Customer.masterId = uplineRoleId === Fanex.ExtAcc.Customer.Master ? uplineId : Fanex.ExtAcc.Customer.masterId;
        // Fanex.ExtAcc.Customer.agentId = uplineRoleId === Fanex.ExtAcc.Customer.Agent ? uplineId : Fanex.ExtAcc.Customer.agentId;
        var params = {
            // groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            // groupId: $('#ddlGroupName').val(),
            // userName: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtUsername.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            // status: this.DOMElements.selStatus.val(),
            // uplineRoleId: uplineRoleId,
            // isShowSuperLimitDate: this.DOMElements.chkShowSuperLimitDate.is(":checked"),
            // IsNoDownlines: this.DOMElements.chkIsNoDownlines.is(":checked"),
            // uplineId: uplineId,
            pageSize: pageSize,
            pageIndex: pageIndex + 1
        };
        //
        // if (isShowCurrency) {
        //     params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
        // }
        //
        _this.cachedParams.previousParams = params;
        _this.Search(params);
    },

    ReloadAfterEdit: function () {
        var _this = Fanex.ExtAcc.Customer.CustomerList;
        _this.Search(_this.cachedParams.previousParams);
    },

    Search: function (params) {
        $.post(parent.resolveClientUrl('HeadlineList/Search'), params, function (result) {
            var _this = Fanex.ExtAcc.HeadlineList;
            // var isSuperList = isNullOrUndefinedOrEmpty(params.uplineRoleId);
            // var hasSubGroups = result.HasSubGroups;
            // _this.cachedParams.uplineId = params.uplineId;
            // _this.cachedParams.uplineRoleId = params.uplineRoleId;
            // var numberOfFrozenColumn = 6;
            // var widthDevice = top.$(top.window).width();
            // if (widthDevice <= 768) {
            //     numberOfFrozenColumn = 3;
            // }
            let bodyRows = [];
            const rowLength = result.GroupHeadlines.length;
            for (let i = 0; i < rowLength; i++) {
                var currentRow = result.GroupHeadlines[i];
                var columns = _this.GetGridBodyRows(currentRow, i, params.pageSize, params.pageIndex - 1);
                bodyRows.push({
                    Columns: columns,
                    // CssClass: Fanex.ExtAcc.Customer.CustomerList.GetCSSRow(currentRow)
                });
            }

            gridOptions = {
                //groupHeaderRows: _this.GetGridGroupHeader(params.uplineRoleId, result.IsMyanmarOdds, result.HasSubGroups),
                columns: _this.GetGridHeader(params.uplineRoleId, result.IsMyanmarOdds, result.HasSubGroups),
                bodyRows: bodyRows,
                autoCalculateGridExpandHeight: true,
                // numberOfFrozenColumn: numberOfFrozenColumn,
                bodyRowHeight: isMobile.phone ? 28 : 25,
                fullHeight: !isMobile.phone,
                gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
                customizeHeader: false,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: true,
                showNumberColumn: true,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: result.PageIndex - 1,
                    totalItem: result.TotalGroups,
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
            //
            // if (bodyRows.length === 0) {
            //     _this.DOMElements.msgNoinfo.css('display', 'block');
            //     _this.DOMElements.jqgrid.css('display', 'none');
            //     _this.DOMElements.icoExportExcel.css('display', 'none');
            // } else {
            _this.DOMElements.msgNoinfo.css('display', 'none');
            _this.DOMElements.jqgrid.css('display', 'block');
            _this.DOMElements.icoExportExcel.show();
            setPagerOptionOnMobile(gridOptions);
            _this.DOMElements.jqgrid.fanexGrid(gridOptions);
            //
            //     workaroundForGridOnPhone();
            // }
            //
            // _this.DOMElements.editMult.attr('style', 'cursor: default !important');
            // _this.DOMElements.editMult.attr('onclick', '');
            // _this.DOMElements.editMult.contextMenu(false);
            // _this.RenderBreadScrumbs(params.uplineId, params.uplineRoleId);
            // _this.DOMElements.icoExportExcel.off();
            // $('#editMult').contextMenu(false);
            // if (bodyRows.length > 0) {
            //     _this.DOMElements.icoExportExcel.click(function () {
            //         var params = {
            //             groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            //             groupId: $('#ddlGroupName').val(),
            //             userName: _this.DOMElements.txtUsername.val(),
            //             status: _this.DOMElements.selStatus.val(),
            //             uplineRoleId: _this.cachedParams.uplineRoleId,
            //             isShowSuperLimitDate: _this.DOMElements.chkShowSuperLimitDate.is(":checked"),
            //             IsNoDownlines: _this.DOMElements.chkIsNoDownlines.is(":checked"),
            //             uplineId: _this.cachedParams.uplineId
            //         };
            //
            //         if (isShowCurrency) {
            //             params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
            //         }
            //
            //         top.exportExcel(resolveClientUrl("CustomerList/GetExportToExcel?") + top.serialize(params), document);
            //     });
            //
            //     var firstCustId = result.CustomerList[0].CustId;
            //     Fanex.ExtAcc.Customer.AllowDoubleComm = result.CustomerList[0].UplineDoubleCommLosing;
            //     $.post(resolveClientUrl('CustomerList/GetCustomerStatus'), { custId: firstCustId, groupName: $("#selectedGroupName").html() }, function (custStatusResult) {
            //         Fanex.ExtAcc.Customer.HideVirtualSports = custStatusResult.HideVirtualSports;
            //         Fanex.ExtAcc.Customer.UplineProductSettings = Fanex.ExtAcc.Customer.GetProductSettings(custStatusResult, !isSuperList);
            //     });
            // }
        }).always(function () {
            Fanex.ExtAcc.UnBlockUI();
            // $("#divSortingByUsername").click(function () {
            //     this.Sorting(this, "Username");
            // });
            $("#jqgrid_header_sort_headline").click(function () {
                const _this = Fanex.ExtAcc.HeadlineList;
                _this.Sorting(this, "Headline");
            });
            $("#jqgrid_header_sort_username").click(function () {
                const _this = Fanex.ExtAcc.HeadlineList;
                _this.Sorting(this, "Username");
            });
        });
    },
    ResetSorting: function () {
        $(".divSorter").removeClass("asc desc");
        this.sortInfo.sortType = "";
    },
    AddSortingIconAfterRender: function (obj) {
        this.ResetSorting();

        if (this.sortInfo.sortClass === "asc") {
            $(obj).addClass("asc");
        } else {
            $(obj).addClass("desc");
        }
    },
    Sorting: function (obj, sortCol) {
        if (this.sortInfo.sortClass === "" || this.sortInfo.sortClass === "desc") {
            this.sortInfo.sortClass = "asc";
            this.sortInfo.sortType = "";
        } else {
            this.sortInfo.sortClass = "desc";
            this.sortInfo.sortType = "-";
        }
        this.sortInfo.sortCol = sortCol;

        if (sortCol === "Username") {
            var newGroup = [];
            var memberNotAssignedToGroup = [];
            console.log("Username sorting...");
            // $.each(this.currentHeadlines, function () {
            //     if (this.GroupId === 0 && this.SMAEntities && this.SMAEntities.length > 0) {
            //         memberNotAssignedToGroup.push(this.SMAEntities[0]);
            //     }
            //     else {
            //         var smaList = jlinq.from(this.SMAEntities).sort(sortType + "Username").select();
            //         newGroup.push({
            //             GroupId: this.GroupId,
            //             GroupName: this.GroupName,
            //             Owner: this.Owner,
            //             SMAEntities: smaList
            //         });
            //     }
            // });
            //
            // if (memberNotAssignedToGroup && memberNotAssignedToGroup.length > 0) {
            //     var sortingMembers = jlinq.from(memberNotAssignedToGroup).sort(sortType + "Username").select();
            //     $.each(sortingMembers, function () {
            //         sortedSMAGroupHeadlines = sortedSMAGroupHeadlines.concat({
            //             GroupId: this.GroupId,
            //             GroupName: this.GroupName,
            //             Owner: this.Owner,
            //             SMAEntities: [this]
            //         });
            //     });
            // }
            //
            // if (newGroup && newGroup.length > 0) {
            //     sortedSMAGroupHeadlines = sortedSMAGroupHeadlines.concat(newGroup);
            // }
        }
        if(sortCol === 'Headline'){
            console.log("Headline sorting")
        }
        //renderSMAGroupHeadlines(sortedSMAGroupHeadlines); // render lai sau khi sort
        //this.AddSortingIconAfterRender(obj);
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

    GetGridHeader: function (uplineRoleId, isMyanmarOdds, hasSubGroups) {
        var columns = [];
        var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
        var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
        var showSubGroups = isSuperList && hasSubGroups;
        columns.push({HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center'});
        columns.push({
            HeaderText: 'Headline', Name: 'headline', Width: 200, CellAlign: 'center',
            CellId: 'HeadlineSorting',
            Sort: true,
            // CssClass: 'divSorter',
            // CustomHeaderRender: function (cell) {
            //     return '<div onclick="Fanex.ExtAcc.HeadlineList.Sorting(this.parentElement,\'Headline\')">Headline</div>';
            // },
        });
        columns.push({
            HeaderText: 'Username', Name: 'username', Width: 200, CellAlign: 'center',
            CellId: 'UsernameSorting',
            Sort: true,
            // CssClass: 'divSorter',
            // CustomHeaderRender: function (cell) {
            //     return '<div onclick="Fanex.ExtAcc.HeadlineList.Sorting(this.parentElement,\'username\')">Username</div>';
            // },
        });
        // columns.push({
        //     HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
        //     CustomHeaderRender: function (cell) {
        //         return '<div id="editMult" class="editMult"></div>';
        //     },
        //     CustomCellRender: function (cell) {
        //         return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.CustId + '" data-username="' + cell.Value.UserName + '" class="menuInactive"></div>';
        //     }
        // });
        // columns.push({
        //     HeaderText: 'Username', Name: 'username', Width: 125,
        //     CustomCellRender: function (cell) {
        //         var usernameValue = cell.Value.UserName,
        //             shortUsername = usernameValue,
        //             tooltipUsername = "";
        //         if (usernameValue.length >= 15) {
        //             shortUsername = usernameValue.substring(0, 15) + "...";
        //             tooltipUsername = usernameValue;
        //         }
        //         var func = 'javascript:Fanex.ExtAcc.Customer.CustomerList.SubmitSearch(' + cell.Value.CustId + ', ' + (isNullOrUndefinedOrEmpty(uplineRoleId) ? Fanex.ExtAcc.Customer.Super : uplineRoleId - 1) + ', null, 0)';
        //         var unlockIcon = cell.Value.LockedAccess.IsLocked
        //             ? '<span class="locked-icon" onclick="javascript:Fanex.ExtAcc.Customer.UnlockAccess(' + cell.Value.CustId + ',' + cell.Value.LockedAccess.Locked + ')" title="Click to unlock"></span>'
        //             : "";
        //         return uplineRoleId === Fanex.ExtAcc.Customer.Agent ?
        //             '<span title="' + tooltipUsername + '">' + shortUsername + '</span>' + unlockIcon :
        //             '<a id="usr-' + cell.Value.CustId + '" href="' + func + '" title="' + tooltipUsername + '">' + shortUsername + '</a>' + unlockIcon;
        //     }
        // });
        // if (isShowCurrency) { columns.push({ HeaderText: 'Currency', Name: 'currency', Width: 75 }); }
        // columns.push({
        //     HeaderText: 'Edit', Name: 'Edit', Width: 30,
        //     CustomCellRender: function (cell) {
        //         return '<div onclick="javascript:Fanex.ExtAcc.Customer.CustomerList.SingleEdit' + Fanex.ExtAcc.Customer.RoleNameConst[cell.Value.RoleId] + '(' + cell.Value.CustId + ', \'' + cell.Value.UserName + '\')" id="edit-' + cell.Value.CustId + '" class="editInfo"></div>';
        //     }
        // });
        columns.push({
            HeaderText: 'Status', Name: 'Status', Width: 100, CellAlign: 'center',
            CustomCellRender: function (cell) {
                return !cell.Value.Enabled ? 'Disabled' : cell.Value.Closed ? 'Closed' : cell.Value.Suspended ? 'Suspended' : 'Open';
            }
        });
       
        return columns.filter(function (item) {
            return item !== undefined
        });
    },
    // GetGridGroupHeader: function (uplineRoleId, isMyanmarOdds, hasSubGroups) {
    //     var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
    //     var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
    //     var showSubGroups = isSuperList && hasSubGroups;
    //     var columns = [];
    //     columns.push({HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2});
    //     columns.push({HeaderText: '', Name: 'Check', MergeRowNumber: 2});
    //     columns.push({HeaderText: 'Username', Name: 'username', MergeRowNumber: 2});
    //     if (isShowCurrency) {
    //         columns.push({HeaderText: 'Currency', Name: 'currency', MergeRowNumber: 2});
    //     }
    //     columns.push({HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2});
    //     columns.push({HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2});
    //     columns.push({HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2});
    //     columns.push(isMemberList ? {HeaderText: 'Nickname', Name: 'NickName', MergeRowNumber: 2} : undefined);
    //     columns.push({HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2});
    //     columns.push({HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2});
    //     columns.push(!isMemberList ? {
    //         HeaderText: 'Commission',
    //         Name: 'commision',
    //         MergeColNumber: isMyanmarOdds ? 4 : 6
    //     } : undefined);
    //     columns.push(showSubGroups ? {
    //         HeaderText: 'Assigned Sub Groups',
    //         Name: 'AssignedSubGroups',
    //         MergeRowNumber: 2
    //     } : undefined);
    //     columns.push(isMemberList ? {HeaderText: 'User Group', Name: 'ugroup', MergeRowNumber: 2} : undefined);
    //     columns.push(isMemberList ? {
    //         HeaderText: 'Agent Commission',
    //         Name: 'discount',
    //         MergeColNumber: isMyanmarOdds ? 4 : 3
    //     } : undefined);
    //     columns.push(isMemberList ? {
    //         HeaderText: 'Player Commission',
    //         Name: 'playerdiscount',
    //         MergeColNumber: isMyanmarOdds ? 4 : 3
    //     } : undefined);
    //     columns.push(!isMemberList ? {
    //         HeaderText: 'No. of Downlines',
    //         Name: 'NoOfDownlines',
    //         MergeRowNumber: 2
    //     } : undefined);
    //     columns.push(isSuperList ? {HeaderText: 'Data Available', Name: 'DateLimit', MergeRowNumber: 2} : undefined);
    //     columns.push({HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2});
    //     columns.push({HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2});
    //     return [{
    //         Columns: columns.filter(function (item) {
    //             return item !== undefined
    //         })
    //     }];
    // },
    GetGridBodyRows: function (currentRow, currentIndex, pageSize, pageIndex) {
        console.log("currentRow",currentRow)
        console.log("currentIndex",currentIndex)
        console.log("pageSize",pageSize)
        console.log("pageIndex",pageIndex)
        var columns = [];
        columns.push({Name: 'RowNumber', Value: currentIndex + pageSize * (pageIndex)});
        // columns.push({Name: 'Check', Value: currentRow});
        columns.push({Name: 'headline', Value: currentRow.GroupName});
        columns.push({Name: 'username', Value: currentRow});
        columns.push({Name: 'Status', Value: currentRow});
        return columns.filter(function (item) {
            return item !== undefined
        });
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
}

// $(document).ready(function () {
//     Fanex.ExtAcc.HeadlineList.Init();
// });