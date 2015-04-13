/// <reference path="../../../Scripts/_references.js" />

function EditMemberByMasterController($scope, $element) {
    var e = $element[0];
    var p = e.parentNode;

    p.removeChild(e);

    $scope.customer = customerSetting;

    $scope.isDisableAutoMPT = customerModel.isDisableAutoMPT;

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


    $scope.$watch("setting.im1", function (newValue, oldValue) {
        if (newValue == oldValue) return; // Avoid $apply checking

        var min = gameGroup.n2;
        var max = gameGroup.s4 - gameGroup.m2;
        var selected = gameGroup.m3;

        if (!newValue && selected >= min && selected <= max) {
            $scope.gameGroup.$m3.min = 0;
            $scope.gameGroup.$m3.max = gameGroup.s4 - gameGroup.m2;
            toPTSource($scope.gameGroup.$m3);
        }

        // Update new im2 for game type            
        if ($scope.customer.isPtByGameType) {
            var c = $scope.gameGroup.gameTypes.length;
            for (var i = 0; i < c; i++) {
                var gameType = $scope.gameGroup.gameTypes[i];

                gameType.im1 = newValue;

                if (newValue)
                    gameType.$m3.max = gameType.s4;
                else
                    gameType.$m3.max = gameType.s4 - gameType.m2;
            }
        }
    });

    $scope.$watch("setting.um1", function (newValue, oldValue) {
        if (newValue == oldValue) return; // Avoid $apply checking
        // Game game group auto PT
        var min = 0;
        var max = 0;
        var selected = 0;

        max = gameGroup.s4;
        selected = Math.min(max, newValue).ptRound();
        var us3Group = Math.min(max, newValue).ptRound();

        gameGroup.um1 = selected;
        gameGroup.$um1.min = gameGroup.$um1.max = gameGroup.$um1.selected = selected;
        gameGroup.$um1 = toPTSource(gameGroup.$um1);

        // Change game type auto PT
        if ($scope.customer.isPtByGameType) {
            var c = $scope.gameGroup.gameTypes.length;

            for (var i = 0; i < c; i++) {
                var gameType = $scope.gameGroup.gameTypes[i];

                max = gameType.s4;
                selected = Math.min(max, newValue).ptRound();

                gameType.$um1.warning = us3Group < min || us3Group > max;

                gameType.um1 = selected;
                gameType.$um1.min = gameType.$um1.max = gameType.$um1.selected = selected;
                gameType.$um1 = toPTSource(gameType.$um1);
            }
        }
    });
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

    $scope.$watch("setting.im1", function (newValue, oldValue) {
        // No need to watch, because it's already watched on render action
    });

    $scope.$watch("setting.um1", function (newValue, oldValue) {
        if (newValue == oldValue) return; // Avoid $apply checking

        max = gameType.s4;
        selected = Math.min(max, newValue).ptRound();

        gameType.um1 = selected;
        gameType.$um1.min = gameType.$um1.max = gameType.$um1.selected = selected;
        gameType.$um1 = toPTSource(gameType.$um1);
    });
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}