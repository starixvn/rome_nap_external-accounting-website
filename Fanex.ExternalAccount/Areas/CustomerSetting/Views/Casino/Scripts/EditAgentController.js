/// <reference path="../../../Scripts/_references.js" />

function EditAgentController($scope, $element) {
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
        $scope.$watch("setting.m2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = gameGroup.n2;
            var max = gameGroup.m3;

            var selected = Math.min(max, newValue);
//            var m2Group = Math.min(max, newValue).ptRound();

            gameGroup.m2 = selected;
            gameGroup.$m2.min = gameGroup.$m2.max = gameGroup.$m2.selected = selected;
            gameGroup.$m2 = toPTSource(gameGroup.$m2);

            var maxA2 = max - selected;
            var a2 = gameGroup.$a2.selected;
            var a2Group = Math.min(maxA2, a2).ptRound();
            a2 = Math.min(maxA2, a2).ptRound();

            gameGroup.$a2.l2 = gameGroup.l2 = gameGroup.im2 ? gameGroup.m3 : (gameGroup.m3 - gameGroup.m2).ptRound();
            gameGroup.a2 = a2;
            gameGroup.$a2.min = gameGroup.$a2.max = gameGroup.$a2.selected = a2;
            gameGroup.$a2 = toPTSource(gameGroup.$a2);
            gameGroup.$a2.max = gameGroup.$a2.l2;

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    min = 0;
                    max = gameType.m3;

                    // Set warning for m2 if break pt rule
                    gameType.$m2.warning = newValue < min || newValue > max.ptRound();

                    selected = Math.min(max, newValue).ptRound();

                    gameType.m2 = selected;
                    gameType.$m2.min = gameType.$m2.max = gameType.$m2.selected = selected;
                    gameType.$m2 = toPTSource(gameType.$m2);

                    maxA2 = max - selected;
                    a2 = gameType.$a2.selected;

                    // Set warning for a3 if break pt rule
                    gameType.$a2.warning = a2Group < min || a2Group > maxA2.ptRound();

                    a2 = Math.min(maxA2, a2).ptRound();
                    gameType.$a2.l2 = gameType.l2 = gameType.im2 ? gameType.m3 : (gameType.m3 - gameType.m2).ptRound();
                    gameType.a2 = a2;
                    gameType.$a2.min = gameType.$a2.max = gameType.$a2.selected = a2;
                    gameType.$a2 = toPTSource(gameType.$a2);
                    gameType.$a2.max = gameType.$a2.l2;
                }
            }
        });

        $scope.$watch("setting.im2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Update min, max for a2
            gameGroup.$a2.min = 0;
            var selected = gameGroup.$a2.selected;
            var max = newValue ? gameGroup.m3.ptRound() : (gameGroup.m3 - gameGroup.m2).ptRound();
            selected = selected > max ? max : selected;
            gameGroup.a2 = gameGroup.$a2.min = gameGroup.$a2.max = gameGroup.$a2.selected = selected.ptRound();

            if (newValue) {
                gameGroup.max = gameGroup.m3;
            }
            else {
                var max = gameGroup.m3 - gameGroup.m2;

                if (selected > max) {
                    selected = max;
                }

                gameGroup.max = max;
            }

            toPTSource(gameGroup.$m2);
            toPTSource(gameGroup.$a2);

            // Update new im2 for game type            
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    gameType.im2 = newValue;

                    if (newValue)
                        gameType.$a2.max = gameType.m3;
                    else
                        gameType.$a2.max = gameType.m3 - gameType.m2;
                }
            }
        });

        $scope.$watch("setting.um2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking
            // Game game group auto PT
            var min = 0;
            var max = 0;
            var selected = 0;

            max = gameGroup.m3;
            selected = Math.min(max, newValue).ptRound();
            var um2Group = Math.min(max, newValue).ptRound();

            gameGroup.um2 = selected;
            gameGroup.$um2.min = gameGroup.$um2.max = gameGroup.$um2.selected = selected;
            gameGroup.$um2 = toPTSource(gameGroup.$um2);

            // Change game type auto PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;

                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.m3;
                    selected = Math.min(max, newValue).ptRound();

                    gameType.$um2.warning = um2Group < min || um2Group > max;

                    gameType.um2 = selected;
                    gameType.$um2.min = gameType.$um2.max = gameType.$um2.selected = selected;
                    gameType.$um2 = toPTSource(gameType.$um2);
                }
            }
        });

        $scope.$watch("setting.a2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.im2 ? gameGroup.m3 : gameGroup.m3 - gameGroup.m2;
            var selected = Math.min(max, newValue).ptRound();
            var a2Group = Math.min(max, newValue).ptRound();

            gameGroup.a2 = selected;
            gameGroup.$a2.min = gameGroup.$a2.max = gameGroup.$a2.selected = selected;
            gameGroup.$a2 = toPTSource(gameGroup.$a2);

            var maxN2 = selected;
            var n2 = gameGroup.$n2.selected;
            var n2Group = Math.min(maxN2, n2).ptRound();

            n2 = Math.min(maxN2, n2).ptRound();
            gameGroup.n2 = n2;
            gameGroup.$n2.min = gameGroup.$n2.max = gameGroup.$n2.selected = n2;
            gameGroup.$n2 = toPTSource(gameGroup.$n2);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.im2 ? gameType.m3 : gameType.m3 - gameType.m2;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for m2 if break pt rule
                    gameType.$a2.warning = a2Group < min || a2Group > max.ptRound();

                    gameType.a2 = selected;
                    gameType.$a2.min = gameType.$a2.max = gameType.$a2.selected = selected;
                    gameType.$a2 = toPTSource(gameType.$a2);

                    maxN2 = selected;

                    // Set warning for a3 if break pt rule
                    gameType.$n2.warning = n2Group < min || n2Group > maxN2.ptRound();

                    n2 = gameType.$n2.selected;
                    n2 = Math.min(maxN2, n2).ptRound();
                    gameType.n2 = n2;
                    gameType.$n2.min = gameType.$n2.max = gameType.$n2.selected = n2;
                    gameType.$n2 = toPTSource(gameType.$n2);
                }
            }
        });

        $scope.$watch("setting.n2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT            
            var min = 0;
            var max = gameGroup.a2;
            var selected = Math.min(max, newValue).ptRound();
            var n2Group = selected;

            gameGroup.n2 = selected;
            gameGroup.$n2.min = gameGroup.$n2.max = gameGroup.$n2.selected = selected;
            gameGroup.$n2 = toPTSource(gameGroup.$n2);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.a2;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for n3 if break pt rule
                    gameType.$n2.warning = n2Group < min || n2Group > max.ptRound();

                    gameType.n2 = selected;
                    gameType.$n2.min = gameType.$n2.max = gameType.$n2.selected = selected;
                    gameType.$n2 = toPTSource(gameType.$n2);
                }
            }
        });

        $scope.$watch("setting.aco", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.mco;
            var selected = Math.min(max, newValue).ptRound();
            var acoGroup = Math.min(max, newValue).ptRound();

            gameGroup.aco = selected;
            gameGroup.$aco.min = gameGroup.$aco.max = gameGroup.$aco.selected = selected;
            gameGroup.$aco = toPTSource(gameGroup.$aco, false, true);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.mco;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for n3 if break pt rule
                    gameType.$aco.warning = acoGroup < min || acoGroup > max.ptRound();

                    gameType.aco = selected;
                    gameType.$aco.min = gameType.$aco.max = gameType.$aco.selected = selected;
                    gameType.$aco = toPTSource(gameType.$aco, false, true);
                }
            }
        });
    }
}

