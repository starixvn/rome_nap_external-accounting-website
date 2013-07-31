/// <reference path="../../../Scripts/_references.js" />

function EditMemberByMasterController($scope, $element) {
    var e = $element[0];
    var p = e.parentNode;

    p.removeChild(e);

    $scope.customer = customerSetting;

    $scope.$on('rendered', function () {
        p.appendChild(e);
    });
}

function GameGroupController($scope) {
    $scope.gameGroup = $scope.$parent.gameGroup;
    var gameGroup = $scope.setting = $scope.gameGroup;

    $scope.setupWatch = function () {
        $scope.$watch("setting.m3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var max = gameGroup.s4;
            var selected = Math.min(max, newValue).ptRound();
            var m3Group = Math.min(max, newValue).ptRound();

            if (max < 0) {
                gameGroup.m3.breakpt = true;
                gameGroup.$m3.breakpt = true;
            }
            else {
                gameGroup.m3 = selected;
                gameGroup.max = max;
                gameGroup.$m3.min = gameGroup.$m3.max = gameGroup.$m3.selected = selected;
                toPTSource(gameGroup.$m3);
            }
            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.s4;

                    if (max < 0) {
                        gameType.m3.breakpt = true;
                        gameType.$m3.breakpt = true;
                    }
                    else {
                        selected = Math.min(max, newValue).ptRound();

                        // Set warning for m3 if break pt rule
                        gameType.$m3.warning = m3Group < 0 || m3Group > max.ptRound();

                        gameType.max = max;
                        gameType.m3 = selected;
                        gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = selected;
                        toPTSource(gameType.$m3);
                    }
                }
            }
        });
    }
}

function GameTypeController($scope) {
    $scope.gameType = $scope.$parent.gameType;
    var gameType = $scope.setting = $scope.gameType;
    
    /* 
    Watch changing of s3, m3, is3 
    Control position taking logic here, each bet type have it's own scope
    */

    $scope.setupWatch = function () {
        $scope.$watch("setting.m3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.s4;

            if (max < 0 || gameType.breakpt) {
                gameType.m3.breakpt = true;
                gameType.$m3.breakpt = true;
            }
            else {
                selected = Math.min(max, newValue).ptRound();

                gameType.max = max;
                gameType.m3 = selected;
                gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = selected;
                //toPTSource(gameType.$m3);
            }
        });
    }
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}