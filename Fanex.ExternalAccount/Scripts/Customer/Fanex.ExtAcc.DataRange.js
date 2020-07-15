var dataRangeController = {
    txtMinDate: null,
    dataRangeType: {
        none: 1,
        timePeriod: 2,
        minDate: 3
    },
    currentCustomerIndex: 0,
    updateResultMessage: "",
    custIds: [],

    init: function () {
        var txtMinDate = $("#txt-min-date");
        dataRangeController.txtMinDate = txtMinDate;

        dataRangeController.clearForm();

        $("#btn-clear").click(dataRangeController.clearForm);

        txtMinDate.datepicker({
            showOn: 'both',
            buttonImageOnly: true,
            buttonImage: resolveClientUrl('Content/Images/calendar.png'),
            buttonText: '',
            dateFormat: 'M dd, yy',
            maxDate: new Date(),
        });

        txtMinDate.val($.datepicker.formatDate('M dd, yy', new Date()));

        $(".ui-datepicker").draggable();

        $("#btn-submit").click(dataRangeController.updateDataRange);

        $("#txt-time-period").keydown(function (e) {
            var val = $(this).val();
            var key = e.charCode || e.keyCode || 0;

            var isAccept =
                (e.ctrlKey && e.keyCode == 67) || //accept ctrl + c(copy)
                (e.ctrlKey && e.keyCode == 86) || //accept ctrl + v(paste)
                key == 16 || //shift
                key == 17 || //ctrl
                key == 8 || //backspace
                key == 36 || //home
                key == 35 || //end
                key == 9 || //tab
                key == 46 || //delete

                (key >= 37 && key <= 40) || //left , right , up , down (arrow)
                (key >= 48 && key <= 57) || //0-9
                (key >= 96 && key <= 105); //numpad 0-9

            return isAccept;
        }).keyup(function () {
            var val = $(this).val();

            if (isNaN(parseFloat(val))) {
                $(this).val("");
            }
        });
        $(".rdb-data-range").click(dataRangeController.clickDataRange);
        dataRangeController.initCustData();
    },
    clickDataRange: function () {
        var type = parseInt($(".rdb-data-range:checked").val(), 0);
        switch (type) {
            case dataRangeController.dataRangeType.none:
                $("#txt-time-period").attr("disabled", "disabled");
                $("#txt-time-period").val("");

                $("#txt-min-date").attr("disabled", "disabled");
                $("#txt-min-date").val($.datepicker.formatDate('M dd, yy', new Date()));
                $("#txt-min-date").datepicker("disable");
                break;
            case dataRangeController.dataRangeType.timePeriod:
                $("#txt-min-date").attr("disabled", "disabled");
                $("#txt-min-date").val($.datepicker.formatDate('M dd, yy', new Date()));
                $("#txt-min-date").datepicker("disable");

                $("#txt-time-period").attr("disabled", null);
                break;
            case dataRangeController.dataRangeType.minDate:
                $("#txt-time-period").attr("disabled", "disabled");
                $("#txt-time-period").val("");

                $("#txt-min-date").attr("disabled", null);
                $("#txt-min-date").datepicker("enable");
                break;
        }
    },
    clearForm: function () {
        $("#rdb-none").attr("checked", "checked");
        dataRangeController.clickDataRange();
    },
    updateDataRange: function () {
        var type = parseInt($(".rdb-data-range:checked").val(), 0);
        dataRangeController.custIds = dataRangeController.getCustIds();

        var params = {
            custIds: dataRangeController.custIds,
            choice: type
        };

        dataRangeController.updateResultMessage = "";

        switch (type) {
            case dataRangeController.dataRangeType.none:
                break;
            case dataRangeController.dataRangeType.timePeriod:
                params.LimitDays = $("#txt-time-period").val();

                if (params.LimitDays == "") {
                    $("#txt-time-period").focus();
                    Validators.showDown($("#txt-time-period").get(0), "This field is required", 0, 10, 37, -10, 10);
                    return;
                }

                var limitDays = parseInt(params.LimitDays, 0);

                if (limitDays == 0 || limitDays > 32767 || limitDays <= 0 || isNaN(limitDays)) {
                    $("#txt-time-period").focus();
                    Validators.showDown($("#txt-time-period").get(0), "Time period is from 1 to 32767", 0, 10, 37, -10, 10);
                    return;
                }

                break;
            case dataRangeController.dataRangeType.minDate:
                params.MinDate = $('#txt-min-date').val();
                break;
        }

        Fanex.ExtAcc.BlockUI();
        dataRangeController.postUpdateDataRange(params);
    },
    getCustIds: function () {
        return $("#CustIds").val();
    },
    postUpdateDataRange: function (params) {
        $.ajax({
            url: resolveClientUrl("DataRange/Update"),
            type: 'post',
            data: params
        }).success(function (result) {

            if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                return;
            }

            if (result && !result.IsSuccess) {
                dataRangeController.updateResultMessage = result.ErrorMessage;
            }

            Fanex.ExtAcc.UnBlockUI();
            dataRangeController.showResultMessage();
        }).error(function (xhr, status, err) {
            if (typeof console != 'undefined') {
                console.log(err);
            }

            dataRangeController.updateResultMessage = err + '<br/>';
            Fanex.ExtAcc.UnBlockUI();
            dataRangeController.showResultMessage();
        });
    },
    showResultMessage: function () {
        if (dataRangeController.updateResultMessage == "") {
            Fanex.ExtAcc.Form.Close();
            if ($('#customerListGrid').length)
                Fanex.ExtAcc.Customer.CallBackCustomerList();
            return;
        }

        Fanex.ExtAcc.Form.Alert("Error", dataRangeController.updateResultMessage.formatErrorMessage(), 300, 150, "alert");
    },
    initCustData: function () {
        var type = parseInt($("#DataRangeType").val(), 0);
        $(".rdb-data-range[value='" + type + "']").click();
        dataRangeController.clickDataRange();

        switch (type) {
            case dataRangeController.dataRangeType.none:
                break;
            case dataRangeController.dataRangeType.timePeriod:
                $("#txt-time-period").val($("#LimitDays").val());
                break;
            case dataRangeController.dataRangeType.minDate:
                $("#txt-min-date").val($.datepicker.formatDate('M dd, yy', new Date($("#MinDate").val())));
                break;
        }
    }
};

$(document).ready(dataRangeController.init);