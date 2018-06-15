function ViewInfoController($scope, $http) {
    $scope.className = 'fiftyPercent';

    if ($scope.$parent.model) {
        $scope.personalInfo = $scope.$parent.model.personalInfo;
        $scope.model = $scope.$parent.model;

        $scope.personalInfoHeight = $('#customerInfo').height();

        if ($scope.personalInfo.roleId.value == 2) {
            $scope.className = 'thirdtyThreePercent';
        }
        else if ($scope.personalInfo.roleId.value == 1) {
            $scope.className = 'onehundredPercent';
        }
    }
    else {
        $scope.model = null;
    }

    $scope.GetBalance = function (custId) {
        parent.Fanex.ExtAcc.BlockUI();
        $.ajax({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post'
        })
        .success(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            parent.$('#contentSplitter').html(result);
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
        });
    };    
};