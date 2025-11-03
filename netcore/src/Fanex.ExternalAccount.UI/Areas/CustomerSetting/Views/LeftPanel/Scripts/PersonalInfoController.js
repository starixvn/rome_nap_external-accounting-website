function PersonalInfoController($scope, $http) {
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

    $scope.TooglePersonalInfo = function () {
        if ($('#personalInfo').hasClass('icon-collapse')) {
            $('#personalInfo').removeClass('icon-collapse');
            $('#personalInfo').addClass('icon-expand');
            //$('#customerInfo').height(25);
            $('#UserInfoContainer').slideUp("fast");
        }
        else {
            $('#personalInfo').addClass('icon-collapse');
            $('#personalInfo').removeClass('icon-expand');
            //$('#customerInfo').height(140);
            $('#UserInfoContainer').slideDown("fast");
        }
    };

    $scope.ViewUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};