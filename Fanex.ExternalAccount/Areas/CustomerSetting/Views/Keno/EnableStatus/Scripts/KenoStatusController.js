angular.module('kenoStatusApp', []);

angular.element($('#ng-app')).ready(function () {
    $('#ng-app').show();
});

function KenoStatusController($scope) {
    $scope.model = model;
    $scope.betTypes = $scope.model.betTypeStatusList;
    $scope.isDisabledAll = true;
    $scope.isCheckeddAll = true;
    var length = $scope.model.betTypeStatusList.length;
    for (var i = 0; i < length; i++) {
        if (!$scope.model.betTypeStatusList[i].isDisabledUpline) {
            $scope.isDisabledAll = false;
            break;
        }
    }

    $scope.Init = function () {
        var allChecked = true;
        for (var i = 0; i < $scope.betTypes.length; i++) {
            if (($scope.betTypes[i].isDisabled && !$scope.betTypes[i].isDisabledUpline) || $scope.isDisabledAll) {
                allChecked = false;
                break;
            }
        }

        if (allChecked)
            document.getElementById('chkAll').checked = true;
    };

    $scope.CheckStatus = function (preVal, scope) {
        var betTypesCheckboxes = document.getElementsByName('betTypes0');
        var allChecked = true;

        if (preVal) {
            if (scope.item.enable) {
                scope.item.enableDownline = scope.item.preValDownline;
                scope.item.disableDownline = true;

                scope.item.isDisabled = false;
                scope.item.isDisabledUpline = false;
                scope.item.isDisabledDownlines = false;

                for (var i = 0; i < betTypesCheckboxes.length; i++) {
                    if (!betTypesCheckboxes[i].checked && !betTypesCheckboxes[i].disabled) {
                        allChecked = false;
                        break;
                    }
                }

                if (allChecked)
                    document.getElementById('chkAll').checked = true;
            }
            else {
                document.getElementById('chkAll').checked = false;

                scope.item.enableDownline = false;
                scope.item.disableDownline = true;                

                scope.item.isDisabled = true;
                scope.item.isDisabledUpline = false;
                scope.item.isDisabledDownlines = true;
            }
        }
        else {
            if (scope.item.enable) {
                scope.item.enableDownline = false;
                scope.item.disableDownline = false;

                scope.item.isDisabled = false;
                scope.item.isDisabledUpline = false;
                scope.item.isDisabledDownlines = false;

                for (var i = 0; i < betTypesCheckboxes.length; i++) {
                    if (!betTypesCheckboxes[i].checked && !betTypesCheckboxes[i].disabled) {
                        allChecked = false;
                        break;
                    }
                }

                if (allChecked)
                    document.getElementById('chkAll').checked = true;
            }
            else {
                document.getElementById('chkAll').checked = false;

                scope.item.enableDownline = false;
                scope.item.disableDownline = true;

                scope.item.isDisabled = true;
                scope.item.isDisabledUpline = false;
                scope.item.isDisabledDownlines = true;
            }
        }

        document.getElementById(scope.item.betTypeID).disabled = scope.item.disableDownline;
        document.getElementById(scope.item.betTypeID).checked = scope.item.enableDownline;
    };

    $scope.Update = function () {
        parent.Fanex.ExtAcc.BlockUI();
        var length = model.betTypeStatusList.length;
        for (var i = 0; i < length; i++) {
            model.betTypeStatusList[i].isDisabledDownlines = model.betTypeStatusList[i].enableDownline;
        }
        $.ajax({
            dataType: 'json',
            type: 'post',
            url: resolveClientUrl('CustomerSetting/Keno/EnableStatus/Update'),
            data: {
                id: custId,
                uplineId: uplineId,
                model: JSON.stringify(model),
                roleId: roleId,
                username: model.userName,
                groupName: parent.$('#selectedGroupName').html()
            }
        })
        .success(function (data) {
            var errorCodes = data.errorCode.toString().indexOf('\u0007') != -1 ? data.errorCode.split('\u0007') : data.errorCode.toString();
            var errorMessages = data.errorMessage.toString().indexOf('\u0007') != -1 ? data.errorMessage.split('\u0007') : data.errorMessage.toString();
            var success = data.errorCode == 1 ? false : true;
            var message = data.errorCode == 1 ? data.errorMessage : '';

            $(errorCodes).each(function (i, error) {
                if (error != '' && parseInt(error) > 0) {
                    success = false;
                    message += (parseInt(error) == 600 ? 'Account was disabled' : errorMessages[i]) + '<br />';
                }
            });

            if (data.errorCode == "0" || data.errorCode == "" || success) {
                $('#errMsg').attr('class', 'successMsg');
                $('#errMsg').html('<font color="Blue">' + successMsg + '</font>');
                $('#errMsg').show();

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    document.location.reload(true);
                }, 3000);
            }
            else {
                $('#errMsg').attr('class', 'errMsg');
                $('#errMsg').html(message.formatErrorMessage());
                $('#errMsg').show();
            }

            parent.Fanex.ExtAcc.UnBlockUI();
        }).error(function (data) {
            if (typeof console != 'undefined')
                console.log(data);

            parent.Fanex.ExtAcc.UnBlockUI();
        });
    };

    $scope.CheckAll = function () {
        var checkbox = document.getElementById('chkAll');
        var betTypeChexkboxes = document.getElementsByName('betTypes0');
        var betTypeChexkboxesDownlines = document.getElementsByName('betTypes1');

        if (checkbox.checked) {
            for (var i = 0; i < betTypeChexkboxes.length; i++) {
                if (betTypeChexkboxes[i].disabled) continue;

                betTypeChexkboxes[i].checked = true;

                if ($scope.betTypes[i].preVal) {
                    betTypeChexkboxesDownlines[i].checked = $scope.betTypes[i].preValDownline;
                    $scope.betTypes[i].isDisabledDownlines = false;
                }
                else {
                    betTypeChexkboxesDownlines[i].disabled = false;
                    $scope.betTypes[i].isDisabledDownlines = true;
                }

                $scope.betTypes[i].isDisabled = false;
            }
        }
        else {
            for (var i = 0; i < betTypeChexkboxes.length; i++) {
                if (betTypeChexkboxes[i].disabled) continue;

                betTypeChexkboxes[i].checked = false;
                betTypeChexkboxesDownlines[i].checked = false;

                if ($scope.betTypes[i].preVal) {
                    betTypeChexkboxesDownlines[i].disabled = true;
                }
                else {
                    betTypeChexkboxesDownlines[i].disabled = true;
                }

                $scope.betTypes[i].isDisabled = true;
                $scope.betTypes[i].isDisabledDownlines = true;
            }
        }
    };
};
