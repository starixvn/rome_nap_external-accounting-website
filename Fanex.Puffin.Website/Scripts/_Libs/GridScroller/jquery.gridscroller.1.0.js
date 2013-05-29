var ub = $.browser;
ub.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
var ff16 = $.browser.mozilla && parseInt($.browser.version, 10) >= 16;

(function ($) {
    $.fn.gridScroller = function (opts) {
        return this.each(function () {
            var o = $(this);
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".container");
            var rowheader = o.find(".row_header");
            o.removeClass('display-none');
            o.find('.grid-title').removeClass('display-none');
            var options = $.extend({}, $.fn.renderScroller.defaults, opts);

            $(window).resize(function () {
                if (!o.hasClass("display-none")) {
                    o.resetGrid(options.tblContentId);
                    var opt = $.extend({}, {}, options);
                    opt.manualCopyHeader = true;
                    o.renderScroller(opt);
                    o.find('.grid-fixed-table').append(' ');
                }
            });
            o.renderScroller(options);
            o.find('.grid-fixed-table').append(' ');
            o.resetScroller(o, container, rowheader, xscrollbar, yscrollbar);
           
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
          container.prop("scrollLeft", 0);
          
          rowheader.prop("scrollTop", 0);
          o.find(".col_header").prop("scrollLeft", 0);
          o.find(".col_footer").prop("scrollLeft", 0);
     }

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
            var col=options.column.length;
            if (col>0){
                options.gridWidth=0;
                options.freezeWidth=0;
                for (i=0;i<col;i++)
                {
                    var colw=options.column[i];
                    options.gridWidth+=colw;
                    if (i<numOfFreezeCol)
                    {
                        options.freezeWidth+=colw;
                    }
                }

                // (padding + border + padding) * col
                options.gridWidth+= (options.padding + 1 + options.padding) * col;
                options.freezeWidth += (numOfFreezeCol - 1) * options.padding*2 + numOfFreezeCol - 1;
            }
            
            var tblWidth = Math.max(tblContent.width(), options.gridWidth);
            var freezeWidth = options.freezeWidth;

            var gridWidth = options.fullWidth ? (($(window).width() < options.minWidth ? options.minWidth : $(window).width()) - options.marginRight) : options.gridWidth;
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
            o.find(".scrollbar-x-content").css("width", (tblWidth - freezeWidth) + "px");
            o.find(".free-cell-bottom-left").css("width", freezeWidth + "px");
            pager.css("width", gridWidth  + "px");
            rowHeaderContainer.css({ "width": (freezeWidth + 5 + 17) + "px" });
            rowHeaderWarpper.css({ "width": (freezeWidth + 5) + "px" });

            var tblHeight = tblContent.height();
            var freezeHeight = tblContent.find("thead").height();
            var freezeFooterHeight = isShowFooter ? colFooterContainer.find("thead").height() + 3 : 0;
            var gridHeight = options.fullHeight ? ($(window).height() - options.gridHeight) : options.gridHeight;
            var isScrollY = gridHeight - titleHeight - pagerHeight - bar - freezeFooterHeight < tblHeight;
            gridHeight = isScrollY ? gridHeight : tblHeight + titleHeight + pagerHeight + bar + freezeFooterHeight;

            o.css("width", (gridWidth - (!isScrollY && !isScrollX ? 18 : 0)) + "px");
            pager.css("width", (gridWidth - ((!isScrollY && !isScrollX ? 18 : 0) ? 18 : 0)) + "px");

            var containerHeight = gridHeight - titleHeight - freezeFooterHeight - pagerHeight - bar;
            o.css("height", (gridHeight - 1 - (isScrollX ? -1 : bar)) + "px");
            rowHeaderContainer.css("height", containerHeight + "px");
            container.css("height", containerHeight + "px");
            rowHeaderWarpper.css("height", containerHeight + "px");
            yscrollbar.css("height", (containerHeight - freezeHeight - (ub.chrome || ub.msie ? 1 : 0)) + "px");
            o.find(".free-cell-top-right").css("height", (freezeHeight - ((ub.msie || ub.chrome || ff16) ? (options.padding * 2 + 1) : 0) + ( ub.chrome ? 1:0))+ "px");
            if (isShowFooter) {
                o.find(".free-cell-bottom-right").css("height", (freezeFooterHeight + (isScrollX ? bar : 0) + 1) + "px");
            }
            freeCellBottom.css("top", (gridHeight - pagerHeight - freezeFooterHeight - bar - (!isShowFooter ? 1:0)) + "px");
            o.find(".scrollbar-y-content").css("height", (tblHeight - freezeHeight) + "px");

            yscrollbar.css("top", (freezeHeight + titleHeight - (ub.chrome ? (colHeaderContainer.find("thead tr").length - 2): 0)) + "px");
            xscrollbar.css("top", (gridHeight - pagerHeight - bar - 1) + "px");
            o.find(".free-bottom-left").css("top", (gridHeight - pagerHeight - bar - 1) + "px");

            rowFooterContainer.css("top", (gridHeight - pagerHeight - freezeFooterHeight - bar) + "px");
            colFooterContainer.css("top", (gridHeight - pagerHeight - freezeFooterHeight - bar) + "px").css("width", gridWidth + "px");
            pager.css("top", (gridHeight - pagerHeight - (isScrollX ? - (isShowFooter ? 0: 1) : bar) - (isShowFooter ? 0: 1)) + "px");

            xscrollbar.css("left", (freezeWidth + (options.padding) * 2 + 1) + "px");
            yscrollbar.css("left", (gridWidth - bar - 1) + "px");
            freeCellTop.css("left", (gridWidth - bar - 1) + "px");
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
        });
    };

    $.fn.renderScroller.defaults = {
        tblContentId: 'grid1',
        isUseLeftTable: false,
        isShowPager: false,
        column:[],
        numberFreezeCol: 2,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 0, //=totalColumnWidth + totalColumn*4 +totalColumn
        minWidth: 600,
        gridHeight: 300,
        freezeWidth: 241, //=totalColumnWidth + (totalColumn-1)*2 + (totalColumn-1)
        padding: 2,
        marginRight: 20
    };

    var scroller = function (obj, container, rowheader, xscrollbar, yscrollbar) {
        this.preLeft = xscrollbar.scrollLeft();
        this.preTop = yscrollbar.scrollTop();
        this.colHeaderContainer = $(obj).find(".col_header");
        this.rowHeaderContainer = $(obj).find(".row_header");
        this.colFooterContainer = $(obj).find(".col_footer");
        this.init(container, rowheader, xscrollbar, yscrollbar);
    };

    scroller.prototype = {
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
        }
    };
})(jQuery);
