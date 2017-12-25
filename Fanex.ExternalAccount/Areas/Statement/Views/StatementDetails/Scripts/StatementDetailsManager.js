var statementDetailsManager;
$(document).ready(function () {
    statementDetailsManager.initPage();
});

var StatementDetailsManager = function () {
    var self = this;
    self.Constants = {
        Format: {
            Number: {
                TwoDecimal: "0,0.00"
            }
        }
    };
    self.initPage = function () {
        self.renderTransferDetails(statementReport);

        $("#").click(function () {
            var url = location.protocol +
                window.rootUrl +
                'NapStatement/StatementReport/ExportTransferDetails?' +
                jQuery.param(transferDetailsCriteria);

            $('#frm').attr('src', url);
        });
    };
    self.renderTransferDetails = function (statementReport) {
        var bodyContent = self.renderBody(statementReport);
        var footerContent = self.renderFooter();

        var tableReport = $("#transfer-details-container");
        tableReport.find("tbody").empty().append(bodyContent);
        tableReport.find("tfoot").empty().append(footerContent);
    };

    self.totalRows = 0;
    self.renderBody = function (statementReport) {
        var html = '';
        var statementListLength = statementReport && statementReport.StatementList ?
            statementReport.StatementList.length :
            0;
        self.totalRows = statementListLength;

        if (statementListLength === 0) {
            html = '<tr><td colspan="7" class="no-data">There is no data</td></tr>';

            return html;
        }

        for (var i = 0; i < statementListLength; i++) {
            var statement = statementReport.StatementList[i];

            var remark = statement.TransactionType + "<br>" + statement.Description +
            (transferDetailsCriteria.SiteName == "NAP" && statement.ExchangeRate ?
                " - " + statementResources.Symbol_ExchangeRate + ":" + statement.ExchangeRate :
                '');

            var amount = statement.TransferAmount.HasData
                ? self.renderNegativeNumber(statement.TransferAmount.Value)
                : statementResources.Symbol_NoData;

            html += '<tr>\
                        <td class="align-center">' + statement.StatementDateString + '</td>\
                        <td>' + remark + '</td>\
                         <td class="align-right">' + amount + '</td>\
                     </tr>';
        }

        return html;
    };
    self.renderNegativeNumber = function (value) {
        var html = numeral(value).format('0,0.00');

        return value < 0 ? '<span class="format-negative">' + html + '</span>' : html;
    };
    self.renderFooter = function () {
        var html = '';
        if (self.totalRows === 0) {
            return html;
        }

        var statement = statementReport.Total;
        var amount = statement.TransferAmount.HasData
            ? self.renderNegativeNumber(statement.TransferAmount.Value)
            : statementResources.Symbol_NoData;

        html += '<tr>\
                        <td class="align-right" colspan="2">' + statementResources.Total + '</td>\
                         <td class="align-right">' + amount + '</td>\
                     </tr>';

        return html;
    };
};

var statementDetailsManager = new StatementDetailsManager();