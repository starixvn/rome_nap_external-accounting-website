/// <reference path="../../../Scripts/_references.js" />

function EditMemberController($scope, $element) {
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
        $scope.$watch("setting.m1", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.m2;
            var selected = Math.min(max, newValue).ptRound();
            var m2Group = Math.min(max, newValue).ptRound();

            gameGroup.m1 = selected;
            gameGroup.$m1.min = gameGroup.$m1.max = gameGroup.$m1.selected = selected;
            gameGroup.$m1 = toPTSource(gameGroup.$m1);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.m2;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for m3 if break pt rule
                    gameType.$m1.warning = m2Group < min || m2Group > max.ptRound();

                    gameType.m1 = selected;
                    gameType.$m1.min = gameType.$m1.max = gameType.$m1.selected = selected;
                    gameType.$m1 = toPTSource(gameType.$m1);
                }
            }
        });

        $scope.$watch("setting.co", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.aco;
            var selected = Math.min(max, newValue).ptRound();
            var acoGroup = Math.min(max, newValue).ptRound();

            gameGroup.co = selected;
            gameGroup.$co.min = gameGroup.$co.max = gameGroup.$co.selected = selected;
            gameGroup.$co = toPTSource(gameGroup.$co, false, true);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.aco;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for n3 if break pt rule
                    gameType.$co.warning = acoGroup < min || acoGroup > max.ptRound();

                    gameType.co = selected;
                    gameType.$co.min = gameType.$co.max = gameType.$co.selected = selected;
                    gameType.$co = toPTSource(gameType.$co, false, true);
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
        $scope.$watch("setting.m1", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.m2;
            selected = Math.min(max, newValue).ptRound();

            gameType.m1 = selected;
            gameType.$m1.min = gameType.$m1.max = gameType.$m1.selected = selected;
            gameType.$m1 = toPTSource(gameType.$m1);
        });

        $scope.$watch("setting.co", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.aco;
            selected = Math.min(max, newValue).ptRound();

            gameType.co = selected;
            gameType.$co.min = gameType.$co.max = gameType.$co.selected = selected;
            gameType.$co = toPTSource(gameType.$co, false, true);
        });
    }
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}