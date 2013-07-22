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
            var min = 0;
            var max = gameGroup.s4 - gameGroup.s3 - gameGroup.m2;
            var selected = Math.min(max, newValue).ptRound();
            var m3Group = Math.min(max, newValue).ptRound();

            gameGroup.m3 = selected;
            gameGroup.$m3.min = gameGroup.$m3.max = gameGroup.$m3.selected = selected;
            gameGroup.$m3 = toPTSource(gameGroup.$m3);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.s4 - gameType.s3 - gameType.m2;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for m3 if break pt rule
                    gameType.$m3.warning = m3Group < min || m3Group > max.ptRound();

                    gameType.m3 = selected;
                    gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = selected;
                    gameType.$m3 = toPTSource(gameType.$m3);
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
        $scope.$watch("setting.s3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking            

            min = 0;
            max = gameType.s4;

            selected = Math.min(max, newValue).ptRound();

            gameType.s3 = selected;
            gameType.$s3.min = gameType.$s3.max = gameType.$s3.selected = selected;
            gameType.$s3 = toPTSource(gameType.$s3);

            maxM3 = max - selected;
            m3 = gameType.$m3.selected;

            m3 = Math.min(maxM3, m3).ptRound();
            gameType.m3 = m3;
            gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = m3;
            gameType.$m3 = toPTSource(gameType.$m3);

            maxN3 = m3;
            n3 = gameType.$n3.selected;

            n3 = Math.min(maxN3, n3).ptRound();
            gameType.n3 = n3;
            gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = n3;
            gameType.$n3 = toPTSource(gameType.$n3);
        });

        $scope.$watch("setting.is3", function (newValue, oldValue) {            
            // No need to watch, because it's already watched on render action
        });

        $scope.$watch("setting.us3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.s4;
            selected = Math.min(max, newValue).ptRound();

            gameType.us3 = selected;
            gameType.$us3.min = gameType.$us3.max = gameType.$us3.selected = selected;
            gameType.$us3 = toPTSource(gameType.$us3);
        });

        $scope.$watch("setting.m3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.is3 ? gameType.s4 : gameType.s4 - gameType.s3;
            selected = Math.min(max, newValue).ptRound();

            gameType.m3 = selected;
            gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = selected;
            gameType.$m3 = toPTSource(gameType.$m3);

            maxN3 = selected;
            n3 = gameType.$n3.selected;

            n3 = Math.min(maxN3, n3).ptRound();
            gameType.n3 = n3;
            gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = n3;
            gameType.$n3 = toPTSource(gameType.$n3);
        });

        $scope.$watch("setting.n3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.m3;
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