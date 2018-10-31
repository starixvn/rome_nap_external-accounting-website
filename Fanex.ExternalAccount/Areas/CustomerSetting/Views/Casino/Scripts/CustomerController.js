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
    $scope.CustomerModel.memberWinLimit.maximumLossFormated = $scope.CustomerModel.memberWinLimit.maximumLoss.formatNumber();
    $scope.IsSuperMasterOnMember = ($scope.CustomerModel.uplineRoleId == 4 || $scope.CustomerModel.uplineRoleId == 3) && $scope.CustomerModel.roleId == 1;
    $scope.IsCrossLine = $scope.CustomerModel.uplineRoleId - $scope.CustomerModel.roleId > 1 ||
        parent.Fanex.ExtAcc.Customer.isSuperEditMember ||
        parent.Fanex.ExtAcc.Customer.isMasterEditMember;
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
            $.ajax({
                url: domain + 'MainSuper/GetInfo',
                method: 'POST',
                data: { superId: custId },
                async: false
            }).success(function (result) {
                $scope.CustomerModel.firstName = result.firstName;
                $scope.CustomerModel.lastName = result.lastName;
                $scope.CustomerModel.phone = result.phone;
                $scope.CustomerModel.mobilePhone = result.mobilePhone;
                $scope.CustomerModel.fax = result.fax;
            });
            break;
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
                var buildWinLossHandler = function (initArray, current, percent, maxAmount) {
                    if (percent >= 100 || percent == -2 || percent > 0) { // >= 100 or N/A or specific
                        if (percent >= 100 || percent == -2) {
                            initArray.push('<font style="color: red">');
                        }
                        initArray.push(percent == -2 ? "N/A" : percent.toFixed(2).formatNumber() + "%");
                        if (percent >= 100 || percent == -2) {
                            initArray.push('</font>');
                        }

                        initArray.push('&nbsp;&nbsp;');
                        initArray.push('(');
                        if (current >= maxAmount && maxAmount != 0) {
                            initArray.push('<font style="color: red">');
                        }
                        initArray.push(current.toFixed(2).formatNumber());
                        if (current >= maxAmount && maxAmount != 0) {
                            initArray.push('</font>');
                        }
                        initArray.push('/');
                        if (maxAmount <= 0) {
                            initArray.push('<font style="color: red">');
                        }
                        initArray.push(maxAmount.toFixed(2).formatNumber());
                        if (maxAmount <= 0) {
                            initArray.push('</font>');
                        }
                        initArray.push(')');
                    }
                    else if (percent == -1) { // Unlimited
                        initArray.push(casinoLang.lblunlimited);
                    }
                    else { // 0%
                        initArray.push('0%');
                    }

                    return '<p style="margin:3px 0;line-height:1.2;">' + initArray.join('') + '</p>';
                };

                var currentWinLoss = result.result;

                var winningHtml = buildWinLossHandler(
                    [casinoLang.winning, ':&nbsp;&nbsp;'],
                    currentWinLoss.currentAmount,
                    currentWinLoss.winPercent,
                    currentWinLoss.maxWinAmount);

                var losingHtml = buildWinLossHandler(
                    [casinoLang.losing, ':&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;'],
                    currentWinLoss.currentAmount * -1,
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

    $scope.SaveCurrentWinLimit = function () {
        var custids = parent.Fanex.ExtAcc.Customer.Ids();
        var usernames = parent.Fanex.ExtAcc.Customer.Names();
        var isMultipleEdit = parent.Fanex.ExtAcc.Customer.IsMultiple;
        var syncStatuses = parent.Fanex.ExtAcc.Customer.RnGCasinoStatuses;
        
        UpdateWinLossMessage = '';
        HasError = false;
        try {
            Fanex.CustomerSetting.Core.BlockUI();
        }
        catch (e) {
        }

        if (isMultipleEdit) {            
            ProcessUpdateWinLossLimit(0, custids, usernames, JSON.stringify($scope.CustomerModel.memberWinLimit), syncStatuses);
        }
        else {
            ProcessUpdateWinLossLimit(null, custids, usernames, JSON.stringify($scope.CustomerModel.memberWinLimit), syncStatuses);
        }
    }

    var UpdateWinLossMessage = '';
    var HasError = false;
    var ProcessUpdateWinLossLimit = function (index, custIds, usernames, winLossLimit, syncStatuses) {
        var custId = index == null ? $scope.CustomerModel.iBCCustID : custIds[index];
        var username = index == null ? $scope.CustomerModel.userName : usernames[index];
        if (index == null && !$scope.CustomerModel.casinoSync) {
            HasError = true;
            UpdateWinLossMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + username + ":</font><br /><font color='red'>Not casino's Customer.</font></div>";
            try {
                $('#errMsg')
                    .html(UpdateWinLossMessage)
                    .show();

                Fanex.CustomerSetting.Core.UnBlockUI();
            }
            catch (e) {
            }
        }
        else if (index != null && !syncStatuses[index]) {
            HasError = true;
            UpdateWinLossMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + username + ":</font><br /><font color='red'>Not casino's Customer </font></div>";
            ProcessUpdateWinLossLimit(++index, custIds, usernames, winLossLimit, syncStatuses);
        }
        else {
            $.ajax({
                url: toUrl('WinLossLimit/Update'),
                type: 'post',
                data: {
                    custId: custId,
                    userId: userId,
                    subAccId: subAccId,
                    winLimit: winLossLimit
                }
            }).success(function (result) {
                if (result.errorCode == 0) {
                    UpdateWinLossMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + username + ":</font><br /><font color='blue'>Updated successfully.</font></div>";
                }
                else {
                    HasError = true;
                    UpdateWinLossMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + username + ":</font><br /><font color='red'>" + result.errorMessage.formatErrorMessage() + "</font></div>";
                }
                if (index == null || index == custIds.length - 1) {
                    $('#errMsg')
                        .html(UpdateWinLossMessage)
                        .show();

                    try {
                        Fanex.CustomerSetting.Core.UnBlockUI();
                        if (!HasError) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                if (parent.document.getElementById('customerListGrid')) {
                                    parent.Fanex.ExtAcc.Customer.CallBackCustomerList();
                                    parent.Fanex.ExtAcc.Form.Close();
                                }
                                else {
                                    document.location.reload(true);
                                }
                            }, 3000);
                        }
                    }
                    catch (e) {
                    }
                }
                else {
                    ProcessUpdateWinLossLimit(++index, custIds, usernames, winLossLimit, syncStatuses);
                }
            }).error(function (data) {
                HasError = true;
                UpdateWinLossMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + username + ":</font><br /><font color='red'>Not casino's Customer.</font></div>";
                if (index == null || index == custIds.length - 1) {
                    $('#errMsg')
                        .html(UpdateWinLossMessage)
                        .show();

                    try {
                        Fanex.CustomerSetting.Core.UnBlockUI();
                    }
                    catch (e) {
                    }
                }
                else {
                    ProcessUpdateWinLossLimit(++index, custIds, usernames, winLossLimit, syncStatuses);
                }
            });
        }
    }
}