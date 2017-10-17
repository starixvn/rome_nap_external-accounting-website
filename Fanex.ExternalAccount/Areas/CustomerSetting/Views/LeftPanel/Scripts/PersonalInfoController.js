function PersonalInfoController($scope, $http) {
    $scope.className = 'fiftyPercent';

    if ($scope.$parent.model) {
        $scope.personalInfo = $scope.$parent.model.personalInfo;
        $scope.model = $scope.$parent.model;
                
        if ($scope.personalInfo.isShowProducts.length > 0) {
            parent.showAllbet = $scope.personalInfo.isShowProducts[0].value;
            parent.showVoidbridge = $scope.personalInfo.isShowProducts[2].value;
            parent.showNG2 = $scope.personalInfo.isShowProducts[4].value;
            parent.showFishingWorld = $scope.personalInfo.isShowProducts[6].value;
            parent.showLottery = $scope.personalInfo.isShowProducts[8].value;
            parent.showBullfighting = $scope.personalInfo.isShowProducts[12].value;
            
            if ($scope.personalInfo.roleId.value == 4) {
                parent.showAllbetBetting = $scope.personalInfo.isShowProducts[1].value;
                parent.showVoidbridgeBetting = $scope.personalInfo.isShowProducts[3].value;
                parent.showNG2Betting = $scope.personalInfo.isShowProducts[5].value;
                parent.showLotteryBetting = $scope.personalInfo.isShowProducts[9].value;
                parent.showBullfightingBetting = $scope.personalInfo.isShowProducts[13].value;
            }
        }

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
        if ($('#personalInfo').hasClass('userDown')) {
            $('#personalInfo').removeClass('userDown');
            $('#personalInfo').addClass('userUp');
            $('#customerInfo').height(25);
        }
        else {
            $('#personalInfo').addClass('userDown');
            $('#personalInfo').removeClass('userUp');
            $('#customerInfo').height(140);
        }
    };

    $scope.ViewUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};