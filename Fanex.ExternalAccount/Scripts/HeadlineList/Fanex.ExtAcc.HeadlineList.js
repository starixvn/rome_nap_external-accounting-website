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
    
    Init: function () {
        this.DOMElements.txtUsername = $('#txtUserName');
        this.DOMElements.txtGroupHeadline = $('#txtGroupHeadline');
        this.DOMElements.selStatus = $('#selStatus');
        this.DOMElements.btnSearch = $('#btnSearch');
        this.DOMElements.icoExportExcel = $('#icoExportExcel');
        this.DOMElements.jqgrid = $('#jqgrid');
        this.DOMElements.msgNoinfo = $('#msgNoinfo');
        this.BindingEvents();
        this.DOMElements.btnSearch.click();

    },
    
    BindingEvents: function () {
        this.DOMElements.btnSearch.off().click(function () {
            const _this = Fanex.ExtAcc.HeadlineList;
            _this.SubmitSearch(_this.uplineId, _this.uplineRoleId, undefined, 0);
        });

        this.DOMElements.txtUsername.keypress(function (e) {
           const _this = Fanex.ExtAcc.HeadlineList;
            const code = e.keyCode ? e.keyCode : e.which;
            if (code === 13) {
                _this.DOMElements.btnSearch.click()
            }
        });

        this.DOMElements.txtGroupHeadline.keypress(function (e) {
            const _this = Fanex.ExtAcc.HeadlineList;
            const code = e.keyCode ? e.keyCode : e.which;
            if (code === 13) {
                _this.DOMElements.btnSearch.click()
            }
        });
    },

    Search: function (params) {
        $.post(parent.resolveClientUrl('HeadlineList/Search'), params, function (result) {
            var _this = Fanex.ExtAcc.HeadlineList;
            backupResult = result;
            let bodyRows = _this.RenderBodyRows(result.GroupHeadlines);

            gridOptions = {
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
                workaroundForGridOnPhone();
            }

            if (bodyRows.length > 0) {
                _this.DOMElements.icoExportExcel.off().click(function () {
                    var params = {
                        UserName: _this.DOMElements.txtUsername.val(),
                        GroupHeadline: _this.DOMElements.txtGroupHeadline.val(),
                        Status: _this.DOMElements.selStatus.val(),
                    };

                    top.exportExcel(resolveClientUrl("HeadlineList/ExportHeadlines?") + top.serialize(params), document);
                });
            }
        }).always(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    },
    
    SubmitSearch: function (itemsPerPage, currentIndex) {
        const _this = Fanex.ExtAcc.HeadlineList;
        const pageSize = !isNullOrUndefined(itemsPerPage) ? itemsPerPage : _this.cachedParams.pageSize;
        const pageIndex = !isNullOrUndefined(currentIndex) ? currentIndex : _this.cachedParams.pageIndex;
        
        Fanex.ExtAcc.BlockUI();
        
        const params = {
            UserName: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtUsername.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            GroupHeadline: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtGroupHeadline.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            Status: this.DOMElements.selStatus.val(),
            PageSize: pageSize,
            PageIndex: pageIndex + 1
        };
      
        _this.cachedParams.previousParams = params;
        _this.Search(params);
    },
    
    GetCSSRow: function (currentRow) {
        function GetClosedClassName(currentRow) {
            switch (currentRow.ClosedReason) {
                case 1:
                    return "closed-byAdmin";
                case 2:
                    return "closed-byUpline";
                case 4:
                    return "closed-byArchive";
                default:
                    return "";
            }
        }

        if (!currentRow.IsEnable) {
            return 'rowDisabled';
        }

        if (currentRow.IsClosed) {
            return GetClosedClassName(currentRow);
        }

        if (currentRow.IsSuspend) {
            return 'rowSuspended';
        }
        return '';
    },
    
    RenderBodyRows: function (groupHeadlines) {
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
                        CellAlign: 'left'
                    });
                }
                columns.push({Name: 'Username', Value: headline.Username, CellAlign: 'left'});
                columns.push({
                    Name: 'Status',
                    Value: !headline.IsEnable ? 'Disabled' : headline.IsClosed ? 'Closed' : headline.IsSuspend ? 'Suspended' : 'Open'
                });
                bodyRows.push({Columns: columns, CssClass: Fanex.ExtAcc.HeadlineList.GetCSSRow(headline)});
            }
        }
        return bodyRows;
    },
    
    GetGridHeader: function () {
        const columns = [];
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
    
    SortByNameExceptFirst: function (arr, sortColumnName, sortDirection, ignoreFirst = false) {
        const firstElement = ignoreFirst ? arr.shift() : null;
        const sorted = arr.sort(function (a, b) {
            const comparison = a[sortColumnName].localeCompare(b[sortColumnName]);
            return sortDirection === "asc" ? comparison : -comparison;
        });

        const result = ignoreFirst ? [firstElement].concat(sorted) : sorted;
        return result;
    }
}
