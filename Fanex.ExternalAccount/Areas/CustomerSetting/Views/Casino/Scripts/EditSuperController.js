/// <reference path="../../../Scripts/_references.js" />

function EditSuperController($scope, $element) {
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
        $scope.$watch("setting.s3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.s4;

            var selected = Math.min(max, newValue);

            gameGroup.s3 = selected;
            gameGroup.$s3.min = gameGroup.$s3.max = gameGroup.$s3.selected = selected;
            gameGroup.$s3 = toPTSource(gameGroup.$s3);

            var n3 = gameGroup.n3;
            var maxN3 = n3 <= selected ? n3 : selected;
            gameGroup.n3 = maxN3;
            gameGroup.$n3.min = gameGroup.$n3.max = gameGroup.$n3.selected = maxN3;
            gameGroup.$n3 = toPTSource(gameGroup.$n3);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    min = 0;
                    max = gameType.s4;

                    // Set warning for s3 if break pt rule
                    gameType.$s3.warning = newValue < min || newValue > max.ptRound();

                    selected = Math.min(max, newValue).ptRound();

                    gameType.s3 = selected;
                    gameType.$s3.min = gameType.$s3.max = gameType.$s3.selected = selected;
                    gameType.$s3.warning = selected > max || selected < min;
                    gameType.$s3 = toPTSource(gameType.$s3);

                    n3 = gameType.n3;
                    maxN3 = n3 <= selected ? n3 : selected;
                    gameType.n3 = maxN3;
                    gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = maxN3;
                    gameType.$n3.warning = maxN3 > max || maxN3 < min;
                    gameType.$n3 = toPTSource(gameType.$n3);
                }
            }
        });

        $scope.$watch("setting.n3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT            
            var min = 0;
            var max = gameGroup.n3;
            var selected = Math.min(max, newValue).ptRound();
            var n3Group = selected;

            gameGroup.n3 = selected;
            gameGroup.$n3.min = gameGroup.$n3.max = gameGroup.$n3.selected = selected;
            gameGroup.$n3 = toPTSource(gameGroup.$n3);
            var n4 = (max - selected).ptRound();
            gameGroup.n4 = n4;

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.s3;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for n3 if break pt rule
                    gameType.$n3.warning = n3Group < min || n3Group > max.ptRound();

                    gameType.n3 = selected;
                    gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = selected;
                    gameType.$n3 = toPTSource(gameType.$n3);
                }
            }
        });

        $scope.$watch("setting.mco", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.sco;
            var selected = Math.min(max, newValue).ptRound();
            var mcoGroup = Math.min(max, newValue).ptRound();

            gameGroup.mco = selected;
            gameGroup.$mco.min = gameGroup.$mco.max = gameGroup.$mco.selected = selected;
            gameGroup.$mco = toPTSource(gameGroup.$mco, false, true);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.sco;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for n3 if break pt rule
                    gameType.$mco.warning = mcoGroup < min || mcoGroup > max.ptRound();

                    gameType.mco = selected;
                    gameType.$mco.min = gameType.$mco.max = gameType.$mco.selected = selected;
                    gameType.$mco = toPTSource(gameType.$mco, false, true);
                }
            }
        });
    }
}

function GameTypeController($scope) {
    $scope.gameType = $scope.$parent.gameType;
    var gameType = $scope.setting = $scope.gameType;
    
    /* 
    Watch changing of s3, n3, mco 
    Control position taking logic here, each bet type have it's own scope
    */

    $scope.setupWatch = function () {
        $scope.$watch("setting.s3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking            

            min = 0;
            max = gameType.s4;

            selected = Math.min(max, newValue).ptRound();

            gameType.s3 = selected;
            gameType.$s3.min = gameType.$s3.max = gameType.$s3.selected = selected;
            gameType.$s3 = toPTSource(gameType.$s3);

            var n3 = gameType.$n3.selected;
            var maxN3 = n3 <= selected ? n3 : selected;
            
            gameType.n3 = maxN3;
            gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = maxN3;
            gameType.$n3 = toPTSource(gameType.$n3);
        });

        $scope.$watch("setting.n3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.n3;
            selected = Math.min(max, newValue).ptRound();

            gameType.n3 = selected;
            gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = selected;
            gameType.$n3 = toPTSource(gameType.$n3);
        });

        $scope.$watch("setting.mco", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.sco;
            selected = Math.min(max, newValue).ptRound();

            gameType.mco = selected;
            gameType.$mco.min = gameType.$mco.max = gameType.$mco.selected = selected;
            gameType.$mco = toPTSource(gameType.$mco, false, true);
        });
    }
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}