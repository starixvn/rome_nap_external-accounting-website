var IsMaster = false;
var IsAgent = false;
var IsMember = false;
var UplineRoleId = 4;

function CustomerController($scope, $element) {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            if (antiXSRFToken.length > 0) {
                xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
            }
        }
    });

    $scope.CustomerModel = customerModel;
    $scope.Enabled = !customerModel.casinoDisabled;
    $scope.Level = '';
    $scope.IsMaster = false;
    $scope.IsAgent = false;
    $scope.IsMember = false;
    $scope.IsSuper = false;
    $scope.CustomerModel.maxTransferFormated = !$scope.CustomerModel.casinoSync && $scope.CustomerModel.roleId == 4 ? $scope.CustomerModel.defaultTransferCredit.formatNumber() : $scope.CustomerModel.maxTransferCredit.formatNumber();
    $scope.CustomerModel.memberWinLimit.maximumLossFormated = $scope.CustomerModel.memberWinLimit.maximumLoss.formatNumber();
    $scope.IsSuperMasterOnMember = ($scope.CustomerModel.uplineRoleId == 4 || $scope.CustomerModel.uplineRoleId == 3) && $scope.CustomerModel.roleId == 1;

    UplineRoleId = $scope.CustomerModel.uplineRoleId;

    switch ($scope.CustomerModel.roleId) {
        case 1:
            $scope.IsMember = true;
            IsMaster = false;
            IsAgent = false;
            IsMember = true;
            break;
        case 2:
            $scope.Level = 'Agent';
            $scope.IsAgent = true;
            IsMaster = false;
            IsAgent = true;
            IsMember = false;
            break;
        case 3:
            $scope.Level = 'Master';
            $scope.IsMaster = true;
            IsMaster = true;
            IsAgent = false;
            IsMember = false;
            break;
        default:
            $scope.Level = 'Super';
            $scope.IsSuper = true;
            IsMaster = false;
            IsAgent = false;
            IsMember = false;
            break;
    }

    if ($scope.IsSuper) {
        customerModel.maxTransferCreditLimit = 0;
    }

    $('#downline').each(function (i, item) {
        $(item).html($scope.Level)
    });

    $scope.EnableClicked = function () {
        var checkbox = $('#chkEnable').get(0);

        if (!$scope.IsMember) {
            if (checkbox.checked) {
                if (!$scope.Enabled) {
                    $('#chkEnableDownline').show();
                    $('#enableDownlineText').show();
                }

                $('#downline').html($scope.Level);
            }
            else {
                $('#chkEnableDownline').hide();
                $('#enableDownlineText').hide();

                if ($scope.Enabled) {
                    $('#downline').html($scope.Level + '&nbsp;' + "<font color='Red'>(Casino downlines will be disabled as well)</font>");
                }
            }
        }
    },

    $scope.Reset = function () {
        if ($scope.CustomerModel.roleId == 4 || ($scope.CustomerModel.uplineRoleId == 4 && $scope.CustomerModel.roleId == 2)) {
            return;
        }

        try {
            Fanex.CustomerSetting.Core.BlockUI();
        }
        catch (e) {
        }

        $.ajax({
            url: toUrl('CasinoCustomer/ResetMaxTransfer'),
            data: {
                id: $scope.CustomerModel.iBCCustID,
                groupName: parent.$('#selectedGroupName').html()
            }
        }).success(function (result) {
            if (result.errorCode == 0) {
                $('#errMsg').html('<font color="Blue">Reset max transfer successfully</font>'.formatSuccessMessage());
                $('#errMsg').show();
                setTimeout(function () {
                    document.location.reload(true);
                }, 2000);
            }
            else {
                $('#errMsg').html(result.errorMessage.formatErrorMessage());
                $('#errMsg').show();
            }
        }).error(function (data) {
            if (typeof console != 'undefined')
                console.log(data);
        }).complete(function (data) {
            try {
                Fanex.CustomerSetting.Core.UnBlockUI();
            }
            catch (e) {
            }
        });
    },

    $scope.GetCurrentWinLimit = function () {
        if ($scope.CustomerModel.roleId != 1) {
            return;
        }

        try {
            Fanex.CustomerSetting.Core.BlockUI();
        }
        catch (e) {
        }

        $.ajax({
            url: toUrl('CasinoCustomer/GetCurrentWinLimit'),
            data: {
                id: $scope.CustomerModel.iBCCustID,
                groupName: parent.$('#selectedGroupName').html()
            }
        }).success(function (result) {
            if (result.errorCode == 0) {
                var buildWinLossHandler = function (initArray, percent, maxAmount) {
                    if (maxAmount) {
                        if (percent >= 100) {
                            initArray.push('<font style="color: red">');
                            initArray.push(percent.toFixed(2).formatNumber());
                            initArray.push('%</font>');
                        }
                        else {
                            initArray.push(percent.toFixed(2).formatNumber());
                            initArray.push('%');
                        }

                        initArray.push('&nbsp;&nbsp;');
                        initArray.push(casinoLang.of);
                        initArray.push('&nbsp;&nbsp;');
                        initArray.push(maxAmount.toFixed(2).formatNumber());
                    }
                    else {
                        initArray.push(casinoLang.unlimited);
                    }

                    return '<p style="margin:3px 0;">' + initArray.join('') + '</p>';
                };

                var currentWinLoss = result.result;

                var winningHtml = buildWinLossHandler(
                    [casinoLang.winning, ':&nbsp;&nbsp;'],
                    currentWinLoss.winPercent,
                    currentWinLoss.maxWinAmount);

                var losingHtml = buildWinLossHandler(
                    [casinoLang.losing, ':&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'],
                    currentWinLoss.lossPercent,
                    currentWinLoss.maxLossAmount);

                var $currentWinLimit = $('#currentWinLimit');
                $currentWinLimit
                    .attr('style', '')
                    .unbind('click')
                    .html(winningHtml + losingHtml);
            }
            else {
                var $errMsg = $('#errMsg');
                $errMsg
                    .html(result.errorMessage.formatErrorMessage())
                    .show();
            }
        }).error(function (data) {
            if (typeof console != 'undefined')
                console.log(data);
        }).complete(function (data) {
            try {
                Fanex.CustomerSetting.Core.UnBlockUI();
            }
            catch (e) {
            }
        });
    }
}