function DownlineListController($scope, $http) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.downlines = $scope.$parent.$parent.model.downlineList;
        $scope.model = $scope.downlines.downlines;
        $scope.pages = $scope.downlines.pages;
        $scope.maxHeight = '240px';
        $('#downlines').css('height', $scope.maxHeight);
        $('#downlines').css('overflow', 'auto');
    }
    else {
        $scope.model = null;
        $scope.maxHeight = 'auto';
    }

    $scope.Init = function () {
        $('#downlines').css('height', '240px');

        if ($scope.pages && $scope.pages.length > 1) {
            for (var i = 1; i <= $scope.pages.length; i++) {
                $('#link-' + i).removeClass('pageSelected');
            }

            var interval = setInterval(function () {
                if (document.getElementById('link-1')) {
                    clearInterval(interval);

                    $('#link-1').addClass('pageSelected');
                }
            }, 100);
        }
    };
};