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
    backupResult: {},
    currentHeadlines: [],
    Init: function (uplineId, uplineRoleId) {
        // $(".icon-filter").click(Fanex.ExtAcc.Customer.showHideFilter);
        // this.cachedParams.uplineId = uplineId;
        // this.cachedParams.uplineRoleId = uplineRoleId;
        this.DOMElements.txtUsername = $('#txtUserName');
        this.DOMElements.txtGroupHeadline = $('#txtGroupHeadline');
        this.DOMElements.selStatus = $('#selStatus');
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

    SubmitSearch: function (itemsPerPage, currentIndex) {
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
            UserName: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtUsername.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            GroupHeadline: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtGroupHeadline.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            Status: this.DOMElements.selStatus.val(),
            // uplineRoleId: uplineRoleId,
            // isShowSuperLimitDate: this.DOMElements.chkShowSuperLimitDate.is(":checked"),
            // IsNoDownlines: this.DOMElements.chkIsNoDownlines.is(":checked"),
            // uplineId: uplineId,
            PageSize: pageSize,
            PageIndex: pageIndex + 1
        };
        //
        // if (isShowCurrency) {
        //     params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
        // }
        //
        _this.cachedParams.previousParams = params;
        _this.Search(params);
    },

    // ReloadAfterEdit: function () {
    //     var _this = Fanex.ExtAcc.Customer.CustomerList;
    //     _this.Search(_this.cachedParams.previousParams);
    // },
    RenderBodyRows: function (groupHeadlines) {
        function GetCSSRow(status) {
            switch (status) {
                case 'Disabled':
                    return 'rowDisabled';
                case 'Suspended':
                    return 'rowSuspended';
                default:
                    return '';
            }
        }

        // const GetClassRow = function (currentRow) {
        //     const status = currentRow.Status;
        //   
        //     if (status === 'Disabled') {
        //         return 'rowDisabled';
        //     }
        //
        //     if (currentRow.Closed) {
        //         return Fanex.ExtAcc.Customer.GetClosedClassName(currentRow);
        //     }
        //
        //     if (status === 'Suspended') {
        //         return 'rowSuspended';
        //     }
        //
        //     if (currentRow.LockedAccess.IsLocked) {
        //         return currentRow.LockedAccess.ClassName;
        //     }
        //
        //     return '';
        // };
        let bodyRows = [];
        const numberRow = groupHeadlines.length;
        for (let i = 0; i < numberRow; i++) {
            const groupHeadline = groupHeadlines[i];
            const groupHeadlineLength = groupHeadline.HeadlineEntities.length;
            let rowMerge = groupHeadlineLength;
            for (let j = 0; j < groupHeadlineLength; j++) {
                const headline = groupHeadline.HeadlineEntities[j];
                let columns = [];
                if (j === 0) {
                    columns.push({Name: "RowNumber", Value: i + 1, MergeRowNumber: rowMerge});
                    columns.push({
                        Name: 'GroupName',
                        Value: groupHeadline.GroupName,
                        MergeRowNumber: rowMerge,
                        CellAlign: 'center'
                    });
                }
                columns.push({Name: 'Username', Value: headline.Username, CellAlign: 'center'});
                columns.push({Name: 'Status', Value: headline.Status});
                bodyRows.push({Columns: columns, CssClass: GetCSSRow(headline.Status)});
            }
        }
        return bodyRows;
    },

    Search: function (params) {
        $.post(parent.resolveClientUrl('HeadlineList/Search'), params, function (result) {
            var _this = Fanex.ExtAcc.HeadlineList;
            backupResult = result;
            let bodyRows = _this.RenderBodyRows(result.GroupHeadlines);

            gridOptions = {
                //groupHeaderRows: _this.GetGridGroupHeader(params.uplineRoleId, result.IsMyanmarOdds, result.HasSubGroups),
                columns: _this.GetGridHeader(),
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
                // showNumberColumn: true,
                pagerOption: {
                    itemsPerPageArray: [100, 200, 300, 400, 500],
                    currentPage: result.PageIndex - 1,
                    totalItem: result.TotalGroups,
                    itemsPerPage: params.PageSize,
                    pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                        // Fanex.ExtAcc.Customer.DisableMultipleContextMenu();
                        _this.cachedParams.PageIndex = currentPageIndex;
                        _this.cachedParams.PageSize = itemsPerPage;
                        _this.SubmitSearch(itemsPerPage, currentPageIndex);
                    }
                },
                sortOption:
                    {
                        serverSortFunction: function (sortColumnName, sortDirection) {
                            if (sortColumnName === 'GroupName') {
                                backupResult.GroupHeadlines = _this.SortByNameExceptFirst(backupResult.GroupHeadlines, 'GroupName', sortDirection, true);
                            }
                            if (sortColumnName === 'Username') {
                                for (const element of backupResult.GroupHeadlines) {
                                    element.HeadlineEntities = _this.SortByNameExceptFirst(element.HeadlineEntities, 'Username', sortDirection);
                                }
                            }
                            gridOptions.bodyRows = _this.RenderBodyRows(backupResult.GroupHeadlines);
                            _this.DOMElements.jqgrid.fanexGrid('reRenderBodyAfterSort', gridOptions, sortColumnName, sortDirection);
                        }
                    },
                marginRight: 12,
                showDistinctRow: false,
                allowTruncate: true,
                showHighlightRow: false
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

                //  workaroundForGridOnPhone();
            }
            //
            // _this.DOMElements.editMult.attr('style', 'cursor: default !important');
            // _this.DOMElements.editMult.attr('onclick', '');
            // _this.DOMElements.editMult.contextMenu(false);
            // _this.RenderBreadScrumbs(params.uplineId, params.uplineRoleId);
            // _this.DOMElements.icoExportExcel.off();
            // $('#editMult').contextMenu(false);
            if (bodyRows.length > 0) {
                _this.DOMElements.icoExportExcel.off().click(function () {
                    var params = {
                        // groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
                        // groupId: $('#ddlGroupName').val(),
                        UserName: _this.DOMElements.txtUsername.val(),
                        GroupHeadline: _this.DOMElements.txtGroupHeadline.val(),
                        Status: _this.DOMElements.selStatus.val(),
                        // uplineRoleId: _this.cachedParams.uplineRoleId,
                        // isShowSuperLimitDate: _this.DOMElements.chkShowSuperLimitDate.is(":checked"),
                        // IsNoDownlines: _this.DOMElements.chkIsNoDownlines.is(":checked"),
                        // uplineId: _this.cachedParams.uplineId
                    };

                    // if (isShowCurrency) {
                    //     params.currencyIds = _this.DOMElements.$multiSelectCurrency.fanexMultiSelect("getItems", true).join(",");
                    // }

                    top.exportExcel(resolveClientUrl("HeadlineList/ExportHeadlines?") + top.serialize(params), document);
                });

                // var firstCustId = result.CustomerList[0].CustId;
                // Fanex.ExtAcc.Customer.AllowDoubleComm = result.CustomerList[0].UplineDoubleCommLosing;
                // $.post(resolveClientUrl('CustomerList/GetCustomerStatus'), { custId: firstCustId, groupName: $("#selectedGroupName").html() }, function (custStatusResult) {
                //     Fanex.ExtAcc.Customer.HideVirtualSports = custStatusResult.HideVirtualSports;
                //     Fanex.ExtAcc.Customer.UplineProductSettings = Fanex.ExtAcc.Customer.GetProductSettings(custStatusResult, !isSuperList);
                // });
            }
        }).always(function () {
            Fanex.ExtAcc.UnBlockUI();
            // $("#divSortingByUsername").click(function () {
            //     this.Sorting(this, "Username");
            // });
            // $("#jqgrid_header_sort_headline").off().click(function () {
            //     const _this = Fanex.ExtAcc.HeadlineList;
            //     _this.Sorting(this, "Headline");
            // });
            // $("#jqgrid_header_sort_username").off().click(function () {
            //     const _this = Fanex.ExtAcc.HeadlineList;
            //     _this.Sorting(this, "Username");
            // });
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
    // Sorting: function (obj, sortCol) {
    //     if (this.sortInfo.sortClass === "" || this.sortInfo.sortClass === "desc") {
    //         this.sortInfo.sortClass = "asc";
    //         this.sortInfo.sortType = "";
    //     } else {
    //         this.sortInfo.sortClass = "desc";
    //         this.sortInfo.sortType = "-";
    //     }
    //     this.sortInfo.sortCol = sortCol;
    //
    //     if (sortCol === "Username") {
    //         console.log("Username sorting...");
    //         // $.each(this.currentHeadlines, function () {
    //         //     if (this.GroupId === 0 && this.SMAEntities && this.SMAEntities.length > 0) {
    //         //         memberNotAssignedToGroup.push(this.SMAEntities[0]);
    //         //     }
    //         //     else {
    //         //         var smaList = jlinq.from(this.SMAEntities).sort(sortType + "Username").select();
    //         //         newGroup.push({
    //         //             GroupId: this.GroupId,
    //         //             GroupName: this.GroupName,
    //         //             Owner: this.Owner,
    //         //             SMAEntities: smaList
    //         //         });
    //         //     }
    //         // });
    //         //
    //         // if (memberNotAssignedToGroup && memberNotAssignedToGroup.length > 0) {
    //         //     var sortingMembers = jlinq.from(memberNotAssignedToGroup).sort(sortType + "Username").select();
    //         //     $.each(sortingMembers, function () {
    //         //         sortedSMAGroupHeadlines = sortedSMAGroupHeadlines.concat({
    //         //             GroupId: this.GroupId,
    //         //             GroupName: this.GroupName,
    //         //             Owner: this.Owner,
    //         //             SMAEntities: [this]
    //         //         });
    //         //     });
    //         // }
    //         //
    //         // if (newGroup && newGroup.length > 0) {
    //         //     sortedSMAGroupHeadlines = sortedSMAGroupHeadlines.concat(newGroup);
    //         // }
    //     }
    //     if (sortCol === 'Headline') {
    //         console.log("Headline sorting")
    //     }
    //     //renderSMAGroupHeadlines(sortedSMAGroupHeadlines); // render lai sau khi sort
    //     //this.AddSortingIconAfterRender(obj);
    // },


    GetGridHeader: function () {
        var columns = [];
        columns.push({HeaderText: 'No.', Name: 'RowNumber', Width: 50, CellAlign: 'center'});
        columns.push({
            HeaderText: 'Headline', Name: 'GroupName', Width: 250, CellAlign: 'center',
            CellId: 'HeadlineSorting',
            Sort: true,
        });
        columns.push({
            HeaderText: 'Username', Name: 'Username', Width: 250, CellAlign: 'center',
            CellId: 'UsernameSorting',
            Sort: true,
        });

        columns.push({
            HeaderText: 'Status', Name: 'Status', Width: 80, CellAlign: 'center',
        });

        return columns.filter(function (item) {
            return item !== undefined
        });
    },

    RenderHeadlineEntities: function (cell, property) {
        const headlineEntities = cell.Value.HeadlineEntities || [];
        const containerDiv = [];

        function createDiv(entity, isLast) {
            const style = isLast ? '' : 'border-color: #c5c5c5; border-style: solid; border-width: 0 0 1px 0; width: inherit;';
            return `<div style="${style}">${entity[property]}</div>`;
        }

        for (let i = 0; i < headlineEntities.length; i++) {
            if (i === headlineEntities.length - 1) {
                containerDiv.push(createDiv(headlineEntities[i], true));
            } else {
                containerDiv.push(createDiv(headlineEntities[i], false));
            }
        }

        return containerDiv.join('');
    },
    // GetGridGroupHeader: function (uplineRoleId, isMyanmarOdds, hasSubGroups) {
    //     var isSuperList = isNullOrUndefinedOrEmpty(uplineRoleId);
    //     var isMemberList = uplineRoleId === Fanex.ExtAcc.Customer.Agent;
    //     var showSubGroups = isSuperList && hasSubGroups;   v
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

        var columns = [];
        //columns.push({Name: 'RowNumber', Value: currentIndex + pageSize * (pageIndex)});
        columns.push({Name: 'RowNumber', Value: currentIndex + 1});
        columns.push({Name: 'headline', Value: currentRow.GroupName});
        // columns.push({Name: 'username', Value: currentRow});
        //columns.push({Name: 'Status', Value: currentRow});
        columns.push({Name: 'username', Value: currentRow.Username});
        columns.push({Name: 'Status', Value: currentRow.Status});
        return columns.filter(function (item) {
            return item !== undefined
        });
    },
    SortByNameExceptFirst: function (arr, sortColumnName, sortDirection, ignoreFirst = false) {
        const firstElement = ignoreFirst ? arr.shift() : null;
        const sorted = arr.sort(function (a, b) {
            const comparison = a[sortColumnName].localeCompare(b[sortColumnName]);
            return sortDirection === "asc" ? comparison : -comparison;
        });

        const result = ignoreFirst ? [firstElement].concat(sorted) : sorted;
        console.log("call sort", sortColumnName, sortDirection, result)
        return result;
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