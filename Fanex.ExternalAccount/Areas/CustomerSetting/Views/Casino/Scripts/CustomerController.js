var IsMaster = false;
var IsAgent = false;
var IsMember = false;
var UplineRoleId = 4;

function CustomerController($scope, $element) {
    $scope.CustomerModel = customerModel;
    $scope.Enabled = !customerModel.casinoDisabled;
    $scope.Level = '';
    $scope.IsMaster = false;
    $scope.IsAgent = false;
    $scope.IsMember = false;
    $scope.IsSuper = false;

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
    }
}