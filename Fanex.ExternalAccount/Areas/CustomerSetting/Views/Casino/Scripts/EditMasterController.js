/// <reference path="../../../Scripts/_references.js" />

function EditMasterController($scope, $element) {
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
            var min = gameGroup.n3;
            var max = gameGroup.s4;

            var selected = Math.min(max, newValue);

            gameGroup.s3 = selected;
            gameGroup.$s3.min = gameGroup.$s3.max = gameGroup.$s3.selected = selected;
            gameGroup.$s3 = toPTSource(gameGroup.$s3);

            var maxM3 = max - selected;
            var m3 = gameGroup.$m3.selected;
            var m3Group = Math.min(maxM3, m3).ptRound();
            m3 = Math.min(maxM3, m3).ptRound();

            gameGroup.$m3.l4 = gameGroup.l4 = gameGroup.is3 ? gameGroup.s4 : (gameGroup.s4 - gameGroup.s3).ptRound();
            gameGroup.m3 = m3;
            gameGroup.$m3.min = gameGroup.$m3.max = gameGroup.$m3.selected = m3;
            gameGroup.$m3 = toPTSource(gameGroup.$m3);
            gameGroup.$m3.max = gameGroup.$m3.l4;

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
                    gameType.$s3 = toPTSource(gameType.$s3);

                    maxM3 = max - selected;
                    m3 = gameType.$m3.selected;

                    // Set warning for m3 if break pt rule
                    gameType.$m3.warning = m3Group < min || m3Group > maxM3.ptRound();

                    m3 = Math.min(maxM3, m3).ptRound();
                    gameType.$m3.l4 = gameType.is3 ? gameType.s4 : (gameType.s4 - gameType.s3).ptRound();
                    gameType.m3 = m3;
                    gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = m3;
                    gameType.$m3 = toPTSource(gameType.$m3);
                    gameType.$m3.max = gameType.$m3.l4;
                }
            }
        });

        $scope.$watch("setting.is3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Update min, max for m3
            gameGroup.$m3.min = 0;
            var selected = gameGroup.$m3.selected;
            var max = newValue ? gameGroup.s4.ptRound() : (gameGroup.s4 - gameGroup.s3).ptRound();
            selected = selected > max ? max : selected;
            gameGroup.m3 = gameGroup.$m3.min = gameGroup.$m3.max = gameGroup.$m3.selected = selected.ptRound();

            if (newValue) {
                gameGroup.max = gameGroup.s4;
            }
            else {
                var max = gameGroup.s4 - gameGroup.s3;

                if (selected > max) {
                    selected = max;
                }

                gameGroup.max = max;
            }

            toPTSource(gameGroup.$s3);
            toPTSource(gameGroup.$m3);

            // Update new is3 for game type            
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    gameType.is3 = newValue;

                    if (newValue)
                        gameType.$m3.max = gameType.s4;
                    else
                        gameType.$m3.max = gameType.s4 - gameType.s3;
                }
            }
        });

        $scope.$watch("setting.us3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking
            // Game game group auto PT
            var min = 0;
            var max = 0;
            var selected = 0;

            max = gameGroup.s4;
            selected = Math.min(max, newValue).ptRound();
            var us3Group = Math.min(max, newValue).ptRound();

            gameGroup.us3 = selected;
            gameGroup.$us3.min = gameGroup.$us3.max = gameGroup.$us3.selected = selected;
            gameGroup.$us3 = toPTSource(gameGroup.$us3);

            // Change game type auto PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;

                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.s4;
                    selected = Math.min(max, newValue).ptRound();

                    gameType.$us3.warning = us3Group < min || us3Group > max;

                    gameType.us3 = selected;
                    gameType.$us3.min = gameType.$us3.max = gameType.$us3.selected = selected;
                    gameType.$us3 = toPTSource(gameType.$us3);
                }
            }
        });

        $scope.$watch("setting.m3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT
            var min = 0;
            var max = gameGroup.is3 ? gameGroup.s4 : gameGroup.s4 - gameGroup.s3;
            var selected = Math.min(max, newValue).ptRound();
            var m3Group = Math.min(max, newValue).ptRound();

            gameGroup.m3 = selected;
            gameGroup.$m3.min = gameGroup.$m3.max = gameGroup.$m3.selected = selected;
            gameGroup.$m3 = toPTSource(gameGroup.$m3);

            var maxN3 = selected;
            var n3 = gameGroup.$n3.selected;
            var n3Group = Math.min(maxN3, n3).ptRound();

            n3 = Math.min(maxN3, n3).ptRound();
            gameGroup.n3 = n3;
            gameGroup.$n3.min = gameGroup.$n3.max = gameGroup.$n3.selected = n3;
            gameGroup.$n3 = toPTSource(gameGroup.$n3);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.is3 ? gameType.s4 : gameType.s4 - gameType.s3;
                    selected = Math.min(max, newValue).ptRound();

                    // Set warning for m3 if break pt rule
                    gameType.$m3.warning = m3Group < min || m3Group > max.ptRound();

                    gameType.m3 = selected;
                    gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = selected;
                    gameType.$m3 = toPTSource(gameType.$m3);

                    maxN3 = selected;

                    // Set warning for n3 if break pt rule
                    gameType.$n3.warning = n3Group < min || n3Group > maxN3.ptRound();

                    n3 = gameType.$n3.selected;
                    n3 = Math.min(maxN3, n3).ptRound();
                    gameType.n3 = n3;
                    gameType.$n3.min = gameType.$n3.max = gameType.$n3.selected = n3;
                    gameType.$n3 = toPTSource(gameType.$n3);
                }
            }
        });

        $scope.$watch("setting.n3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            // Change game group PT            
            var min = 0;
            var max = gameGroup.m3;
            var selected = Math.min(max, newValue).ptRound();
            var n3Group = selected;

            gameGroup.n3 = selected;
            gameGroup.$n3.min = gameGroup.$n3.max = gameGroup.$n3.selected = selected;
            gameGroup.$n3 = toPTSource(gameGroup.$n3);

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    max = gameType.m3;
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

            gameType.$m3.l4 = gameType.is3 ? gameType.s4 : (gameType.s4 - gameType.s3).ptRound();
            gameType.m3 = m3;
            gameType.$m3.min = gameType.$m3.max = gameType.$m3.selected = m3;
            gameType.$m3 = toPTSource(gameType.$m3);
            gameType.$m3.max = gameType.$m3.l4;

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