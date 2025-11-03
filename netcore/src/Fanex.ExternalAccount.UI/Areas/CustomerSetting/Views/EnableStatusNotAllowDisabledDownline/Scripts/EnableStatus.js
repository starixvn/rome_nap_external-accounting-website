angular.module('enableStatusApp', []);

angular.element($('#ng-app')).ready(function () {
    $('#ng-app').show();
});

function EnableStatusController($scope) {
    $scope.model = model;
    $scope.context = model.requestContext;
    $scope.entities = model.entities;
    $scope.isDisabledAll = true;
    $scope.checkSyncProducts = [21, 18];
    $scope.dirtyCount = 0;
    var length = $scope.entities.length;
    for (var i = 0; i < length; i++) {
        if ((!$scope.entities[i].disabledUplines && !$scope.entities[i].hide) || ($scope.entities[i].synced && $($scope.checkSyncProducts).index($scope.context.product) != -1)) {
            $scope.isDisabledAll = false;
            break;
        }
    }

    $scope.Init = function () {     

        $.ajaxSetup({
            beforeSend: function (xhr) {
                var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
                if (antiXSRFToken.length > 0) {
                    xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
                }
            }
        });

        // Check hidden status of product
        var length = $scope.entities.length;
        for (var i = 0; i < length; i++) {
            $scope.entities[i].hide =
                ($scope.entities[i].hide && $($scope.checkSyncProducts).index($scope.context.product) == -1) ||
                (!$scope.entities[i].synced && $($scope.checkSyncProducts).index($scope.context.product) != -1);
        }

        if ($scope.context.newMode) {
            $('#submit').addClass('disabledButton');
            $('#submit').attr('disabled', true);
        }

        $('#title').html('Enable ' + model.requestContext.productId);
        $scope.sortedModel = groupBy($scope.entities, function (item) {
            return [item.sportTypeName];
        });

        var allChecked = true;
        for (var i = 0; i < $scope.entities.length; i++) {
            if (!$scope.entities[i].hide && (($scope.entities[i].disabled && !$scope.entities[i].disabledUplines) || $scope.isDisabledAll)) {
                allChecked = false;
                break;
            }
        }

        if (allChecked)
            document.getElementById('chkAll').checked = true;
    };

    $scope.Update = function (onCompleted) {
        Fanex.CustomerSetting.Core.BlockUI();
        if ($scope.ValidateParams()) {
            $.ajax({
                dataType: 'json',
                type: 'post',
                url: resolveClientUrl('CustomerSetting/EnableStatus/ProductStatus/Update'),
                data: {
                    model: JSON.stringify(model)
                }
            })
            .success(function (data) {
                var success = true;
                var message = '';

                $(data).each(function (i, error) {
                    if (error.ErrorCode == '5003') {
                        message = error.ErrorMessage;
                        success = false;
                    }
                    else if (!error.isSuccess) {
                        success = false;
                        message += (parseInt(error.errorCode) == 600 ? errMsgAccountDisabled : error.errorMessage) + '<br />';
                    }
                });

                if (data.errorCode == "0" || data.errorCode == "" || success) {
                    $('#errMsg').attr('class', 'successMsg');
                    $('#errMsg').html('<font color="Blue">' + successMsg + '</font>');
                    $('#errMsg').show();
                    if (!model.entities[0].preValDownline !== model.entities[0].disabledDownlines && model.requestContext.level >= 2) {
                        model.warning = msgWarningUpdateSettingForEnabledDownlines;
                        $scope.$apply();
                    }

                    $('#submit').remove();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        if (model.reloadAfterUpdate) {
                            if (parent.document.getElementById(model.iframeId))
                                parent.document.getElementById(model.iframeId).src = parent.document.getElementById(model.iframeId).src;
                        }

                        if (model.closePopupFunction != '') {
                            document.write('<script type="text/javascript">' + model.closePopupFunction + '</script>');
                        }
                        else if (model.reloadAfterUpdate) {
                            document.location.reload(true);
                        }

                    }, 5000);
                }
                else {
                    $('#errMsg').attr('class', 'errMsg');
                    $('#errMsg').html(message.formatErrorMessage());
                    $('#errMsg').show();
                }

                Fanex.CustomerSetting.Core.UnBlockUI();

                if (typeof onCompleted == 'function') {
                    onCompleted(data);
                }
            }).error(function (data) {
                if (data.status == 0) {
                    document.location.reload(true);
                }

                if (typeof console != 'undefined')
                    console.log(data);

                Fanex.CustomerSetting.Core.BlockUI();

                if (typeof onCompleted == 'function') {
                    onCompleted(data);
                }
            });
        } else {
            Fanex.CustomerSetting.Core.UnBlockUI();
        }
    };

    $scope.CheckAll = function () {
        var checkbox = document.getElementById('chkAll');
        var betTypeChexkboxes = document.getElementsByName('betTypes0');
        var betTypeChexkboxesDownlines = document.getElementsByName('betTypes1');
        var isDirty = false;

        if (checkbox.checked) {
            for (var i = 0; i < betTypeChexkboxes.length; i++) {
                if (betTypeChexkboxes[i].disabled || betTypeChexkboxes[i].id.indexOf('true') != -1) continue;

                betTypeChexkboxes[i].checked = true;

                if ($scope.entities[i].preVal) {
                    betTypeChexkboxesDownlines[i].checked = $scope.entities[i].preValDownline;
                    $scope.entities[i].disabledDownlines = false;
                }
                else {
                    betTypeChexkboxesDownlines[i].checked = false;
                    betTypeChexkboxesDownlines[i].disabled = false;
                    $scope.entities[i].disabledDownlines = true;
                }

                $scope.entities[i].disabled = false;

                if (checkbox.checked == $scope.entities[i].preVal) {
                    $scope.entities[i].isDirty = false;
                }
                else {
                    isDirty = true;
                    $scope.entities[i].isDirty = true;
                }
            }
        }
        else {
            for (var i = 0; i < betTypeChexkboxes.length; i++) {
                if (betTypeChexkboxes[i].disabled || betTypeChexkboxes[i].id.indexOf('true') != -1) continue;

                betTypeChexkboxes[i].checked = false;
                betTypeChexkboxesDownlines[i].checked = false;

                if ($scope.entities[i].preVal) {
                    betTypeChexkboxesDownlines[i].disabled = true;
                }
                else {
                    betTypeChexkboxesDownlines[i].disabled = true;
                }

                $scope.entities[i].disabled = true;
                $scope.entities[i].disabledDownlines = true;

                if (checkbox.checked == $scope.entities[i].preVal) {
                    $scope.entities[i].isDirty = false;
                }
                else {
                    isDirty = true;
                    $scope.entities[i].isDirty = true;
                }
            }
        }

        if ($scope.context.newMode) {
            if (isDirty) {
                $('#submit').removeClass('disabledButton');
                $('#submit').attr('disabled', false);
            }
            else {
                $('#submit').addClass('disabledButton');
                $('#submit').attr('disabled', true);
            }
        }
    };

    $scope.EnableStatusChange = function (scope) {
        if (!scope.item.enable) {
            scope.item.disabledDownlines = true;
            scope.item.enableDownlines = false;
        }
        else {
            scope.item.disabledDownlines = !scope.item.preValDownlines;
            scope.item.enableDownlines = scope.item.preValDownlines;
        }
    }

    // Every time value of Enable or Enable Downlines has been changed, +1 to CountDirty &  -1 vice versa.
    // Anytime, if CountDirty == 0 => nothing change & vice versa.
    $scope.CountDirty = function (currentVal, preVal, scope) {
        $scope.dirtyCount = currentVal != preVal ? $scope.dirtyCount + 1 : $scope.dirtyCount - 1;
        scope.item.isDirty = $scope.dirtyCount > 0;
    }

    $scope.ValidateParams = function () {
        if (model.requestContext.level >= 2) {
            for (var i = 0 ; i < model.entities.length; i++) {
                var entity = model.entities[i];
                if (entity.disabled && !entity.disabledDownlines) {
                    model.errorMsg = msgErrorProductIsNotOfferedForThisLine;
                    return false;
                } else if ((entity.disabled && entity.preVal != entity.enable) ||
                    (entity.disabledDownlines && entity.preValDownlines != entity.enableDownlines)) {
                    model.errorMsg = msgErrorNotAllowedDisableDownline;
                    return false;
                }
            }
        }

        return true;
    }
}

function groupBy(array, f) {
    var groups = {};
    array.forEach(function (o) {
        var group = JSON.stringify(f(o));
        groups[group] = groups[group] || [];
        groups[group].sportTypeName = o.sportTypeName;
        groups[group].push(o);
    });
    return Object.keys(groups).map(function (group) {
        return groups[group];
    })
}