function GameTypeController($scope) {
    $scope.gameType = $scope.$parent.gameType;
    var gameType = $scope.setting = $scope.gameType;
    
    /* 
    Watch changing of m2, a2, n2, aco 
    Control position taking logic here, each bet type have it's own scope
    */

    $scope.setupWatch = function () {
        $scope.$watch("setting.m2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameType.m3;
            var selected = Math.min(max, newValue).ptRound();

            gameType.m2 = selected;
            gameType.$m2.min = gameType.$m2.max = gameType.$m2.selected = selected;
            gameType.$m2 = toPTSource(gameType.$m2);

            var maxA2 = (max - selected).ptRound();
            var a2 = gameType.$a2.selected;

            a2 = Math.min(maxA2, a2).ptRound();
            gameType.$a2.l2 = gameType.im2 ? gameType.m3 : (gameType.m3 - gameType.m2).ptRound();
            gameType.a2 = a2;
            gameType.$a2.min = gameType.$a2.max = gameType.$a2.selected = a2;
            gameType.$a2 = toPTSource(gameType.$a2);
            gameType.$a2.max = gameType.$a2.l2;

            maxN2 = a2;
            n2 = gameType.$a2.selected;

            n2 = Math.min(maxN2, n2).ptRound();
            gameType.n2 = n2;
            gameType.$a2.min = gameType.$a2.max = gameType.$a2.selected = n2;
            gameType.$a2 = toPTSource(gameType.$a2);
        });

        $scope.$watch("setting.im2", function (newValue, oldValue) {
            // No need to watch, because it's already watched on render action
        });

        $scope.$watch("setting.um2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            max = gameType.m3;
            selected = Math.min(max, newValue).ptRound();

            gameType.um2 = selected;
            gameType.$um2.min = gameType.$um2.max = gameType.$um2.selected = selected;
            gameType.$um2 = toPTSource(gameType.$um2);
        });

        $scope.$watch("setting.a2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameType.m3 - gameType.m2;
            var selected = Math.min(max, newValue).ptRound();

            gameType.a2 = selected;
            gameType.$a2.min = gameType.$a2.max = gameType.$a2.selected = selected;
            gameType.$a2 = toPTSource(gameType.$a2);

            var maxN2 = selected;
            var n2 = gameType.$n2.selected;

            n2 = Math.min(maxN2, n2).ptRound();
            gameType.n2 = n2;
            gameType.$n2.min = gameType.$n2.max = gameType.$n2.selected = n2;
            gameType.$n2 = toPTSource(gameType.$n2);            
        });

        $scope.$watch("setting.n2", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT            
            var min = 0;
            var max = gameType.a2;
            var selected = Math.min(max, newValue).ptRound();

            gameType.n2 = selected;
            gameType.$n2.min = gameType.$n2.max = gameType.$n2.selected = selected;
            gameType.$n2 = toPTSource(gameType.$n2);
        });

        $scope.$watch("setting.aco", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameType.mco;
            var selected = Math.min(max, newValue).ptRound();

            gameType.aco = selected;
            gameType.$aco.min = gameType.$aco.max = gameType.$aco.selected = selected;
            gameType.$aco = toPTSource(gameType.$aco, false, true);
        });
    }
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}