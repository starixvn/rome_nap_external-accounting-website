var module = angular.module("leftPanelApp", [])
.directive('ngBlur', function () {
    return function (scope, elem, attrs) {
        elem.bind('blur', function () {
            scope.$apply(attrs.ngBlur);
        });
    };
});

angular.element(document.getElementById('ng-app')).ready(function () {
    parent.Fanex.ExtAcc.UnBlockUI();
    $('#ng-app').show();
    $('#txtLeftPanelUserName').focus();
});

Array.prototype.contains = function (item) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === item) {
            return true;
        }
    }

    return false;
};

function GetDownline(isDirectDownline, custId, pageIndex) {
    parent.Fanex.ExtAcc.BlockUI();

    $.ajax({
        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetDownlineList?isDirectDownline=' + isDirectDownline + '&custId=' + custId + '&pageIndex=' + pageIndex),
        type: 'post'
    })
    .success(function (result) {
        model.downlineList = result;
        model.downlineList.downlines.currentPage = pageIndex;

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
                var page = '<a id="link-' + pages[i].value + '" class="page" href="javascript:;" onclick="GetDownline(' + isDirectDownline + ',\'' + model.personalInfo.custId.value + '\', ' + pages[i].value + ')">' + pages[i].value + '</a>';
                $('#pages').append(page);
            }

            $('#downlines').scrollTop(0);
        }

        $('#downlines').css('height', '240px');

        if (pages && pages.length > 1) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);

                for (var i = 1; i <= pages.length; i++) {
                    $('#link-' + i).removeClass('pageSelected');
                }

                $('#link-' + model.downlineList.downlines.currentPage).addClass('pageSelected');
            }, 100);
        }

        parent.Fanex.ExtAcc.UnBlockUI();
    })
    .error(function (result) {
        parent.Fanex.ExtAcc.UnBlockUI();
    });
};

window.top.reloadLeftPanel = function() {
    parent.Fanex.ExtAcc.Form.Close();
    Fanex.CustomerSetting.Core.BlockUI();
    var iframe = parent.document.getElementById("leftFrame");
    iframe.src = iframe.src;
}