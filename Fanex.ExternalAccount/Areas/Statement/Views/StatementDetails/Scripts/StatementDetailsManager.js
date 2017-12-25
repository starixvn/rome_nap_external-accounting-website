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


    };
};

var statementDetailsManager = new StatementDetailsManager();