function LineInfoController($scope, $http) {
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

    $scope.Login = function (username) {
        $http({
            url: parent.resolveClientUrl('User/InternalLogin/?username=' + username),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            window.open(result.url, '_blank');
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.ViewUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};