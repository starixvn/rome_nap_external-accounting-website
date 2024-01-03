"use strict";
var WinLossDetailCopyHtml = /** @class */ (function () {
    function WinLossDetailCopyHtml(clipboard) {
        this.clipboard = clipboard;
    }
    WinLossDetailCopyHtml.prototype.init = function (tableId) {
        var _this = this;
        if (tableId === void 0) { tableId = "tbl-report"; }
        $("#icon-copy").tooltip();
        var clipboard = new this.clipboard("#icon-copy", {
            text: function () { return _this._copyTableHtml($("#" + tableId)); }
        });
        if (window.document.queryCommandSupported && window.document.queryCommandSupported("copy")) {
            clipboard.on("success", function (e) {
                var $this = $(e.trigger);
                $this.tooltip({ content: $("#hiddenCopiedText").val() }).tooltip("open");
                setTimeout(function () {
                    $this.tooltip({ content: $("#hiddenCopyText").val() }).tooltip("close");
                }, 3000);
            }).on("error", function (e) {
                $(e.trigger).remove();
                console.error("Action:", e.action);
                console.error("Trigger:", e.trigger);
            });
        }
    };
    WinLossDetailCopyHtml.prototype._copyTableHtml = function (tableReport) {
        var html = ['<table>'];
        html.push(this._copyTableHeader(tableReport));
        html.push(this._copyTableBody(tableReport));
        html.push(this._copyTableFooter(tableReport));
        html.push('</table>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableHeader = function (tableReport) {
        var html = ['<thead><tr>'];
        tableReport.find("thead tr:first th:not(.hide)").each(function () {
            var $this = $(this);
            html.push('<th');
            var rowspan = $this.attr("rowspan");
            if (rowspan) {
                html.push(' rowspan="', rowspan, '"');
            }
            var colspan = $this.attr("colspan");
            if (colspan) {
                html.push(' colspan="', colspan, '"');
            }
            html.push('>', this.innerText, '</th>');
        });
        html.push('</tr>');
        if (tableReport.find("thead tr").length == 2) {
            html.push('<tr>');
            tableReport.find("thead tr:last th:not(.hide)").each(function () {
                html.push('<th>', this.innerText, '</th>');
            });
            html.push('</tr>');
        }
        html.push('</thead>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableBody = function (tableReport) {
        var html = ['<tbody>'];
        tableReport.find("tbody tr").each(function () {
            html.push('<tr>');
            $(this).find("td:not(.hide)").each(function () {
                var $this = $(this);
                html.push('<td');
                var rowspan = $this.attr("rowspan");
                if (rowspan) {
                    html.push(' rowspan="', rowspan, '"');
                }
                var colspan = $this.attr("colspan");
                if (colspan) {
                    html.push(' colspan="', colspan, '"');
                }
                html.push('>', this.innerText, '</td>');
            });
            html.push('</tr>');
        });
        html.push('</tbody>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableFooter = function (tableReport) {
        var html = ['<tfoot><tr>'];
        tableReport.find("tfoot tr td:not(.hide)").each(function () {
            html.push('<td>', this.innerText, '</td>');
        });
        html.push('</tr></tfoot>');
        return html.join("");
    };
    return WinLossDetailCopyHtml;
}());
//# sourceMappingURL=winlossdetail-copyhtml.js.map