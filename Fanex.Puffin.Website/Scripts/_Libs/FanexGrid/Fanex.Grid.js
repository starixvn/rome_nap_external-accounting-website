// --------------------------------------------------------------------------
// <copyright file="Fanex.Grid.js" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

(function ($) {

    //// Grid Scroller
    var ub = $.browser;
    ub.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
    var ff16 = $.browser.mozilla && parseInt($.browser.version, 10) >= 16;
    var ff18 = $.browser.mozilla && (parseInt($.browser.version, 10) >= 18);
    var extraPosition = ff18 ? 1 : 0;
    //// Method expose for user use
    var methods =
    {
        createFanexGrid: function (opts) {
            if (opts.showColumnsShowHide == false) {
                opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, opts.columnsNeedHide);
            }
            opts = $.extend(true, {}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        reRender: function (opts) {
            if (opts.showColumnsShowHide == false) {
                opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, opts.columnsNeedHide);
            }
            opts = $.extend(true, {}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        reRenderBodyAfterSort: function (opts, sortColumnName, sortDirection) {
            opts = $.extend(true, {}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.ReRenderBodyAfterSort(sortColumnName, sortDirection);
                if (opts.showPager == true) {
                    fanexGrid.RenderPager();
                }
                return fanexGrid.GetjQueryInstance();
            });
        },
        showColumns: function (opts, columnsNeedShow) {
            opts.showColumnsShowHide = true;
            opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, columnsNeedShow);
            opts = $.extend(true, {}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                opts.pagerOption = Fanex.Grid.List[fanexGrid.tableContentId].pagerOption;
                Fanex.Grid.Helper.UpdateSortInforAfterShowHideColumns(opts, Fanex.Grid.List[fanexGrid.tableContentId].sortOption);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        hideColumns: function (opts, columnsNeedHide) {
            opts.showColumnsShowHide = false;
            opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, columnsNeedHide);
            opts.columnsNeedHide = columnsNeedHide;
            opts = $.extend(true, {}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                opts.pagerOption = Fanex.Grid.List[fanexGrid.tableContentId].pagerOption;
                Fanex.Grid.Helper.UpdateSortInforAfterShowHideColumns(opts, Fanex.Grid.List[fanexGrid.tableContentId].sortOption);
                //fanexGrid.RenderGrid();
                fanexGrid.RenderShowHideColumns(false);
                return fanexGrid.GetjQueryInstance();
            });
        },
        resizeGrid: function () {
            return this.each(function () {
                var $this = $(this);
                var currentContentId = 'tblContent_' + $this.attr('id');
                $this.gridScroller(Fanex.Grid.List[currentContentId].gridOptions);
                return $this;
            });
        }
    };

    //// Plugin
    $.fn.fanexGrid = function (options) {
        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return methods.createFanexGrid.apply(this, arguments);
        }
        else {
            throw 'Options cannot be null';
        }
    };

    //// Default Options
    $.fn.fanexGrid.defaults =
    {
        customizeHeader: false,
        headerRows: [],
        columns: [],
        bodyRows: [],
        numberOfFrozenColumn: 0,
        showFooter: false,
        footerRows: [],
        showTitle: false,
        headerColumnHeight: 25,
        bodyRowHeight: 21,
        footerRowHeight: 21,
        titleInfo:
        {
            text: "",
            hideWhenNoData: true,
            showExcelButton: false,
            exportExcelFunction: null,
            showBackButton: false,
            backFunction: null,
            showSendMailButton: false,
            sendMailFunction: null,
            showCollapseExpandButton: false,
            customButtons: ''
        },
        showPager: false,
        pagerOption:
        {
            itemsPerPage: 100,
            maxItemsPerPage: 500,
            currentPage: 1,
            totalItem: 0,
            startPerPage: 100,
            pageStep: 50,
            afterPagedFunction: null
        },
        showTree: false,
        gridExpandHeight: 150,
        showNoDataOnFrozen: false,
        showColumnsShowHide: true,
        showDistinctRow: true,
        showHighlightRow: true,
        highlightFrozenWidth: 1,
        showNumberColumn: false,
        marginRight: 5,
        allowTruncate: false,
        columnsNeedHide: [],
        sortOption:
        {
            customGridSort: null,
            serverSortFunction: null,
            afterGridSorted: null,
            beforeGridSorted: null
        },
        hideYBackGroundScroller: false,
        bottomBorderWidth: 1,
        treeParentPaddingLeft: 20,
        isResetScroller: true
    };

    var Fanex = Fanex || {};

    Fanex.Grid = function (grid, options) {
        this.Options = options;
        this.$grid = $(grid);
        this.gridId = this.$grid.attr('id');
        this.tableContentId = 'tblContent_' + this.gridId;
        this.frozenContentId = 'frozenContent_' + this.gridId;
        this.headerColumnsCloneFromOptionColumns = Fanex.Grid.Helper.CreateHeaderColumnsFromOptionsColumn(options.columns.clone());
        //// Calculate fontsize for hide cell value if cell value exceed
        try {
            this.currentFontSize = parseInt(this.$grid.css('font-size').replace('px', ''));
        }
        catch (e) {
            this.currentFontSize = 12;
        }
        this.currentFontSize -= 4;
        //// Create a hashtable contain columns information for esay retrieve data
        this.columnsHastable = Fanex.Grid.Helper.GetColumnsHastable(options.columns);
        //// Create columnswidth array
        this.columnsWidth = Fanex.Grid.Helper.GetArrayWidth(options.columns, options.showColumnsShowHide, false);
        this.columnsForComputingWidth = Fanex.Grid.Helper.GetArrayWidth(options.columns, options.showColumnsShowHide, true);
        if (this.Options.customPager != true) {
            this.Options.pagerOption.totalItem = this.Options.bodyRows.length;
        }
        this.gridOptions = null;
        this.frozenColsName = [];
        //// If frozen, then create an array contain frozen columns name
        if (options.numberOfFrozenColumn > 0) {
            //var frozenColumnNumber = isValid(options.numberOfFrozenColumnSaved) ? options.numberOfFrozenColumnSaved : options.numberOfFrozenColumn;
            for (var colIndex = 0; colIndex < options.numberOfFrozenColumn; colIndex++) {
                this.frozenColsName.push(options.columns[colIndex].Name);
            }
            //// If highlight frozen then set width for hightligth
            if (options.highlightFrozenWidth > 1) {
                var lastFrozenCol = options.columns[options.numberOfFrozenColumn - 1];
                if (isValid(lastFrozenCol.TrClass)) {
                    lastFrozenCol.TdClass += " border-right-highlight ";
                }
                else {
                    lastFrozenCol.TdClass = "border-right-highlight ";
                }
                options.highlightFrozenWidth--;
            }
        }
    };

    Fanex.Grid.prototype =
    {
        RenderGrid: function () {
            var me = this;
            var options = me.Options;
            //// Render Table Main Frame
            var html = me.RenderBody(me.RenderHeader()) + (options.showFooter ? me.RenderFooter() : "");
            me.$grid.empty().html(html);
            me.$grid.before(me.RenderTitle());
            //me.$grid.parent().css("float","left");
            me.RegisterMouseHoverEvent();
            me.Initialize();
            //// Render Title
            //
        },
        RenderShowHideColumns: function (isShow) {
            var me = this;
            var options = me.Options;
            var currentMainBodyHtml = me.$grid.find('#' + me.tableContentId + '> tbody').html();
            var currentFrozenBodyHeader = me.$grid.find('#' + me.frozenContentId + '> tbody').html()
            var bodyHtml = me.RenderHeader() + me.RenderCommonHeader('mainbody');
            var footer = (options.showFooter ? me.RenderFooter() : "");
            var bodyHtmlTemp = '';
            // Fixbug for replace $' string
            if (options.numberOfFrozenColumn > 0) {
                bodyHtmlTemp = bodyHtml.split('{$}frozenBody');
                if (bodyHtmlTemp.length == 2) {
                    bodyHtml = bodyHtmlTemp[0] + currentFrozenBodyHeader + bodyHtmlTemp[1];
                }
            }
            bodyHtmlTemp = bodyHtml.split('{$}_');
            if (bodyHtmlTemp.length == 2) {
                bodyHtml = bodyHtmlTemp[0] + currentMainBodyHtml + bodyHtmlTemp[1];
            }

            bodyHtml += footer;

            me.$grid.empty().html(bodyHtml);
            var selector = [];
            var columnsShowLength = options.columns.length;
            for (var colIndex = 0; colIndex < columnsShowLength; colIndex++) {
                //selector.push('.free-grid-td-' + colIndex);
                me.$grid.find('.free-grid-td-' + colIndex).removeClass("grid-show-hide-column");
            }
            //me.$grid.find(selector.join(',')).show();

            if (isShow) {

            }
            else {
                var columnsHideLength = options.columnsNeedHide.length;
                selector = [];
                try {
                    for (var colIndex1 = 0; colIndex1 < columnsHideLength; colIndex1++) {
                        var currentCol = options.columnsNeedHide[colIndex1];
                        //selector.push('.free-grid-td-' + currentCol);
                        me.$grid.find('.free-grid-td-' + currentCol).addClass("grid-show-hide-column");
                    }
                }
                catch (e) {
                    console.log(e);
                }
                //me.$grid.find(selector.join(',')).hide();
            }

            me.RegisterMouseHoverEvent();
            me.Initialize();
        },
        ReRenderBodyAfterSort: function (sortColumnName, sortDirection) {
            var me = this;
            var options = me.Options;
            me.RenderBody('', true);
            me.gridOptions = Fanex.Grid.List[me.tableContentId].gridOptions;
            me.gridOptions.isResetScroller = false;
            me.RegisterMouseHoverEvent();
            $(me.$grid).gridScroller(me.gridOptions);
            //// Save Current Sor
            Fanex.Grid.List[me.tableContentId].sortOption =
            {
                SortColumnName: sortColumnName,
                SortDirection: sortDirection
            };
        },
        GetjQueryInstance: function () {
            return this.$grid;
        },
        Initialize: function () {
            var me = this;
            var options = this.Options;
            var numberOfFrozenColumnNew = (options.showColumnsShowHide == false ? Fanex.Grid.Helper.ComputeNewFrozenColumn(options.columnsNeedHide, options.numberOfFrozenColumn) : options.numberOfFrozenColumn);
            //// Render Grid Scroller
            me.gridOptions =
            {
                tblContentId: me.tableContentId,
                fullHeight: true,
                column: me.columnsWidth,
                gridHeight: options.gridExpandHeight,
                numberFreezeCol: numberOfFrozenColumnNew,
                highlightFrozenWidth: options.highlightFrozenWidth,
                heightScrollYAdd: 0,
                marginRight: options.marginRight,
                hideYBackGroundScroller: options.hideYBackGroundScroller,
                bottomBorderWidth: options.bottomBorderWidth,
                isResetScroller: options.isResetScroller
            };
            $(me.$grid).initGrid({
                isShowPager: (options.showPager && (options.bodyRows.length > 0))
            }).gridScroller(me.gridOptions);

            //// Render Pager
            if (options.showPager == true) {
                me.RenderPager();
                Fanex.Grid.List[me.tableContentId].pagerOption = options.pagerOption;
            }
            me.RegisterEvents();

        },
        RenderTitle: function () {
            var me = this;
            var options = me.Options;
            var titleId = me.gridId + '-grid-title';
            if (options.showTitle == false) {
                return '';
            }
            else {
                me.$grid.parent().find('#' + titleId).remove();
                // style="width:' + me.$grid.width() + 'px"
                var titleHtml = [];
                titleHtml.push('<div id="' + me.gridId + '-grid-title' + '" class="grid-title display-none"><div class="grid-title-label" id="reporttile">');
                titleHtml.push(options.titleInfo.text);
                titleHtml.push('</div>');
                titleHtml.push('<div class="grid-toolbox">');
                if (options.titleInfo.showBackButton == true) {
                    titleHtml.push('<input id="btnBack' + me.gridId + '" class="btnBack" type="button" title="Back" value="" name="btnBack" />');
                }
                if (options.titleInfo.hideWhenNoData == true && options.bodyRows.length > 0) {
                    if (options.titleInfo.customButtons != '') {
                        titleHtml.push(options.titleInfo.customButtons);
                    }
                    if (options.titleInfo.showCollapseExpandButton == true) {
                        titleHtml.push('<input class="btExpand expanded" id="btnCollapseExpand' + me.gridId + '" type="button" value="" title="Collapse/Expand" />');
                    }
                    if (options.titleInfo.showExcelButton == true) {
                        titleHtml.push('<input type="button" id="btnExport' + me.gridId + '" class="btnExcel" title="Export to Excel" />');
                    }
                    if (options.titleInfo.showSendMailButton == true) {
                        titleHtml.push('<input id="btnSendMail' + me.gridId + '" class="btnSendMail" type="button" title="Send Mail" />');
                    }
                }
                titleHtml.push('</div></div>');

                return titleHtml.join('');
            }
        },
        RenderHeader: function () {
            var me = this;
            var options = this.Options;
            var headerHtml = [];



            //// Clone and Add last row of Group Header
            if (options.customizeHeader) {
                this.Options.cloneGroupHeaderRows = options.groupHeaderRows.clone();
                this.Options.cloneGroupHeaderRows.push({
                    Columns: me.headerColumnsCloneFromOptionColumns.clone()
                });

            }

            //// If there are any item of header column
            var columnLength = options.columns.length;
            if (columnLength > 0) {
                if (options.numberOfFrozenColumn > 0) {
                    headerHtml.push(me.RenderCommonHeader('frozenheader').replace('{$}_', ''));
                    headerHtml.push(me.RenderCommonHeader('frozenbody'));
                }
                headerHtml.push(me.RenderCommonHeader('mainheader').replace('{$}_', ''));
            }

            return headerHtml.join('');
        },
        RenderCommonHeader: function (type) {
            var me = this;
            var options = this.Options;
            var divHeaderClass = '';
            var divContainerClass = '';
            var tableClass = '';
            var hasId = false;
            var tableContainId = me.tableContentId;
            var columns = me.headerColumnsCloneFromOptionColumns.slice(0);
            var columnLength = me.headerColumnsCloneFromOptionColumns.length;
            var replaceMark = '_';

            // Grid Frame
            //////////////////////////////////
            // frozen Header //  main header//
            //////////////////////////////////
            // frozen body   // main body   //
            //////////////////////////////////
            switch (type) {
                case 'frozenheader':
                    divContainerClass = 'free-cell free-top-left';
                    tableClass = 'grid-fixed-table free-cell-top-left';
                    columnLength = options.numberOfFrozenColumn;
                    break;
                case 'frozenbody':
                    divHeaderClass = 'row_header_wrapper';
                    divContainerClass = 'row_header row-headers-background';
                    tableClass = 'grid-fixed-table row_header_content';
                    columnLength = options.numberOfFrozenColumn;
                    hasId = true;
                    tableContainId = me.frozenContentId;
                    replaceMark = 'frozenBody';
                    break;
                case 'mainheader':
                    divHeaderClass = 'col_header row-headers-background scroll-y';
                    divContainerClass = 'header-container';
                    tableClass = 'grid-fixed-table col_header_content';
                    break;
                case 'mainbody':
                    divHeaderClass = 'grid-container container';
                    divContainerClass = 'content-container';
                    tableClass = 'grid-fixed-table col_body_content';
                    hasId = true;
                    break;
            }

            var headerHtml = [];
            var trsHeader = [];
            //// Build Header
            if (divHeaderClass != '') {
                headerHtml.push('<div class="' + divHeaderClass + '">');
            }
            headerHtml.push('<div class="' + divContainerClass + '">');
            headerHtml.push('<table class="' + tableClass + '" ' + (hasId ? ('id="' + tableContainId + '"') : '') + ' ><thead>');

            if (options.customizeHeader) {
                var currentGroupHeader = options.cloneGroupHeaderRows;
                if (type == 'frozenheader' || type == 'frozenbody') {
                    //// Create Header Rows for Frozen Table
                    currentGroupHeader = Fanex.Grid.Helper.CreateRowsForFrozenColumn(options.cloneGroupHeaderRows.slice(0), options.numberOfFrozenColumn, me.frozenColsName);
                }
                for (var rowIndex = 0; rowIndex < currentGroupHeader.length; rowIndex++) {
                    var currentRow = currentGroupHeader[rowIndex];

                    if (currentRow.Columns.length > 0) {
                        headerHtml.push('<tr>');

                        for (var colIndex = 0; colIndex < currentRow.Columns.length && colIndex < columnLength; colIndex++) {
                            var currentCol = currentRow.Columns[colIndex];
                            if (isValid(currentCol)) {
                                var customSpan = '';
                                var currentHeight = options.headerColumnHeight;
                                var showHeaderMergeCol = true;

                                if (isValid(currentCol) && isValid(currentCol.MergeRowNumber)) {
                                    //// If It has merge row and current row has only one columns and column has merge row, then remove next row
                                    Fanex.Grid.Helper.RemoveItemForRowSpan(currentGroupHeader, rowIndex, currentCol, me.columnsHastable);
                                    var currentMergeRowNumber = ' rowspan="' + currentCol.MergeRowNumber + '"';
                                    if (type == 'frozenheader' || type == 'frozenbody') {
                                        //// Calculate height for merger row
                                        currentHeight = ((options.headerColumnHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5);
                                    }
                                    customSpan += currentMergeRowNumber;
                                }

                                //// If Current cell has merge Col
                                if (isValid(currentCol) && isValid(currentCol.MergeColNumber)) {
                                    //// Compute for With for merge
                                    currentCol.Width = Fanex.Grid.Helper.GetColMergeWidth(me.columnsForComputingWidth, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber, options.showColumnsShowHide, options.columns);
                                    if (options.showColumnsShowHide == false) {
                                        showHeaderMergeCol = Fanex.Grid.Helper.HideHeaderGroupWhenShowHide(options.columnsNeedHide, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber);
                                        customSpan += ' colspan="' + Fanex.Grid.Helper.ReComputeMergeColumAfterShowHide(options.columnsNeedHide, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber) + '"';
                                    }
                                    else {
                                        customSpan += ' colspan="' + currentCol.MergeColNumber + '"';
                                    }
                                }

                                if (isValid(currentCol) && isValid(currentCol.MergeHeaderColNumber)) {
                                    customSpan += ' colspan="' + currentCol.MergeHeaderColNumber + '"';
                                }

                                var cellInfo =
                                {
                                    CellTag: 'th',
                                    CustomSpan: customSpan,
                                    CssClass: currentCol.CssClass,
                                    SortClass: (currentRow.Columns[colIndex].Sort == true ? ' colSorter' : ''),
                                    CellWidth: (currentCol.Width || me.columnsHastable[currentCol.Name].Width),
                                    CellHeight: currentHeight,
                                    CellId: (me.columnsHastable[currentCol.Name].Sort == true ? (me.gridId + '_header_sort_' + currentCol.Name) : ''),
                                    Sort: currentRow.Columns[colIndex].Sort,
                                    CellColumnName: currentCol.Name,
                                    HeaderText: currentCol.HeaderText,
                                    CustomHeaderRender: currentCol.CustomHeaderRender || me.columnsHastable[currentCol.Name].CustomHeaderRender,
                                    SortDirection: currentRow.Columns[colIndex].SortDirection || '',
                                    CanShowHide: (me.columnsHastable[currentCol.Name].CanShowHide && (!options.showColumnsShowHide) && showHeaderMergeCol),
                                    TdClass: me.columnsHastable[currentCol.Name].TdClass || '',
                                    CellAlign: me.columnsHastable[currentCol.Name].HeaderAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'center')
                                };
                                trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                            }

                        }
                        headerHtml.push(trsHeader.join(''));
                        headerHtml.push('</tr>');
                        trsHeader = [];
                    }
                }
            }
            else {
                //// Has no frozen column
                headerHtml.push('<tr>');
                for (var colIndex = 0; colIndex < columnLength; colIndex++) {
                    var currentCol = me.headerColumnsCloneFromOptionColumns[colIndex];
                    var customSpan = '';
                    if (isValid(currentCol) && isValid(currentCol.MergeHeaderColNumber)) {
                        customSpan += ' colspan="' + currentCol.MergeHeaderColNumber + '"';
                    }
                    var cellInfo =
                    {
                        CellTag: 'th',
                        CustomSpan: customSpan,
                        CssClass: currentCol.CssClass,
                        SortClass: (currentCol.Sort == true ? ' colSorter' : ''),
                        CellWidth: (currentCol.Width || me.columnsHastable[currentCol.Name].Width),
                        CellHeight: options.headerColumnHeight,
                        CellId: (me.columnsHastable[currentCol.Name].Sort == true ? (me.gridId + '_header_sort_' + currentCol.Name) : ''),
                        Sort: currentCol.Sort,
                        CellColumnName: currentCol.Name,
                        HeaderText: currentCol.HeaderText,
                        CustomHeaderRender: currentCol.CustomHeaderRender || me.columnsHastable[currentCol.Name].CustomHeaderRender,
                        SortDirection: currentCol.SortDirection || '',
                        CanShowHide: me.columnsHastable[currentCol.Name].CanShowHide && (!options.showColumnsShowHide),
                        TdClass: me.columnsHastable[currentCol.Name].TdClass || '',
                        CellAlign: me.columnsHastable[currentCol.Name].HeaderAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'center')
                    };
                    trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                }
                headerHtml.push(trsHeader.join(''));
                headerHtml.push('</tr>');
            }
            headerHtml.push('</thead><tbody>{$}' + replaceMark + '</tbody></table>');
            headerHtml.push('</div>');
            if (divHeaderClass != '') {
                headerHtml.push('</div>');
            }
            return headerHtml.join('');
        },
        RenderFrozenRow: function (currentRow, colHtmlTag, isBody, rowIndex, treeCollapseExpandClass, isForNoData, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var frozenRowHtml = [];
            var colsLength = currentRow.Columns.length;
            var currentCollapseExpandId = '';
            var parentRowClass = '';
            var trHighlightAtt = (options.showHighlightRow ? (' highlightatt="highlight-att-frozen-' + parentHighlight + '-' + rowIndex + '"') : '');
            var trClass = (options.showHighlightRow ? 'highlight-row ' : '') + (treeCollapseExpandClass || '') + (currentRow.CssClass || '') + (options.showDistinctRow ? ((rowIndex + 1) % 2 == 0 ? ' freegrid-even' : '') : '');
            if (options.showTree == true && currentRow.SubRows) {
                parentRowClass = 'parent-row parent-level-' + level;
            }
            frozenRowHtml.push('<tr class="' + trClass + ' ' + parentRowClass + '" ' + (currentRow.CustomAttr || '') + trHighlightAtt + '>');
            for (var columnIndex = 0; columnIndex < colsLength; columnIndex++) {
                var currentCol = currentRow.Columns[columnIndex];
                var customSpan = '';
                var currentHeight = 'height:' + options.bodyRowHeight + 'px;';
                var customClass = 'table-cell';
                var currentWidth = me.columnsHastable[currentCol.Name].Width;
                var setCurrentWidth = false;
                var currentHeightForComputeCenter = options.bodyRowHeight;

                //// If Merge Row, then add rowspan, If in the same row and there are no other columns, then cal height, or there are some columns but they also have colspan, then cal height
                if (isValid(currentCol.MergeRowNumber)) {
                    customSpan += ' rowspan="' + currentCol.MergeRowNumber + '" ';
                    if (Fanex.Grid.Helper.CheckHideColSpan(currentRow.Columns)) {
                        currentHeightForComputeCenter = ((options.bodyRowHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5);
                        currentHeight = 'height:' + currentHeightForComputeCenter + 'px;';
                        customSpan = '';
                    }
                }

                //// If Merge Column and current row is footer, then calculate height
                if (isValid(currentCol.MergeColNumber)) {
                    customSpan += ' colspan="' + currentCol.MergeColNumber + '" ';
                    currentWidth = Fanex.Grid.Helper.GetColMergeWidth(me.columnsWidth, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber, options.showColumnsShowHide, options.columns);
                }
                var renderObject = me.RenderBodyCell(
                        {
                            HtmlTag: colHtmlTag,
                            CustomSpan: customSpan,
                            Height: currentHeight
                        },
                        {
                            IsNoData: isForNoData,
                            ColIndex: columnIndex,
                            RowIndex: rowIndex,
                            CssClass: (currentCol.CssClass || (me.columnsHastable[currentCol.Name].CellCssClass || '')),
                            Width: currentWidth,
                            Align: (currentCol.CellAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'left')),
                            CustomAttr: (currentCol.CustomAttr || ''),
                            ParentHighlight: parentHighlight
                        },
                        currentCol,
                        currentRow,
                        isBody,
                        level
                    );
                frozenRowHtml.push(renderObject.CellHtml);
                if (renderObject.CurrentColumnShowTree == true) {
                    currentCollapseExpandId = renderObject.CurrentCollapseExpandId;
                }
            }
            frozenRowHtml.push('</tr>');

            if (options.showTree == true && currentRow.SubRows) {
                frozenRowHtml.push(me.RenderSubRow(currentRow, false, isBody, 0, currentCollapseExpandId, parentHighlight, level));
            }

            return frozenRowHtml.join('');
        },
        RenderNormalRow: function (currentRow, colHtmlTag, isBody, rowIndex, treeCollapseExpandClass, isForNoData, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var rowHtml = [];
            var colsLength = currentRow.Columns.length;
            var currentCollapseExpandId = '';
            var parentRowClass = '';
            var trHighlightAtt = (options.showHighlightRow && isBody ? (' highlightatt="highlight-att-main-' + parentHighlight + '-' + rowIndex + '"') : '');
            var trClass = (options.showHighlightRow ? 'highlight-row ' : '') + (currentRow.CssClass || '') + ' ' + (treeCollapseExpandClass || '') + ' ' + (options.showDistinctRow ? ((rowIndex + 1) % 2 == 0 ? ' freegrid-even' : '') : '');

            if (colsLength > 0) {
                //// If show grid with tree, then add class to parent row
                if (options.showTree == true && currentRow.SubRows) {
                    parentRowClass = 'parent-row parent-level-' + level;
                }
                rowHtml.push('<tr class="' + trClass + ' ' + parentRowClass + '" ' + (currentRow.CustomAttr || '') + ' ' + trHighlightAtt + '>');

                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    var currentWidth = me.columnsHastable[currentCol.Name].Width;
                    var customSpan = '';
                    var currentHeight = 'height:' + options.bodyRowHeight + 'px;';
                    var currentHeightForComputeCenter = options.bodyRowHeight;
                    if (isValid(currentCol.MergeRowNumber)) {
                        customSpan += ' rowspan="' + currentCol.MergeRowNumber + '" ';
                        currentHeightForComputeCenter *= currentCol.MergeRowNumber;
                    }

                    //// If merge Column, then calculate width for merging
                    if (isValid(currentCol.MergeColNumber)) {
                        customSpan += ' colspan="' + currentCol.MergeColNumber + '" ';
                        currentWidth = Fanex.Grid.Helper.GetColMergeWidth(me.columnsWidth, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber, options.showColumnsShowHide, options.columns);
                    }
                    var renderObject = me.RenderBodyCell({
                        HtmlTag: colHtmlTag,
                        CustomSpan: customSpan,
                        Height: currentHeight
                    }, {
                        IsNoData: isForNoData,
                        ColIndex: colIndex,
                        RowIndex: rowIndex,
                        CssClass: (currentCol.CssClass || (me.columnsHastable[currentCol.Name].CellCssClass || '')),
                        Width: currentWidth,
                        Align: (currentCol.CellAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'left')),
                        CustomAttr: (currentCol.CustomAttr || ''),
                        ParentHighlight: parentHighlight
                    },
                        currentCol,
                        currentRow,
                        isBody,
                        level
                    );
                    rowHtml.push(renderObject.CellHtml);
                    if (renderObject.CurrentColumnShowTree == true) {
                        currentCollapseExpandId = renderObject.CurrentCollapseExpandId;
                    }

                }
                rowHtml.push('</tr>');

                if (options.showTree == true && currentRow.SubRows) {
                    rowHtml.push(me.RenderSubRow(currentRow, true, isBody, 0, currentCollapseExpandId, parentHighlight, level));
                }
            }

            return rowHtml.join('');
        },
        RenderSubRow: function (currentRow, isMain, isBody, rowIndex, currentCollapseExpandId, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var subRowHtml = [];
            var currentSubRows = (isMain == true ? currentRow.SubRows : Fanex.Grid.Helper.CreateRowsForFrozenColumn(currentRow.SubRows, options.numberOfFrozenColumn, me.frozenColsName));
            var subRowsLength = currentSubRows.length;
            level++;
            for (var subRowIndex = 0; subRowIndex < subRowsLength; subRowIndex++) {
                var currentSubRow = currentSubRows[subRowIndex];
                if (isMain == true) {
                    subRowHtml.push(me.RenderNormalRow(currentSubRow, 'td', isBody, subRowIndex, currentCollapseExpandId, false, parentHighlight + '-' + rowIndex + '-' + subRowIndex, level));
                }
                else {
                    subRowHtml.push(me.RenderFrozenRow(currentSubRow, 'td', isBody, subRowIndex, currentCollapseExpandId, false, parentHighlight + '-' + rowIndex + '-' + subRowIndex, level));
                }
            }
            return subRowHtml.join('');
        },
        RenderBodyCell: function (container, cell, currentCol, currentRow, isBody, level) {
            var me = this;
            var options = me.Options;
            var cellHtml = [];
            var currentCollapseExpandId = '';

            var currentValue = currentCol.Value;
            if (me.columnsHastable[currentCol.Name].FormatNumber && !isNaN(currentValue) && currentValue !== '') {
                currentValue = numeral(currentValue).format(me.columnsHastable[currentCol.Name].FormatNumber);
            }

            if (me.columnsHastable[currentCol.Name].FormatDateTime && isValid(currentValue) && currentValue !== '') {
                var momentObject = moment(currentValue);
                if (momentObject.isValid()) {
                    currentValue = momentObject.format(me.columnsHastable[currentCol.Name].FormatDateTime);
                }
            }

            if (me.columnsHastable[currentCol.Name].FormatNegative) {
                if (currentCol.Value < 0) {
                    currentValue = '<span class="format-negative">' + me.columnsHastable[currentCol.Name].FormatNegative.replace('$', currentValue) + '</span>';
                }
            }

            if (me.columnsHastable[currentCol.Name].FormatPositive) {
                if (currentCol.Value > 0) {
                    currentValue = '<span class="format-positive">' + me.columnsHastable[currentCol.Name].FormatPositive.replace('$', currentValue) + '</span>';
                }
            }

            var cellContent = '';
            if (cell.IsNoData == true) {
                cellContent = currentCol.Value;
            }
            else {
                cellContent = typeof currentCol.CustomCellRender == 'function' ? currentCol.CustomCellRender(currentCol, currentValue) : ((typeof (me.columnsHastable[currentCol.Name].CustomCellRender) == 'function' && isBody) ? me.columnsHastable[currentCol.Name].CustomCellRender(currentCol, currentValue) : currentValue);
            }

            //// If show tree, then add icon expand
            var tdClass = 'free-grid-td-' + cell.ColIndex + ' ' + (currentCol.TdClass || (me.columnsHastable[currentCol.Name].TdClass || ''));
            if (options.showTree == true && currentRow.SubRows && me.columnsHastable[currentCol.Name].ShowCollapseExpand == true) {
                tdClass = 'tr-grid-collapse-expand';
            }

            //// For show/hide column
            if (me.columnsHastable[currentCol.Name].CanShowHide && me.columnsHastable[currentCol.Name].CanShowHide == true && (!options.showColumnsShowHide)) {
                tdClass += ' grid-show-hide-column';
            }

            var truncateClass = '';
            var allowTruncate = me.columnsHastable[currentCol.Name].AllowTruncate == true ? true : options.allowTruncate;
            if (allowTruncate == true) {
                truncateClass = ' column-truncate';
            }


            var titleTooltip = '';
            if (typeof (currentCol.Value) == 'string' && (currentCol.Value.length * me.currentFontSize > me.columnsHastable[currentCol.Name].Width) && allowTruncate == true) {
                titleTooltip = ' title="' + currentCol.Value + '" ';
            }

            var isCurrentColShowTree = (options.showTree == true
            && currentRow.SubRows
            && currentRow.SubRows.length > 0 && me.columnsHastable[currentCol.Name].ShowCollapseExpand == true);

            var paddingLeft = '';
            if ((isCurrentColShowTree && level != 0)
            || (level > 0
                && !(currentRow.SubRows && currentRow.SubRows.length > 0)
                && me.columnsHastable[currentCol.Name].ShowCollapseExpand == true)
            || (level == 0 && isCurrentColShowTree
                && (!isValid(currentRow.SubRows) || currentRow.SubRows.length == 0))) {
                var totalPaddingLeft = level * options.treeParentPaddingLeft;
                paddingLeft = 'padding-left:' + totalPaddingLeft + 'px;';
                cell.Width -= totalPaddingLeft;
            }

            cellHtml.push('<' + container.HtmlTag + ' ' + container.CustomSpan + ' class="' + tdClass + '"' + ' style="' + container.Height + '"' + ' >');
            cellHtml.push('<div class="table-cell ' + cell.CssClass + ' ' + truncateClass + '" style="width:' + cell.Width + 'px;' + paddingLeft + '" ' + 'align="' + cell.Align + '" ' + titleTooltip + cell.CustomAttr + '>');

            if (isCurrentColShowTree) {
                currentCollapseExpandId = me.gridId + '_tree_' + cell.ParentHighlight;
                cellHtml.push('<span class="grid-collapse-expand grid-expand ' + currentCollapseExpandId + '-class' + '" id="' + currentCollapseExpandId + '"></span>');
                currentCollapseExpandId += ' child-row';
            }
            cellHtml.push((isBody && options.showNumberColumn && cell.ColIndex == 0 && !cell.IsNoData) ? (cell.RowIndex + 1) : cellContent);
            cellHtml.push('</div></' + container.HtmlTag + '>');

            return {
                CellHtml: cellHtml.join(''),
                CurrentCollapseExpandId: currentCollapseExpandId,
                CurrentColumnShowTree: isCurrentColShowTree
            };
        },
        RenderEmptyRow: function (columnName, columnValue, mergeColumnNumber, cssClass, showOnMainBody) {
            var me = this;
            var options = me.Options;
            var columns = [{
                Name: columnName,
                Value: columnValue,
                MergeColNumber: mergeColumnNumber,
                CssClass: cssClass
            }];
            if (showOnMainBody) {
                columns.push({
                    Name: options.columns[options.numberOfFrozenColumn].Name,
                    Value: Fanex.Grid.Constant.NO_DATA_INFO,
                    MergeColNumber: (options.columns.length - options.numberOfFrozenColumn),
                    CssClass: cssClass
                });
            }

            var emptyRowHtml = this.RenderNormalRow({
                Columns: columns
            }, 'td', true, 0, '', true, '');
            return emptyRowHtml;
        },
        RenderHeaderCell: function (cellInfo, currentColumn) {
            var me = this;
            var options = me.Options;
            var cellHtml = [];
            var cellTag = cellInfo.CellTag || 'td';
            var customSpan = cellInfo.CustomSpan || ' ';
            var cssClass = cellInfo.CssClass || ' ';
            var sortClass = cellInfo.SortClass || ' ';
            var cellWidth = 'width:' + (cellInfo.CellWidth || '200') + 'px;';
            var cellHeight = isValid(cellInfo.CellHeight) ? ('height:' + (cellInfo.CellHeight || options.headerColumnHeight) + 'px;') : ' ';
            var cellId = isValid(cellInfo.CellId) ? ' id="' + cellInfo.CellId + '"' : " ";
            var sort = cellInfo.Sort || false;
            var cellColumnName = cellInfo.CellColumnName || '';
            var showClass = (isValid(cellInfo.CanShowHide) && cellInfo.CanShowHide) ? ('grid-show-hide-column') : '' + cellInfo.TdClass;

            cellHtml.push('<' + cellTag + ' ' + customSpan + ' class="' + showClass + '" >');

            cellHtml.push('<div class="table-cell ' + cssClass + ' ' + sortClass + '" style="' + cellWidth + cellHeight + '" ' + cellId + ' align="' + cellInfo.CellAlign + '">');
            if (sort == true) {
                cellHtml.push('<div class="display-inline-block">');
            }

            if (typeof cellInfo.CustomHeaderRender == 'function') {
                cellHtml.push(cellInfo.CustomHeaderRender(currentColumn));
            }
            else {
                cellHtml.push(cellInfo.HeaderText);
            }

            if (sort == true) {
                var sortDirection = cellInfo.SortDirection || '';
                cellHtml.push('</div><div class="divSorter ' + sortDirection + '" data-columnname="' + cellColumnName + '"> </div>');
            }

            cellHtml.push('</div>')
            cellHtml.push('</' + cellTag + '>');
            return cellHtml.join('');
        },
        RenderBody: function (headerHtml, reRenderBody) {
            var me = this;
            var options = this.Options;
            var bodyHtml = (reRenderBody == true ? '' : (headerHtml + me.RenderCommonHeader('mainbody')));
            var bodyRowsHtml = [];
            var bodyFrozenHtml = [];
            var currentBodyRows = [];
            var currentPageStartIndex = 0;

            //// If show pager, then take approriate data range
            if (options.showPager == true && options.customPager != true) {
                var startIndex = (options.pagerOption.currentPage * options.pagerOption.itemsPerPage);
                var endIndex = ((options.pagerOption.currentPage + 1) * options.pagerOption.itemsPerPage);
                currentBodyRows = options.bodyRows.slice(startIndex, endIndex);
                currentPageStartIndex = startIndex;
            }
            else {
                currentBodyRows = options.bodyRows;
            }

            var bodyLength = currentBodyRows.length;
            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ? Fanex.Grid.Helper.CreateRowsForFrozenColumn(currentBodyRows, options.numberOfFrozenColumn, me.frozenColsName) : [];
            var frozenBodyLength = currentFrozenRows.length;

            if (bodyLength > 0) {
                //// Render Frozen Row and Main Row at the same time
                for (var rowIndex = 0; rowIndex < frozenBodyLength; rowIndex++) {
                    var currentRow = currentBodyRows[rowIndex];
                    var currentFrozenRow = currentFrozenRows[rowIndex];
                    bodyRowsHtml.push(me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                    bodyFrozenHtml.push(me.RenderFrozenRow(currentFrozenRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                }

                //// If MainRow still has item, then continue render
                for (var rowIndex = frozenBodyLength; rowIndex < bodyLength; rowIndex++) {
                    var currentRow = currentBodyRows[rowIndex];
                    bodyRowsHtml.push(me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                }
            }
            else {
                //// Render There is no data, showNoDataOnFrozen
                var emptyData = Fanex.Grid.Constant.NO_DATA_INFO;
                if (options.numberOfFrozenColumn > 0) {

                    if (options.showNoDataOnFrozen) {
                        //// Show No Info on the frozen table
                        bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.columns.length, 'no-data-frozen', false));
                        bodyFrozenHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.numberOfFrozenColumn, 'no-data-frozen', false));
                    }
                    else {
                        //// Show No Info on the main table
                        bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, '', options.numberOfFrozenColumn, 'no-data-frozen', true));
                        bodyFrozenHtml.push(me.RenderEmptyRow(options.columns[0].Name, '', options.numberOfFrozenColumn, '', false));
                    }
                }
                else {
                    //// Show No Info on the main table
                    bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.columns.length, 'no-data-not-frozen', false));
                }
            }

            if (reRenderBody == true) {
                me.$grid.find('#' + me.frozenContentId + '> tbody').empty().html(bodyFrozenHtml.join(''));
                me.$grid.find('#' + me.tableContentId + '> tbody').empty().html(bodyRowsHtml.join(''));
            }
            else {
                // Fixbug for replace $' string
                var bodyHtmlTemp = bodyHtml.split('{$}frozenBody');
                if (bodyHtmlTemp.length == 2) {
                    bodyHtml = bodyHtmlTemp[0] + bodyFrozenHtml.join('') + bodyHtmlTemp[1];
                }
                bodyHtmlTemp = bodyHtml.split('{$}_');
                if (bodyHtmlTemp.length == 2) {
                    bodyHtml = bodyHtmlTemp[0] + bodyRowsHtml.join('') + bodyHtmlTemp[1];
                }
            }

            return Fanex.Grid.Helper.RemoveSpaceIE9(bodyHtml);
        },
        RenderFooter: function () {
            var me = this;
            var options = me.Options;
            var footerHtml = [];
            var frozenFooterHtml = [];

            footerHtml.push('<div class="col_footer row-footer-background scroll-y">' + '<div class="footer-container">' + '<table class="col_footer_content grid-fixed-table">' + '<thead>');

            frozenFooterHtml.push(' <div class="row_footer row-footer-background z999">' + ' <table class="grid-fixed-table row_footer_content">' + '    <thead>');

            var footerRowsLength = options.footerRows.length;
            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ? Fanex.Grid.Helper.CreateRowsForFrozenColumn(options.footerRows, options.numberOfFrozenColumn, me.frozenColsName) : [];
            var frozenFooterLength = currentFrozenRows.length;

            //// Render Frozen fotter and main Footer
            for (var rowIndex = 0; rowIndex < frozenFooterLength; rowIndex++) {
                var currentRow = options.footerRows[rowIndex];
                var currentFrozenRow = currentFrozenRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, 'th', false, rowIndex, '', false, ''));
                frozenFooterHtml.push(me.RenderFrozenRow(currentFrozenRow, 'th', false, rowIndex, '', false, ''));
            }

            //// Render only main Footer
            for (var rowIndex = frozenFooterLength; rowIndex < footerRowsLength; rowIndex++) {
                var currentRow = options.footerRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, 'th', false, rowIndex, '', false, ''));
            }

            frozenFooterHtml.push('   </thead>' + '</table>' + '</div>');

            footerHtml.push(' </thead>' + '</table>' + '</div>' + '</div>');

            return frozenFooterHtml.join('') + footerHtml.join('');
        },
        RenderPager: function () {
            var me = this;
            var options = this.Options;
            var pagerOption = {};
            if (options.bodyRows.length > 0) {
                if (options.customPager && options.customPager == true) {
                    pagerOption =
                    {
                        callback: options.pagerOption.pagerSelectCallBack
                    };
                }
                else {
                    pagerOption =
                    {
                        callback: function (page_index, itemsPerPage, jq) {
                            if (me.Options.pagerOption.itemsPerPage != itemsPerPage) {
                                page_index = 0;
                            }
                            me.Options.pagerOption.currentPage = page_index;
                            me.Options.pagerOption.itemsPerPage = itemsPerPage;
                            me.RenderBody('', true);
                            me.gridOptions.isResetScroller = true;
                            $(me.$grid).gridScroller(me.gridOptions);
                            me.RenderPager();
                            me.RegisterMouseHoverEvent();
                            //// Save for rerender
                            Fanex.Grid.List[me.tableContentId].pagerOption = me.Options.pagerOption;
                            if (typeof (options.pagerOption.afterPagedFunction) == 'function') {
                                options.pagerOption.afterPagedFunction();
                            }
                            return false;
                        }
                    };
                }

                pagerOption.startPerPage = options.pagerOption.startPerPage;
                pagerOption.stepPerPage = options.pagerOption.pageStep;
                pagerOption.maxTtemsPerPage = options.pagerOption.maxItemsPerPage;
                pagerOption.itemsPerPage = options.pagerOption.itemsPerPage;
                pagerOption.currentPage = options.pagerOption.currentPage;
                pagerOption.total_items = options.pagerOption.totalItem;
                pagerOption.itemsPerPageArray = options.pagerOption.itemsPerPageArray;

                this.$grid.find('#pagination').fanexPagination(pagerOption.total_items, pagerOption);
            }
        },
        RegisterEvents: function () {
            var me = this;
            var options = me.Options;
            //// Sorting
            me.RegisterSortEvent();

            //// For Onclick Title Button
            if (options.showTitle == true) {
                if (typeof options.titleInfo.exportExcelFunction == 'function') {
                    me.$grid.parent().off('click', '#btnExport' + me.gridId, null).on('click', '#btnExport' + me.gridId, options.titleInfo.exportExcelFunction);
                }
                if (typeof options.titleInfo.backFunction == 'function') {
                    me.$grid.parent().off('click', '#btnBack' + me.gridId, null).on('click', '#btnBack' + me.gridId, options.titleInfo.backFunction);
                }
                if (typeof options.titleInfo.sendMailFunction == 'function') {
                    me.$grid.parent().off('click', '#btnSendMail' + me.gridId, null).on('click', '#btnSendMail' + me.gridId, options.titleInfo.sendMailFunction);
                }
                if (options.titleInfo.showCollapseExpandButton == true) {
                    me.$grid.parent().off('click', '#btnCollapseExpand' + me.gridId, null).on('click', '#btnCollapseExpand' + me.gridId, function () {
                        var $this = $(this);
                        if ($this.hasClass('expanded')) {
                            me.$grid.find('.child-row').hide();
                            me.$grid.find('.grid-collapse-expand').removeClass('grid-expand').addClass('grid-collapse');
                            $this.removeClass('expanded').addClass('collapsed');
                        }
                        else {
                            $this.removeClass('collapsed').addClass('expanded');
                            me.$grid.find('.grid-collapse-expand').removeClass('grid-collapse').addClass('grid-expand');
                            me.$grid.find('.child-row').show();
                        }
                        me.gridOptions.isResetScroller = false;
                        $(me.$grid).gridScroller(me.gridOptions);
                    });
                }
            }

            //// Bind collapse expand event
            if (options.showTree == true) {
                me.$grid.off('click', '.parent-row > td.tr-grid-collapse-expand', null).on('click', '.parent-row > td.tr-grid-collapse-expand', function () {
                    var $this = $(this);
                    var collapseexpandspan = $this.find('.grid-collapse-expand');
                    var isExpand = false;
                    if (collapseexpandspan.hasClass('grid-expand')) {
                        me.$grid.find('.' + collapseexpandspan.attr('id') + '-class').removeClass('grid-expand').addClass('grid-collapse');
                        isExpand = false;
                    }
                    else {
                        me.$grid.find('.' + collapseexpandspan.attr('id') + '-class').removeClass('grid-collapse').addClass('grid-expand');
                        isExpand = true;
                    }
                    var $child = me.$grid.find('.' + collapseexpandspan.attr('id'));
                    if ($child.length > 0) {
                        $child.toggle();
                        for (var index = 0; index < $child.length; index++) {
                            var $currentChild = $($child[index]);
                            RecursiveCollapseExpandChild($currentChild, isExpand);
                        }
                        me.gridOptions.isResetScroller = false;
                        $(me.$grid).gridScroller(me.gridOptions);
                    }
                });

                function RecursiveCollapseExpandChild($current, isExpand) {
                    var currentStatus = '';
                    var collapseexpandspan = $current.find('.grid-collapse-expand');
                    var $child = me.$grid.find('.' + collapseexpandspan.attr('id'));

                    if (isExpand) {
                        if (collapseexpandspan.hasClass('grid-expand')) {
                            $child.show();
                        }
                        else {
                            $child.hide();
                        }
                    }
                    else {
                        $child.hide();
                    }

                    for (var index = 0; index < $child.length; index++) {
                        var $currentChild = $($child[index]);
                        RecursiveCollapseExpandChild($currentChild, isExpand);
                    }
                };
            }
        },
        RegisterSortEvent: function () {
            var me = this;
            var options = me.Options;
            //// For Sorting
            var colsLength = options.columns.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                var currentCol = options.columns[colIndex];
                if (currentCol.Sort == true) {
                    me.$grid.off('click', '#' + me.gridId + '_header_sort_' + currentCol.Name, null).on('click', '#' + me.gridId + '_header_sort_' + currentCol.Name, function () {
                        var $this = $(this).find('.divSorter');
                        var currentDirection = '';
                        if ($this.hasClass('desc')) {
                            me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                            $this.addClass('asc');
                            currentDirection = 'ASC';
                        }
                        else if ($this.hasClass('asc')) {
                            me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                            $this.addClass('desc');
                            currentDirection = 'DESC';
                        }
                        else {
                            me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                            $this.addClass('desc');
                            currentDirection = 'DESC';
                        }
                        var customSortFunction = null;
                        for (var colIndex = 0; colIndex < options.columns.length; colIndex++) {
                            if (options.columns[colIndex].Name == $this.data('columnname')) {
                                options.columns[colIndex].SortDirection = currentDirection.toLowerCase();
                                customSortFunction = options.columns[colIndex].CustomSortFunction
                            }
                            else {
                                options.columns[colIndex].SortDirection = '';
                            }
                        }
                        //customGridSort: null
                        if (typeof (options.sortOption.serverSortFunction) == 'function') {
                            options.sortOption.serverSortFunction($this.data('columnname'), currentDirection);
                        }
                        else {
                            if (typeof (options.sortOption.beforeGridSorted) == 'function') {
                                options.bodyRows = options.sortOption.beforeGridSorted(options.bodyRows, $this.data('columnname'), currentDirection);
                            }
                            if (typeof (options.sortOption.customGridSort) == 'function') {
                                options.bodyRows = options.sortOption.customGridSort(options.bodyRows, $this.data('columnname'), currentDirection, customSortFunction);
                            }
                            else {
                                options.bodyRows = Fanex.Grid.Helper.GetSortedArray(options.bodyRows, $this.data('columnname'), currentDirection, customSortFunction);
                            }
                            if (typeof (options.sortOption.afterGridSorted) == 'function') {
                                options.bodyRows = options.sortOption.afterGridSorted(options.bodyRows, $this.data('columnname'), currentDirection);
                            }
                            me.ReRenderBodyAfterSort($this.data('columnname'), currentDirection);
                        }
                    });
                }
            }
        },
        RegisterMouseHoverEvent: function () {
            var me = this;
            var options = me.Options;
            //// For Mouse hover event
            if (options.showHighlightRow == true && options.numberOfFrozenColumn > 0) {
                me.$grid.find('.highlight-row').unbind('mouseenter mouseleave').mouseenter(function () {
                    var $this = $(this);
                    var hlattr = $this.attr('highlightatt');
                    if (isValid(hlattr)) {
                        if (hlattr.indexOf('frozen') > 0) {
                            me.$grid.find('tr[highlightatt="' + hlattr.replace('frozen', 'main') + '"]').addClass('mousehoverbeyond');
                        }
                        else {
                            me.$grid.find('tr[highlightatt="' + hlattr.replace('main', 'frozen') + '"]').addClass('mousehoverbeyond');
                        }
                    }
                }).mouseleave(function () {
                    var $this = $(this);
                    var hlattr = $this.attr('highlightatt');
                    if (isValid(hlattr)) {
                        if (hlattr.indexOf('frozen') > 0) {
                            me.$grid.find('tr[highlightatt="' + hlattr.replace('frozen', 'main') + '"]').removeClass('mousehoverbeyond');
                        }
                        else {
                            me.$grid.find('tr[highlightatt="' + hlattr.replace('main', 'frozen') + '"]').removeClass('mousehoverbeyond');
                        }
                    }
                });
            }
        }
    };

    //// Grid Helper 
    Fanex.Grid.Helper =
    {
        RemoveItemForRowSpan: function (rows, currentRowIndex, mainColumn, colsHashtable) {
            var rowSpanSize = mainColumn.MergeRowNumber;
            var columnName = mainColumn.Name;
            var currentLength = currentRowIndex + rowSpanSize;
            var rowsLength = currentLength < rows.length ? currentLength : rows.length;
            for (var rowIndex = currentRowIndex + 1; rowIndex < rowsLength; rowIndex++) {
                for (var colIndex = 0; colIndex < rows[rowIndex].Columns.length; colIndex++) {
                    var currentCol = rows[rowIndex].Columns[colIndex];
                    if (currentCol.Name == columnName) {
                        if (isValid(currentCol.Sort) && currentCol.Sort == true) {
                            var firstColIndex = Fanex.Grid.Helper.GetColumnIndexFromColumnName(rows[currentRowIndex], currentCol.Name);
                            rows[currentRowIndex].Columns[firstColIndex].Sort = true;
                            rows[currentRowIndex].Columns[firstColIndex].SortDirection = currentCol.SortDirection;
                        }


                        if (isValid(mainColumn.MergeColNumber)) {
                            var mergeColLength = mainColumn.MergeColNumber + colIndex;
                            mergeColLength = (mergeColLength <= rows[rowIndex].Columns.length ? mergeColLength : 0);
                            for (var mergeColIndex = colIndex + 1; mergeColIndex < mergeColLength; mergeColIndex++) {
                                rows[rowIndex].Columns.remove(mergeColIndex);
                            }
                        }
                        rows[rowIndex].Columns.remove(colIndex);
                    }
                }
            }
        },
        GetColumnIndexFromColumnName: function (row, columnName) {
            for (var index = 0; index < row.Columns.length; index++) {
                if (row.Columns[index].Name == columnName) {
                    return index;
                }
            }
            return -1;
        },
        CreateRowsForFrozenColumn: function (mainRows, frozenColNumber, frozenColsName) {
            var frozenRows = [];
            var rowLength = mainRows.length;

            for (var rowIndex = 0; rowIndex < rowLength; rowIndex++) {
                var currentRow = mainRows[rowIndex];
                var newRow = clone(currentRow);
                newRow.Columns = [];
                var isset = false;
                var colsLength = currentRow.Columns.length < frozenColNumber ? currentRow.Columns.length : frozenColNumber;

                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    if (Fanex.Grid.Helper.CheckExistName(frozenColsName, currentCol.Name)) {
                        newRow.Columns.push(clone(currentRow.Columns[colIndex]));
                        isset = true;
                    }
                    else if (isValid(prevCol) && isValid(prevCol.MergeColNumber) && prevCol.MergeColNumber + colIndex <= colsLength) { }
                    var prevCol = currentCol;
                }
                if (isset) {
                    frozenRows.push(newRow);
                }
            }

            return frozenRows;
        },
        CheckExistName: function (frozenColsName, name) {
            for (var colIndex = 0; colIndex < frozenColsName.length; colIndex++) {
                if (frozenColsName[colIndex] == name) {
                    return true;
                }
            }
            return false;
        },
        CheckHideColSpan: function (currentFrozenColums) {
            var colsLength = currentFrozenColums.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                if (!isValid(currentFrozenColums[colIndex].MergeRowNumber)) {
                    return false;
                }
            }
            return true;
        },
        GetArrayWidth: function (columns, showColumnsShowHide, isForComputingWidth) {
            var arrayWidth = [];
            var columnLength = columns.length;
            for (var columnIndex = 0; columnIndex < columnLength; columnIndex++) {
                var currentColumn = columns[columnIndex];
                if (isValid(showColumnsShowHide) && showColumnsShowHide == false && currentColumn.CanShowHide && currentColumn.CanShowHide == true && isForComputingWidth == false) { }
                else {
                    arrayWidth.push(currentColumn.Width);
                }
            }
            return arrayWidth;
        },
        GetColMergeWidth: function (arrayWidth, startIndex, colMergeNumber, showColumnsShowHide, columns) {
            var colWidth = 0;
            var finalColMergeNumber = colMergeNumber;
            var colLength = arrayWidth.length < (startIndex + colMergeNumber) ? arrayWidth.length : (startIndex + colMergeNumber);
            for (var colIndex = startIndex; colIndex < colLength; colIndex++) {
                if (isValid(showColumnsShowHide) && showColumnsShowHide == false && columns[colIndex].CanShowHide && columns[colIndex].CanShowHide == true) {
                    finalColMergeNumber--;
                }
                else {
                    colWidth += arrayWidth[colIndex];
                }
            }
            colWidth += ((finalColMergeNumber - 1) * 5);
            return colWidth;
        },
        GetColumnWidth: function (columns, name) {
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                if (columns[colIndex].Name == name) {
                    return columns[colIndex].Width;
                }
            }
            return 0;
        },
        GetColumnsHastable: function (columns) {
            var hastable = {};
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                var currentCol = columns[colIndex];
                hastable[currentCol.Name] = currentCol;
                hastable[currentCol.Name].ColumnIndex = colIndex;
            }
            return hastable;
        },
        GetSortedArray: function (rows, sortField, sortDirection, customSortFunction) {
            if (sortDirection == 'ASC') {
                return rows.sort(function (row1, row2) {
                    return Fanex.Grid.Helper.CompareRow(row1, row2, sortField, function (col1Value, col2Value) {
                        if (typeof (customSortFunction) == 'function') {
                            return customSortFunction(col1Value, col2Value, sortDirection);
                        }
                        return col1Value > col2Value ? 1 : -1;
                    });
                });
            }
            else {
                return rows.sort(function (row1, row2) {
                    return Fanex.Grid.Helper.CompareRow(row1, row2, sortField, function (col1Value, col2Value) {
                        if (typeof (customSortFunction) == 'function') {
                            return customSortFunction(col1Value, col2Value, sortDirection);
                        }
                        return col1Value < col2Value ? 1 : -1;
                    });
                });
            }
        },
        CompareRow: function (row1, row2, sortField, compareFunction) {
            var col1Length = row1.Columns.length;
            var col2Length = row2.Columns.length;
            var col1Value = "";
            var col2Value = "";

            for (var col1Index = 0; col1Index < col1Length; col1Index++) {
                if (row1.Columns[col1Index].Name == sortField) {
                    col1Value = row1.Columns[col1Index].Value;
                    break;
                }
            }
            for (var col2Index = 0; col2Index < col2Length; col2Index++) {
                if (row2.Columns[col2Index].Name == sortField) {
                    col2Value = row2.Columns[col2Index].Value;
                    break;
                }
            }

            //            if (typeof (col1Value) == 'string' && typeof (col2Value) == 'string') {
            //                col1Value = parseFloat(col1Value.replace(',', ''));
            //                col2Value = parseFloat(col2Value.replace(',', ''));
            //            }

            return compareFunction(col1Value, col2Value);
        },
        SetColumnDisplay: function (columns, showHideArray) {
            var columnsLengh = columns.length;
            for (var colIndex = 0; colIndex < columnsLengh; colIndex++) {
                columns[colIndex].CanShowHide = false;
            }
            var showhidelength = showHideArray.length;
            for (var index = 0; index < showhidelength; index++) {
                columns[showHideArray[index]].CanShowHide = true;
            }
            return columns;
        },
        ComputeNewFrozenColumn: function (showhideArray, frozenColumnNumber) {
            var currentFrozenNumber = frozenColumnNumber;
            for (var index = 0; index < showhideArray.length; index++) {
                if (showhideArray[index] < currentFrozenNumber) {
                    frozenColumnNumber--;
                }
            }
            return frozenColumnNumber;
        },
        HideHeaderGroupWhenShowHide: function (hideColumns, startIndex, mergeColNumber) {
            var length = startIndex + mergeColNumber;
            for (var index = startIndex; index < length; index++) {
                if (!hideColumns.contains(index)) {
                    return false;
                }
            }
            return true;
        },
        ReComputeMergeColumAfterShowHide: function (hideColumns, startIndex, mergeColNumber) {
            var currentMergeCol = mergeColNumber;
            var length = startIndex + mergeColNumber;
            for (var index = startIndex; index < length; index++) {
                if (hideColumns.contains(index)) {
                    currentMergeCol--;
                }
            }
            return currentMergeCol;
        },
        UpdateSortInforAfterShowHideColumns: function (options, sortOption) {
            if (isValid(sortOption)) {
                var colsLength = options.columns.length;
                var customSortFunction = null;
                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    if (options.columns[colIndex].Name == sortOption.SortColumnName) {
                        options.columns[colIndex].SortDirection = sortOption.SortDirection.toLowerCase(); ;
                        customSortFunction = options.columns[colIndex].CustomSortFunction
                    }
                }
                options.bodyRows = Fanex.Grid.Helper.GetSortedArray(options.bodyRows, sortOption.SortColumnName, sortOption.SortDirection, customSortFunction);
            }
        },
        RemoveSpaceIE9: function (html) {
            if ($.browser.msie && parseInt($.browser.version, 10) === 9 && html != undefined) {
                var expr = new RegExp('>[ \t\r\n\v\f]*<', 'g');
                html = html.replace(expr, '><');
            }

            return html;
        },
        CreateHeaderColumnsFromOptionsColumn: function (columns) {
            var cloneColumns = columns.slice(0);
            var colLength = columns.length;
            var colArray = [];
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                var currentCol = clone(columns[colIndex]);
                var currentWidth = currentCol.Width;
                if (isValid(currentCol.MergeHeaderColNumber) && currentCol.MergeHeaderColNumber > 0) {
                    var currentMaxLength = currentCol.MergeHeaderColNumber + colIndex;
                    colIndex++;
                    for (; colIndex < currentMaxLength && colIndex < colLength; colIndex++) {
                        currentWidth += columns[colIndex].Width + 5;
                    }
                    colIndex--;
                }
                currentCol.Width = currentWidth;
                colArray.push(currentCol);
            }
            return colArray;
        }
    };

    Fanex.Grid.List = {};

    Fanex.Grid.Constant =
    {
        NO_DATA_INFO: 'There is no data'
    };

    $.fn.gridScroller = function (opts) {
        return this.each(function () {
            var o = $(this);
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".container");
            var rowheader = o.find(".row_header");
            o.parent().removeClass('display-none');
            o.parent().find('.grid-title').removeClass('display-none');
            var options = $.extend({}, $.fn.renderScroller.defaults, opts);
            var timeOutResize = null;


            if (!isValid(Fanex.Grid.List[options.tblContentId])) {
                $(window).resize(function () {
                    if (!o.hasClass("display-none")) {
                        o.resetGrid(options.tblContentId);
                        var opt = $.extend({}, {}, Fanex.Grid.List[options.tblContentId].gridOptions);
                        opt.manualCopyHeader = true;
                        o.renderScroller(opt);
                        o.find('.grid-fixed-table').append(' ');
                        //console.log("resize-" + options.tblContentId);
                    }
                });
                Fanex.Grid.List[options.tblContentId] =
                {
                    gridOptions: options,
                    pagerOption: null,
                    sortOption: null
                };
            }
            else {
                Fanex.Grid.List[options.tblContentId].gridOptions = options;
            }

            o.renderScroller(options);
            o.find('.grid-fixed-table').append(' ');

            if (options.isResetScroller) {
                o.resetScroller(o, container, rowheader, xscrollbar, yscrollbar);
            }
            else {
                var top = yscrollbar.scrollTop();
                container.prop("scrollTop", top);
                rowheader.prop("scrollTop", top);
            }

            return new scroller(o, container, rowheader, xscrollbar, yscrollbar);
        });
    };

    $.fn.initGrid = function (options) {
        return this.each(function () {
            var o = $(this);
            //Render Free Cell
            var freeCellTpl = '<div class="free-cell free-bottom-left">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-bottom-left">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>\
                <div class="free-cell free-top-right">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-top-right">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>\
                <div class="free-cell free-bottom-right">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-bottom-right">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>';
            o.append(freeCellTpl);
            //Render Scrollbar
            var scrollbarTpl = '<div class="y-scrollbar">\
                    <div class="scrollbar-y-content">\
                        &nbsp;</div>\
                </div>\
                <div class="x-scrollbar">\
                    <div class="scrollbar-x-content">\
                        &nbsp;</div>\
                </div>';
            o.append(scrollbarTpl);
            if (options.isShowPager) {
                var pagerTpl = '<div class="grid-pager">\
                                <div class="grid-pager-content">\
                                    <div id="pagination">&nbsp;</div>\
                                </div>\
                            </div>';
                o.append(pagerTpl);
            }
        });
    };

    $.fn.resetGrid = function () {
        return this.each(function (id) {
            var o = $(this);
            var xscrollbar = o.find(".x-scrollbar");
            var container = o.find(".container");
            var tblContent = $("#" + id);
            var colHeaderContainer = o.find(".col_header");
            var colFooterContainer = o.find(".col_footer");

            o.css("width", "");
            container.css("width", "");
            o.find(".content-container").css("width", "")
            xscrollbar.css("width", "");
            colHeaderContainer.css("width", "");
            colFooterContainer.css("width", "");
            colHeaderContainer.find(".header-container").css("width", "");
            o.find(".footer-container").css("width", "");
            o.find(".scrollbar-x-content").css("width", "");
            o.find(".free-cell-bottom-left").css("width", "");
        });
    };

    $.fn.resetScroller = function (o, container, rowheader, xscrollbar, yscrollbar) {
        yscrollbar.prop("scrollTop", 0);
        container.prop("scrollTop", 0);
        rowheader.prop("scrollTop", 0);

        container.prop("scrollLeft", 0);
        xscrollbar.prop("scrollLeft", 0);
        o.find(".col_header").prop("scrollLeft", 0);
        o.find(".col_footer").prop("scrollLeft", 0);
    };

    $.fn.renderScroller = function (options) {
        return this.each(function () {
            var bar = 17;
            var o = $(this);
            var title = o.find(".grid-title");
            var titleHeight = (!title.hasClass('display-none') && title.length > 0) ? title.height() : 0;
            var pager = o.find(".grid-pager");
            var pagerHeight = pager.length > 0 ? pager.height() : 0;
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".container");
            var tblContent = o.find("#" + options.tblContentId);

            var freeCellTop = o.find(".free-top-right");
            var freeCellBottom = o.find(".free-bottom-right");

            var colHeaderContainer = o.find(".col_header");
            var rowHeaderContainer = o.find(".row_header");
            var rowHeaderWarpper = o.find(".row_header_wrapper");
            var colFooterContainer = o.find(".col_footer");
            var rowFooterContainer = o.find(".row_footer");
            var isShowFooter = !colFooterContainer.hasClass('display-none') && colFooterContainer.find("thead tr").length > 0;

            var numOfFreezeCol = options.numberFreezeCol;
            var col = options.column.length;
            if (col > 0) {
                options.gridWidth = 0;
                options.freezeWidth = 0;
                for (i = 0; i < col; i++) {
                    var colw = options.column[i];
                    options.gridWidth += colw;
                    if (i < numOfFreezeCol) {
                        options.freezeWidth += colw;
                    }
                }

                // (padding + border + padding) * col
                //options.gridWidth += (options.padding + 1 + options.padding) * col;
                options.gridWidth += (options.padding + 1 + options.padding) * col;
                options.freezeWidth += (numOfFreezeCol - 1) * options.padding * 2 + numOfFreezeCol - 1;
            }

            var tblWidth = Math.max(tblContent.width(), options.gridWidth);
            var freezeWidth = options.freezeWidth;

            var parentWidth = o.parent().width();
            var browserWindowWidth = $(window).width();
            if (parentWidth > browserWindowWidth) {
                parentWidth = browserWindowWidth;
            }

            //var gridWidth = options.fullWidth ? (($(window).width() < options.minWidth ? options.minWidth : $(window).width()) - options.marginRight) : options.gridWidth;
            var gridWidth = options.fullWidth ? ((parentWidth < options.minWidth ? options.minWidth : parentWidth) - options.marginRight) : options.gridWidth;
            var isScrollX = gridWidth < tblWidth + 17;
            gridWidth = isScrollX ? gridWidth : (tblWidth + bar);

            o.css("width", gridWidth + "px");
            o.find(".content-container").css("width", tblWidth + "px");
            o.find(".col_header_content").css("width", tblWidth + "px");
            container.css("width", gridWidth + "px");
            xscrollbar.css("width", (gridWidth - freezeWidth - bar - (options.padding) * 2 - 1) + "px");
            colHeaderContainer.css("width", gridWidth + "px");
            colHeaderContainer.find(".header-container").css("width", tblWidth + "px");
            o.find(".footer-container").css("width", tblWidth + "px");
            o.find(".scrollbar-x-content").css("width", (tblWidth - freezeWidth - (isOnChromeBrowser ? 5 : 0)) + "px");
            o.find(".free-cell-bottom-left").css("width", freezeWidth + "px");
            pager.css("width", gridWidth + "px");
            rowHeaderContainer.css({
                "width": (freezeWidth + 5 + 17 + options.highlightFrozenWidth) + "px"
            });
            rowHeaderWarpper.css({
                "width": (freezeWidth + 5 + options.highlightFrozenWidth) + "px"
            });

            var tblHeight = tblContent.height();
            var freezeHeight = tblContent.find("thead").height();
            //var freezeFooterHeight = isShowFooter ? colFooterContainer.find("thead").height() + 3 : 0;
            var freezeFooterHeight = isShowFooter ? colFooterContainer.find("thead").height() + options.bottomBorderWidth : 0;
            var gridHeight = options.fullHeight ? ($(window).height() - options.gridHeight) : options.gridHeight;
            var isScrollY = gridHeight - titleHeight - pagerHeight - bar - freezeFooterHeight < tblHeight;
            gridHeight = isScrollY ? gridHeight : tblHeight + titleHeight + pagerHeight + bar + freezeFooterHeight;

            if (options.hideYBackGroundScroller) {
                o.css("width", (gridWidth - (!isScrollY && !isScrollX ? 18 : 0)) + "px");
                pager.css("width", (gridWidth - ((!isScrollY && !isScrollX ? 18 : 0) ? 18 : 0)) + "px");
            }

            var containerHeight = gridHeight - titleHeight - freezeFooterHeight - pagerHeight - bar;
            o.css("height", (gridHeight - 1 - (isScrollX ? -1 : bar)) + "px");
            rowHeaderContainer.css("height", containerHeight + "px");
            container.css("height", containerHeight + "px");
            rowHeaderWarpper.css("height", containerHeight + "px");
            yscrollbar.css("height", (containerHeight - freezeHeight - (ub.chrome || ub.msie ? 1 : 0) - options.heightScrollYAdd) + "px");
            o.find(".free-cell-top-right").css("height", (freezeHeight - ((ub.msie || ub.chrome || ff16) ? (options.padding * 2 + 1) : 0) + (ub.chrome ? 1 : 0)) + "px");
            if (isShowFooter) {
                o.find(".free-cell-bottom-right").css("height", (freezeFooterHeight + (isScrollX ? bar : 0) + 1) + "px");
            }
            freeCellBottom.css("top", (gridHeight - pagerHeight - freezeFooterHeight - bar - (!isShowFooter ? 1 : 0)) + "px");
            o.find(".scrollbar-y-content").css("height", (tblHeight - freezeHeight) + "px");

            yscrollbar.css("top", (freezeHeight + titleHeight - (ub.chrome ? (colHeaderContainer.find("thead tr").length - 2) : 0) + options.heightScrollYAdd) + "px");
            xscrollbar.css("top", (gridHeight - pagerHeight - bar - 1 + extraPosition) + "px");
            if (numOfFreezeCol > 0) {
                o.find(".free-bottom-left").css("top", (gridHeight - pagerHeight - bar - 1 + extraPosition) + "px");
            }
            else {
                o.find(".free-bottom-left").remove();
            }

            rowFooterContainer.css({
                "top": (gridHeight - pagerHeight - freezeFooterHeight - bar) + "px",
                "border-top-width": options.bottomBorderWidth + "px"
            });
            colFooterContainer.css({
                "width": gridWidth + "px",
                "top": (gridHeight - pagerHeight - freezeFooterHeight - bar) + "px",
                "border-top-width": options.bottomBorderWidth + "px"
            });
            //pager.css("top", (gridHeight - pagerHeight - (isScrollX ? -(isShowFooter ? 0 : 1) : bar) - (isShowFooter ? 1 : 1)) + "px");
            pager.css("top", (gridHeight - pagerHeight - (isScrollX ? -(isShowFooter ? 0 : 1) : bar) - 1 + extraPosition * 2) + "px");

            xscrollbar.css("left", (freezeWidth + (options.padding) * 2 + 1 - extraPosition) + "px");
            yscrollbar.css("left", (gridWidth - bar - 1 + extraPosition) + "px");
            freeCellTop.css("left", (gridWidth - bar - 1 + extraPosition) + "px");
            freeCellBottom.css("left", (gridWidth - bar - 1) + "px");

            if (isScrollY) {
                yscrollbar.css("overflow-y", "scroll");
            }
            else {
                yscrollbar.css("overflow-y", "hidden");
            }

            if (isScrollX) {
                xscrollbar.css("overflow-x", "scroll");
            }
            else {
                xscrollbar.css("overflow-x", "hidden");
            }

            ////Recalculate Title Width
            var gridTitle = o.parent().find(".grid-title");
            if (gridTitle.length > 0) {
                gridTitle.width(o.width());
            }
            //// Fixbug show scroller on row header on chrome
            if (isOnChromeBrowser) {
                var currentRowHeaderStyle = rowHeaderContainer.attr('style');
                rowHeaderContainer.css("overflow", "hidden !important");
            }
        });
    };

    $.fn.renderScroller.defaults =
    {
        tblContentId: 'grid1',
        isUseLeftTable: false,
        isShowPager: false,
        column: [],
        numberFreezeCol: 2,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 0,
        //=totalColumnWidth + totalColumn*4 +totalColumn
        minWidth: 600,
        gridHeight: 300,
        freezeWidth: 241,
        //=totalColumnWidth + (totalColumn-1)*2 + (totalColumn-1)
        padding: 2,
        marginRight: 20,
        isResetScroller: true,
        highlightFrozenWidth: 0,
        heightScrollYAdd: 0,
        hideYBackGroundScroller: true,
        bottomBorderWidth: 3
    };

    var scroller = function (obj, container, rowheader, xscrollbar, yscrollbar) {
        this.preLeft = xscrollbar.scrollLeft();
        this.preTop = yscrollbar.scrollTop();
        this.colHeaderContainer = $(obj).find(".col_header");
        this.rowHeaderContainer = $(obj).find(".row_header");
        this.colFooterContainer = $(obj).find(".col_footer");
        this.init(container, rowheader, xscrollbar, yscrollbar);
    };

    scroller.prototype =
    {
        init: function (container, rowheader, xscrollbar, yscrollbar) {
            var me = this;
            yscrollbar.scroll(function () {
                var top = yscrollbar.scrollTop();
                if (me.preTop != top) {
                    container.prop("scrollTop", top);
                    me.rowHeaderContainer.prop("scrollTop", top);
                    me.preTop = top;
                }
            });

            container.scroll(function () {
                var top = container.scrollTop();
                if (me.preTop != top) {
                    me.rowHeaderContainer.prop("scrollTop", top);
                    yscrollbar.prop("scrollTop", top);
                    me.preTop = top;
                }
                //
                if (isOnSmartPhoneBrowser) {
                    var left = container.scrollLeft();
                    if (me.preLeft != left) {
                        me.colHeaderContainer.prop("scrollLeft", left);
                        me.colFooterContainer.prop("scrollLeft", left);
                        xscrollbar.prop("scrollLeft", top);
                        me.preLeft = left;
                    }
                }
            });

            rowheader.scroll(function () {
                var top = rowheader.scrollTop();
                if (me.preTop != top) {
                    container.prop("scrollTop", top);
                    yscrollbar.prop("scrollTop", top);
                    me.preTop = top;
                }
            });

            xscrollbar.scroll(function () {
                var left = xscrollbar.scrollLeft();
                if (me.preLeft != left) {
                    me.colHeaderContainer.prop("scrollLeft", left);
                    me.colFooterContainer.prop("scrollLeft", left);
                    container.prop("scrollLeft", left);
                    me.preLeft = left;
                }
            });

            if (isOnSmartPhoneBrowser) {
                me.colHeaderContainer.scroll(function () {
                    var left = me.colHeaderContainer.scrollLeft();
                    if (me.preLeft != left) {
                        container.prop("scrollLeft", left);
                        me.colFooterContainer.prop("scrollLeft", left);
                        xscrollbar.prop("scrollLeft", top);
                        me.preLeft = left;
                    }
                });
                me.colFooterContainer.scroll(function () {
                    var left = me.colFooterContainer.scrollLeft();
                    if (me.preLeft != left) {
                        container.prop("scrollLeft", left);
                        me.colHeaderContainer.prop("scrollLeft", left);
                        xscrollbar.prop("scrollLeft", top);
                        me.preLeft = left;
                    }
                });
            }
        }
    };
})(jQuery);



// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};

function isValid(obj) {
    if (typeof obj != "undefined" && obj != null) {
        return true;
    }
    return false;
}

function clone(obj) {
    var target = {};
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            target[i] = obj[i];
        }
    }
    return target;
}

Array.prototype.clone = function () {
    return this.slice(0);
};

Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}


//----------------------------------------------
// Search Devives

var deviceIphone = "iphone";
var deviceIpod = "ipod";
var deviceIpad = "ipad";
var deviceS60 = "series60";
var deviceSymbian = "symbian";
var engineWebKit = "webkit";
var deviceAndroid = "android";
var deviceBB = "blackberry";
var devicePalm = "palm";
var deviceWinMob = "windows ce";

var uagent = navigator.userAgent.toLowerCase();
var currentDevice = "";
function DetectSmartPhone() {
    if (uagent.indexOf(deviceIphone) > -1) {
        currentDevice = deviceIphone;
        return true;
    }
    else if (uagent.indexOf(deviceIpod) > -1) {
        currentDevice = deviceIpod;
        return true;
    }
    else if (uagent.indexOf(deviceIpad) > -1) {
        currentDevice = deviceIpad;
        return true;
    }
    else if (uagent.indexOf(deviceS60) > -1) {
        currentDevice = deviceS60;
        return true;
    }
    else if (uagent.indexOf(deviceSymbian) > -1) {
        currentDevice = deviceSymbian;
        return true;
    }
    else if (uagent.indexOf(engineWebKit) > -1) {
        currentDevice = engineWebKit;
        return true;
    }
    else if (uagent.indexOf(deviceAndroid) > -1) {
        currentDevice = deviceAndroid;
        return true;
    }
    else if (uagent.indexOf(deviceBB) > -1) {
        currentDevice = deviceBB;
        return true;
    }
    else if (uagent.indexOf(devicePalm) > -1) {
        currentDevice = devicePalm;
        return true;
    }
    else if (uagent.indexOf(deviceWinMob) > -1) {
        currentDevice = deviceWinMob;
        return true;
    }
    else
        return false;
}

var isOnSmartPhoneBrowser = DetectSmartPhone();
var isOnChromeBrowser = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
var isOnFireFox4 = null;