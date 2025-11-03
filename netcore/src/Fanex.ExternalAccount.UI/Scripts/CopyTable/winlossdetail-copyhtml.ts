"use strict";
declare var Clipboard: any;

class WinLossDetailCopyHtml {
    private clipboard: any;

    constructor(clipboard: any) {
        this.clipboard = clipboard;
    }

    init(tableId = "tbl-report") {
        $("#icon-copy").tooltip();
        const clipboard = new this.clipboard("#icon-copy", {
            text: () => this._copyTableHtml($(`#${tableId}`))
        });

        if (window.document.queryCommandSupported && window.document.queryCommandSupported("copy")) {
            clipboard.on("success", function (e) {
                const $this = $(e.trigger);
                $this.tooltip({ content: $("#hiddenCopiedText").val()}).tooltip("open");
                setTimeout(() => {
                    $this.tooltip({ content: $("#hiddenCopyText").val() }).tooltip("close");
                }, 3000);
            }).on("error", function (e) {
                $(e.trigger).remove();
                console.error("Action:", e.action);
                console.error("Trigger:", e.trigger);
            });
        }
    }

    _copyTableHtml(tableReport: JQuery) {
        const html = ['<table>'];
        html.push(this._copyTableHeader(tableReport));
        html.push(this._copyTableBody(tableReport));
        html.push(this._copyTableFooter(tableReport));
        html.push('</table>');

        return html.join("");
    }

    _copyTableHeader(tableReport: JQuery) {
        const html = ['<thead><tr>'];
        tableReport.find("thead tr:first th:not(.hide)").each(function () {
            const $this = $(this);
            html.push('<th');

            const rowspan = $this.attr("rowspan");
            if (rowspan) {
                html.push(' rowspan="', rowspan, '"');
            }

            const colspan = $this.attr("colspan");
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
    }

    _copyTableBody(tableReport: JQuery) {
        const html = ['<tbody>'];

        tableReport.find("tbody tr").each(function () {
            html.push('<tr>');
            $(this).find("td:not(.hide)").each(function () {
                const $this = $(this);
                html.push('<td');
                const rowspan = $this.attr("rowspan");
                if (rowspan) {
                    html.push(' rowspan="', rowspan, '"');
                }

                const colspan = $this.attr("colspan");
                if (colspan) {
                    html.push(' colspan="', colspan, '"');
                }
                html.push('>', this.innerText, '</td>');
            });
            html.push('</tr>');
        });

        html.push('</tbody>');
        return html.join("");
    }

    _copyTableFooter(tableReport: JQuery) {
        const html = ['<tfoot><tr>'];
        tableReport.find("tfoot tr td:not(.hide)").each(function () {
            html.push('<td>', this.innerText, '</td>');
        });

        html.push('</tr></tfoot>');
        return html.join("");
    }
}