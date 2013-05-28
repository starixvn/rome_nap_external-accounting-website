var jOldVerion = parseFloat($.fn.jquery.substring(0, 3)) <= 1.5 ? true : false;

(function ($) {

    $.fn.gridScrollerLite = function (opts) {

        return this.each(function () {
            var o = $(this);
          
            var container = o.find(".container");
            var options = $.extend({}, $.fn.renderScrollerLite.defaults, opts);

            $(window).resize(function () {
                if (!o.hasClass("hidden")) {
                    o.resetGridLite(options.tblContentId);
                    var opt = $.extend({}, {}, options);
                    opt.manualCopyHeader = true;
                    o.renderScrollerLite(opt);
                    o.find('.grid-fixed-table').append(' ');
                }
            });
            o.renderScrollerLite(options);
            o.find('.grid-fixed-table').append(' ');
          
        });

    };

    $.fn.initGridLite = function (options) {
        return this.each(function () {
            var o = $(this);
            //Render Free Cell
            var freeCellTpl = '<div class="free-cell free-top-right"></div>';
            o.append(freeCellTpl);
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

    $.fn.resetGridLite = function () {
        return this.each(function (id) {
            var o = $(this);       
            var container = o.find(".container");
            var tblContent = $("#" + id);
            var headerContainer = o.find(".col_header");
            var footerContainer = o.find(".col_footer");

            o.css("width", "");
            container.css("width", "");
            o.find(".content-container").css("width", "")
            headerContainer.css("width", "");
            footerContainer.css("width", "");
        });
    };

    $.fn.renderGrid=function(options){
        return this.each(function () {
             var bar = 17;
            var o = $(this);
            var title = o.find(".grid-title");
            var titleHeight = title.length > 0 ? title.height() : 0;
            var pager = o.find(".grid-pager");
            var pagerHeight = pager.length > 0 ? pager.height() : 0;
        
            var container = o.find(".container");
            var tblContent = o.find("#" + options.tblContentId);
            var freeCellTop = o.find(".free-top-right");
            var headerContainer = o.find(".col_header");
            var footerContainer = o.find(".col_footer");

            var header=tblContent.find("thead").remove();
            headerContainer.find(".col_header_content thead").empty().append(header.html());
            var footer=tblContent.find("tfoot").remove();
            footerContainer.find(".col_footer_content thead").empty().append(footer.html());

            var headerHeight =  headerContainer.length > 0 ? headerContainer.find("thead").height() : 0;
            var footerHeight = footerContainer.length > 0 ? footerContainer.find("thead").height() : 0;
            var gridWidth = options.fullWidth ? ($(window).width() - options.marginRight) : options.gridWidth;

            o.css("width", gridWidth + "px");
            container.css("width", (gridWidth) + "px");
            tblContent.css("width", (gridWidth - 17) + "px");
            headerContainer.css("width", (gridWidth) + "px");
            headerContainer.find(".col_header_content").css("width", (gridWidth -17) + "px");
            footerContainer.css("width", (gridWidth) + "px");
            footerContainer.find(".col_footer_content").css("width", (gridWidth -17) + "px");
         
            pager.css("width", gridWidth + "px");

            var tblHeight = tblContent.height();
            var gridHeight = options.fullHeight ? ($(window).height() - options.paddingHeight) : options.gridHeight;
            var isNoScrollY=tblHeight + titleHeight + headerHeight + footerHeight + pagerHeight + options.paddingHeight < gridHeight;
            if (isNoScrollY)
            {
                gridHeight=tblHeight + titleHeight + headerHeight + footerHeight + pagerHeight + options.paddingHeight;
            }
            
            o.css("height", (gridHeight - (isNoScrollY ? options.paddingHeight:0)) + "px");
            container.css("height", (gridHeight - titleHeight - headerHeight  - footerHeight - pagerHeight)  + "px");
            container.css("top",(titleHeight + headerHeight) + "px");
         
            o.find(".free-top-right").css("height", (headerHeight - 1) + "px");
            o.find(".free-top-right").css("top", (titleHeight + 1) + "px");
           
            footerContainer.css("top", (gridHeight - pagerHeight - footerHeight - (isNoScrollY ? options.paddingHeight:0)) + "px").css("width", gridWidth + "px");
            pager.css("top", (gridHeight - pagerHeight) + "px")
            freeCellTop.css("left", (gridWidth - bar - 1) + "px");
        });
    }

    $.fn.renderScrollerLite = function (options) {
        return this.each(function () {
            var o = $(this);
            var tblContent = o.find("#" + options.tblContentId);
            var headerContainer = o.find(".col_header");
            var footerContainer = o.find(".col_footer");
            o.renderGrid(options);
      
            if (options.autoFixColumnWidth)
            {
                var tds=tblContent.find("tr:first td");
                var cols=tds.length;
                for (i=0;i<cols;i++)
                {
                    var w=$(tds[i]).width() + "px";
                    headerContainer.find("tr:first th:eq(" + i + ")").css({width:w});
                    footerContainer.find("tr:first th:eq(" + i + ")").css({width:w});
                    o.renderGrid(options);
                }

            }

        });
    };



    $.fn.renderScrollerLite.defaults = {
        tblContentId: 'grid1',
        isShowPager: false,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 0,
        gridHeight: 0,
        paddingHeight:0,
        padding: 2,
        autoFixColumnWidth:false,
        marginRight: 6
    };

})(jQuery);