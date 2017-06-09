function EnableThirdPartyProductStatus() {
    var self = this;

    self.checkAll_OnChange = function () {
        var checkEnableAll_Element = $('#checkAll');
        var checkEnableAll_Value = checkEnableAll_Element.is(':checked');
        var checkEnable_Collection = $('input[id^="checkEnable_"]:enabled', '#updateStatusDialog')
        checkEnable_Collection.each(function () {
            var $this = $(this);
            if (checkEnableAll_Value) {
                if (!$this.prop('checked')) {
                    $this.prop('checked', true);
                    $this.trigger('change');
                }
            } else {
                if ($this.prop('checked')) {
                    $this.prop('checked', false);
                    $this.trigger('change');
                }
            }
        });
    }

    self.checkEnable_OnChange = function (productStatusDataRow) {
        var checkEnable_Element = $('#checkEnable_' + productStatusDataRow.SportType + '_' + productStatusDataRow.BetType);
        var checkEnableDownlines_Element = $('#checkEnableDownlines_' + productStatusDataRow.SportType + '_' + productStatusDataRow.BetType);
        var checkEnable_Value = checkEnable_Element.is(':checked');
        var checkEnableDownlines_Value = checkEnableDownlines_Element.is(':checked');
        var checkEnable_OriginalValue = !productStatusDataRow.IsDisabled;
        var checkEnableDownlines_OriginalValue = !productStatusDataRow.IsDisabledDownline;
        var submitBtn = $('#thirdPartyProductStatusSubmitBtn');
        if (checkEnable_Value) {
            if (checkEnableDownlines_OriginalValue) {
                checkEnableDownlines_Element.prop('checked', true);
                checkEnableDownlines_Element.prop('disabled', true);
            }
            else {
                checkEnableDownlines_Element.prop('checked', false);
                checkEnableDownlines_Element.prop('disabled', false);
            }
        }
        else {
            checkEnableDownlines_Element.prop('disabled', true);
            checkEnableDownlines_Element.prop('checked', false);
        }

        if (checkEnable_Value == checkEnable_OriginalValue) {
            checkEnable_Element.removeClass('is-data-change');
        }
        else {
            checkEnable_Element.addClass('is-data-change');
        }

        ShowHideSubmitButton();
    }

    self.checkEnableDownlines_OnChange = function (productStatusDataRow) {
        var checkEnableDownlines_Element = $('#checkEnableDownlines_' + productStatusDataRow.SportType + '_' + productStatusDataRow.BetType);
        var checkEnableDownlines_Value = checkEnableDownlines_Element.is(':checked');
        var checkEnableDownlines_OriginalValue = !productStatusDataRow.IsDisabledDownline;
        var submitBtn = $('#thirdPartyProductStatusSubmitBtn');
        if (checkEnableDownlines_Value == checkEnableDownlines_OriginalValue) {
            checkEnableDownlines_Element.removeClass('is-data-change');
        } else {
            checkEnableDownlines_Element.addClass('is-data-change');
        }

        ShowHideSubmitButton();
    }

    self.submit = function (model) {
        Fanex.CustomerSetting.Core.BlockUI();
        ClearMessages();
        for (var i = 0 ; i < model.ProductStatusList.length; i++) {
            var productStatusDataRow = model.ProductStatusList[i];
            productStatusDataRow.IsDisabled = !$('#checkEnable_' + productStatusDataRow.SportType + '_' + productStatusDataRow.BetType).is(':checked');
            if (productStatusDataRow.HasDownline) {
                productStatusDataRow.IsDisabledDownline = !$('#checkEnableDownlines_' + productStatusDataRow.SportType + '_' + productStatusDataRow.BetType).is(':checked');
            }
        }

        RemoveUnchangedDataRow(model, originalModel);
        if (model.ProductStatusList.length > 0) {
            var param = {
                custId: model.CustId,
                productId: model.ProductStatusList[0].ProductId,
                productStatus: JSON.stringify(model.ProductStatusList)
            };

            $.post("ThirdPartyProductStatus/Update", param, function (result) { UpdateSuccess(result); });
        } else {
            $('#updateStatusDialog>#wrnMsg').text(errorMsg.ThereIsNoChange).removeClass('hidden');
            Fanex.CustomerSetting.Core.UnBlockUI();
        }
    }

    function RemoveUnchangedDataRow(model, originalModel) {
        model.ProductStatusList = model.ProductStatusList.filter(function (item, index) {
            var originalItem = originalModel.ProductStatusList[index];
            return !(originalItem && originalItem.IsDisabled == item.IsDisabled && originalItem.IsDisabledDownline == item.IsDisabledDownline);
        });
    }

    function UpdateSuccess(result) {
        var isSuccess = true;
        var errorMsg = '';
        for (var i = 0 ; i < result.length; i++) {
            var errorCode = result[i].ErrorCode;
            var errorMessage = result[i].ErrorMessage;
            if (errorCode > 0) {
                isSuccess = false;
                errorMsg += errorMessage + '\n';
            }
        }

        if (isSuccess) {
            var $successDiv = $('#updateStatusDialog>#successMsg');
            $successDiv.text('Status updated successfully');
            $successDiv.removeClass('hidden');
            $('#thirdPartyProductStatusSubmitBtn').prop('disabled', true);
            $('#thirdPartyProductStatusSubmitBtn').unbind();
            setTimeout(function () {
                parent.Fanex.ExtAcc.Form.Close();
                Fanex.CustomerSetting.Core.BlockUI();
                var iframe = parent.document.getElementById("leftFrame");
                iframe.src = iframe.src;
            }, 5000);
        } else {
            var $errorDiv = $('#updateStatusDialog>#errMsg');
            $errorDiv.text(errorMsg);
            $errorDiv.removeClass('hidden');
        }

        Fanex.CustomerSetting.Core.UnBlockUI();
    }

    function ClearMessages() {
        var $successDiv = $('#updateStatusDialog>#successMsg');
        var $errorDiv = $('#updateStatusDialog>#errMsg');
        var $warningDiv = $('#updateStatusDialog>#wrnMsg');
        $successDiv.text('');
        $successDiv.addClass('hidden');
        $errorDiv.text('');
        $errorDiv.addClass('hidden');
        $warningDiv.text('');
        $warningDiv.addClass('hidden');
    }

    function ShowHideSubmitButton() {
        var submitBtn = $('#thirdPartyProductStatusSubmitBtn');
        if ($('input[id^="checkEnable_"],input[id^="checkEnableDownlines_"]').hasClass('is-data-change')) {
            submitBtn.prop('disabled', false); 
        }
        else {
            submitBtn.prop('disabled', true);
        }
    }
};

var enableThirdPartyProductStatus = new EnableThirdPartyProductStatus();