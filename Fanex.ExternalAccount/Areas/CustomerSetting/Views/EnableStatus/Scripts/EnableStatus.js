angular.module('enableStatusApp', []);

angular.element($('#ng-app')).ready(function () {
    $('#ng-app').show();
});

function EnableStatusController($scope) {
    $scope.model = model;
    $scope.context = model.requestContext;
    $scope.entities = model.entities;
    $scope.isDisabledAll = true;

    var length = $scope.entities.length;
    for (var i = 0; i < length; i++) {
        if (!$scope.entities[i].disabledUplines && !$scope.entities[i].hide) {
            $scope.isDisabledAll = false;
            break;
        }        
    }

    $scope.Init = function () {
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

    $scope.CheckStatus = function (preVal, scope) {
        var betTypesCheckboxes = document.getElementsByName('betTypes0');
        var allChecked = true;

        scope.item.isDirty = preVal != scope.item.enable;

        if ($scope.context.newMode) {
            var dirty = false;
            for (var i = 0; i < $scope.entities.length; i++) {
                if ($scope.entities[i].isDirty) {
                    dirty = true;
                    break;
                }
            }

            if (!dirty) {
                $('#submit').addClass('disabledButton');
                $('#submit').attr('disabled', true);
            }
            else {
                $('#submit').removeClass('disabledButton');
                $('#submit').attr('disabled', false);
            }
        }

        if (preVal) {
            if (scope.item.enable) {
                scope.item.enableDownline = scope.item.preValDownline;
                scope.item.disableDownline = true;

                scope.item.disabled = false;
                scope.item.disabledUplines = false;
                scope.item.disabledDownlines = false;

                for (var i = 0; i < betTypesCheckboxes.length; i++) {
                    if (betTypesCheckboxes[i].id.indexOf('true') == -1 && !betTypesCheckboxes[i].checked && !betTypesCheckboxes[i].disabled) {
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

                scope.item.disabled = true;
                scope.item.disabledUplines = false;
                scope.item.disabledDownlines = true;
            }
        }
        else {
            if (scope.item.enable) {
                scope.item.enableDownline = false;
                scope.item.disableDownline = false;

                scope.item.disabled = false;
                scope.item.disabledUplines = false;
                scope.item.disabledDownlines = scope.item.preValDownline ? false : true;

                for (var i = 0; i < betTypesCheckboxes.length; i++) {
                    if (betTypesCheckboxes[i].id.indexOf('true') == -1 && !betTypesCheckboxes[i].checked && !betTypesCheckboxes[i].disabled) {
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

                scope.item.disabled = true;
                scope.item.disabledUplines = false;
                scope.item.disabledDownlines = true;
            }
        }

        document.getElementById(scope.item.betTypeId).disabled = scope.item.disableDownline;
        document.getElementById(scope.item.betTypeId).checked = scope.item.enableDownline;
    };

    $scope.Update = function (onCompleted) {
        Fanex.CustomerSetting.Core.BlockUI();

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
                if (!error.isSuccess) {
                    success = false;
                    message += (parseInt(error.errorCode) == 600 ? errMsgAccountDisabled : error.errorMessage) + '<br />';
                }
            });

            if (data.errorCode == "0" || data.errorCode == "" || success) {
                $('#errMsg').attr('class', 'successMsg');
                $('#errMsg').html('<font color="Blue">' + successMsg + '</font>');
                $('#errMsg').show();

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

                }, 3000);
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
            if (typeof console != 'undefined')
                console.log(data);

            Fanex.CustomerSetting.Core.BlockUI();

            if (typeof onCompleted == 'function') {
                onCompleted(data);
            }
        });
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

    $scope.CheckDownline = function (id, scope) {
        var checkboxDownlines = document.getElementById(id);
        scope.item.disabledDownlines = !checkboxDownlines.checked;
    };
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