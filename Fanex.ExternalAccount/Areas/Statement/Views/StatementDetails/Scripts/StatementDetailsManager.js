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
    };
    self.renderTransferDetails = function (statementReport) {
        var statementListLength = statementReport && statementReport.StatementList ?
            statementReport.StatementList.length :
            0;
        var bodyData = [];
        var footerData = [];
        if (statementListLength) {
            for (var i = 0; i < statementListLength; i++) {
                var statement = statementReport.StatementList[i];
                var remark = statement.TransactionType + "<br>" + statement.Description +
                    (transferDetailsCriteria.SiteName == "NAP" && statement.ExchangeRate ?
                        " - " + statementResources.Symbol_ExchangeRate + ":" + statement.ExchangeRate :
                        '');

                var currentRow = {
                    Columns: [{ Name: "ColDate", Value: statement.StatementDateString },
                        { Name: "ColRemark", Value: remark },
                        {
                            Name: "ColAmount",
                            Value: statement.TransferAmount.HasData ?
                                statement.TransferAmount.Value :
                                statementResources.Symbol_NoData
                        }]
                };

                bodyData.push(currentRow);
            }

            var statement = statementReport.Total;
            footerData.push({
                Columns: [
                    {
                        Name: "ColDate",
                        Value: statementResources.Total,
                        MergeColNumber: 2,
                        CellAlign: 'right'
                    },
                    {
                        Name: "ColAmount",
                        Value: statement.TransferAmount.HasData ?
                            statement.TransferAmount.Value :
                            statementResources.Symbol_NoData
                    }]
            });
        }

        var headerColumns = [
            {
                HeaderText: statementResources.Date,
                Width: 150,
                Name: "ColDate",
                HeaderAlign: "center",
                CellAlign: "center"
            },
            {
                HeaderText: statementResources.Remark,
                Width: 400,
                Name: "ColRemark",
                HeaderAlign: "center",
                CellAlign: "left"
            },
            {
                HeaderText: statementResources.Amount,
                Width: 150,
                Name: "ColAmount",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            }];

        gridOptions = {
            columns: headerColumns,
            bodyRows: bodyData,
            footerRows: footerData,
            headerColumnHeight: 24,
            bodyRowHeight: 25,
            numberOfFrozenColumn: 0,
            gridExpandHeight: 50,
            showHighlightRow: false,
            showPager: false,
            showTitle: true,
            titleInfo: {
                text: "",
                showExcelButton: true,
                exportExcelFunction: function () {
                    var url = location.protocol + window.rootUrl + 'NapStatement/StatementReport/ExportTransferDetails?' +
                        jQuery.param(transferDetailsCriteria);
                    jQuery('#frm').attr('src', url);

                }
            },
            showFooter: true
        };

        $('#transfer-details-container').fanexGrid(gridOptions);
    };
};

var statementDetailsManager = new StatementDetailsManager();