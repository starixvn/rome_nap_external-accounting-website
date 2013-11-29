/// <reference path="../../../Scripts/_references.js" />

function EditMemberBySuperController($scope, $element) {
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

            if (selected < min || selected > max) {
                gameGroup.s3.breakpt = true;
                gameGroup.$s3.breakpt = true;
            }
            else {
                gameGroup.s3 = selected;
                gameGroup.$s3.min = gameGroup.$s3.max = gameGroup.$s3.selected = selected;
                gameGroup.$s3 = toPTSource(gameGroup.$s3);
            }

            // Change game type PT
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    min = gameType.n3;
                    max = gameType.s4;

                    // Set warning for s3 if break pt rule
                    gameType.$s3.warning = newValue < min || newValue > max.ptRound();

                    selected = Math.min(max, newValue).ptRound();

                    if (selected < min || selected > max) {
                        gameType.s3.breakpt = true;
                        gameType.$s3.breakpt = true;
                    }
                    else {
                        gameType.s3 = selected;
                        gameType.$s3.min = gameType.$s3.max = gameType.$s3.selected = selected;
                        gameType.$s3 = toPTSource(gameType.$s3);
                    }
                }
            }
        });

        $scope.$watch("setting.is3", function (newValue, oldValue) {
            if (newValue == oldValue) return; // Avoid $apply checking

            var min = gameGroup.n3;
            var max = gameGroup.s4 - gameGroup.m3 - gameGroup.m2;
            var selected = gameGroup.s3;

            if (!newValue && selected >= min && selected <= max) {
                $scope.gameGroup.$s3.min = 0;
                $scope.gameGroup.$s3.max = gameGroup.s4 - gameGroup.m3 - gameGroup.m2;
                toPTSource($scope.gameGroup.$s3);
            }

            // Update new is3 for game type            
            if ($scope.customer.isPtByGameType) {
                var c = $scope.gameGroup.gameTypes.length;
                for (var i = 0; i < c; i++) {
                    var gameType = $scope.gameGroup.gameTypes[i];

                    gameType.is3 = newValue;
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
    }
}

function BettingLimitController($scope) {
    $scope.bettingLimits = $scope.$parent.gameType.bettingLimits;
}