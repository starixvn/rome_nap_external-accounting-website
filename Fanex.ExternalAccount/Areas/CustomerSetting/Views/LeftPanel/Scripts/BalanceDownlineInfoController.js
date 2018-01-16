function BalanceDownlineInfoController($scope, $http) {
    $scope.className = 'fiftyPercent';

    if ($scope.$parent.model) {
        $scope.personalInfo = $scope.$parent.model.personalInfo;
        $scope.model = $scope.$parent.model;

        $scope.personalInfoHeight = $('#customerInfo').height();

        if ($scope.personalInfo.roleId.value == 2) {
            $scope.className = 'thirdtyThreePercent';
        }
        else if ($scope.personalInfo.roleId.value == 1) {
            $scope.className = 'onehundredPercent';
        }
    }
    else {
        $scope.model = null;
    }

    $scope.GetDownline = function (custId) {
        if (model.downlineLoaded) return;

        parent.Fanex.ExtAcc.BlockUI();
        var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
        var token = "";
        if (antiXSRFToken.length > 0) {
            token = antiXSRFToken[0].value;
        }
        $http({ url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetDownlineList?custId=' + custId + '&pageIndex=1'),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest", "__RequestVerificationToken": token }
        })
        .success(function (result) {
            model.downlineLoaded = true;
            model.downlineList = result;
            model.downlineList.downlines.currentPage = 1;

            var users = result.downlines;
            var length = users.length;

            if (length > 0) {
                $('#downlines').empty();
                $('#downlines').show();
                $('#nodownline').hide();
            }
            else {
                $('#downlines').hide();
                $('#nodownline').show();
            }

            for (var i = 0; i < length; i++) {
                var item = '<div class="downlineTitle percent50">' +
                                '<a class="detail2" href="javascript:;" onclick="parent.Fanex.ExtAcc.ViewUser(\'' + users[i].text + '\', 1)">' + users[i].text + '</a>' +
                            '</div>';
                $('#downlines').append(item);
            }

            var pages = model.downlineList.pages;
            if (model.downlineList.pages.length > 1) {
                $('#pages').empty();
                $('#page').show();
                $('#pages').show();

                for (var i = 0; i < pages.length; i++) {
                    var page = '<a id="link-' + pages[i].value + '" class="page" href="javascript:;" onclick="GetDownline(\'' + model.personalInfo.custId.value + '\', ' + pages[i].value + ')">' + pages[i].value + '</a>';
                    $('#pages').append(page);
                }

                $('#downlines').scrollTop(0);
            }

            $('#downlines').css('height', (pages.length > 1 ? '178px' : '208px'));

            if (pages && pages.length > 1) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    for (var i = 1; i <= pages.length; i++) {
                        $('#link-' + i).removeClass('pageSelected');
                    }

                    $('#link-1').addClass('pageSelected');
                }, 100);
            }

            parent.Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
        });
    };
};