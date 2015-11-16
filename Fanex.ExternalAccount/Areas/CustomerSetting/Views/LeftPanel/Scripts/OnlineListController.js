function OnlineListController($scope) {
    if ($scope.$parent && $scope.$parent.model) {
        $scope.model = $scope.$parent.model;
        $scope.onlineList = $scope.$parent.model.onlineList.onlines;
    }
    else {
        $scope.model = null;
        $scope.onlineList = null;
    }

    $scope.Init = function () {
        if ($scope.onlineList && $scope.onlineList.length > 0) {
            for (var i = 0; i < $scope.onlineList.length; i++) {
                $scope.onlineList[i].index = i + 1;
            }
        }
    };
};