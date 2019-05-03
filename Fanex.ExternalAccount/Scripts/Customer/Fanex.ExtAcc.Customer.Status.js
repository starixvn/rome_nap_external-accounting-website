Fanex.ExtAcc.Customer.Status = new function () {
    var self = this;
    self.cachedDownlinesDetailInfoData;
    self.DOMElements;
    self.CustIdList;
    self.RoleId;

    self.clearMessage = function () {
        self.DOMElements.successMsg.empty().hide();
        self.DOMElements.errMsg.empty().hide();
    }

    self.btnUnsuspend_OnClick = function () {
        parent.Fanex.ExtAcc.BlockUI();
        self.DOMElements.btnUnsuspend.attr("disabled", true);
        self.clearMessage();
        var params = {
            custId: self.DOMElements.custId.val(),
            suspended: false
        };

        $.ajax({
            type: 'POST',
            url: resolveClientUrl('Customer/UpdateSuspendSingle'),
            dataType: "json",
            data: params,
            success: function (result) {
                if (result.ErrorCode == 0) {
                    self.DOMElements.successMsg.html(top.window.LanguageResources.UpdateSuspendSuccess).show();
                    setTimeout(function () {
                        parent.$('#windowPopup').dialog('close');
                        parent.$('#windowPopup').dialog('destroy');
                        top.window.frames['leftFrame'].src = top.window.frames['leftFrame'].src;
                    }, 3000)
                }
                else {
                    self.DOMElements.errMsg.html(result.ErrorMessage).show();
                    self.DOMElements.btnUnsuspend.removeAttr("disabled");
                }
            },
            complete: function () {
                self.DOMElements.btnUnsuspend.removeAttr("disabled");
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    };

    self.btnBack_OnClick = function () {
        self.DOMElements.tabDetail.fadeOut(function () { self.DOMElements.tabMain.fadeIn(); });
    };

    self.btnReset_OnClick = function () {
        parent.Fanex.ExtAcc.BlockUI();
        var params = {
            custIdList: self.CustIdList,
            roleId: self.RoleId
        };

        $.ajax({
            type: 'POST',
            url: resolveClientUrl('Customer/MultipleSuspend'),
            dataType: 'html',
            data: params,
            success: function (result) {
                $('#windowPopup > div').eq(0).html(result);
                self.Init();
            },
            complete: function () {
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    };

    self.btnSuspendMultiple_OnClick = function () {
        parent.Fanex.ExtAcc.BlockUI();
        self.DOMElements.btnReset.attr("disabled", true);
        self.DOMElements.btnSuspendMultiple.attr("disabled", true);
        self.clearMessage();
        var params = {
            request: $('.button-suspend-status').map(function () {
                var $this = $(this);
                return { CustId: $this.attr('data-custid'), UserName: $this.attr('data-username'), Suspended: $this.hasClass('suspend-switch') };
            }).get()
        };

        console.log(params);
        $.ajax({
            type: 'POST',
            url: resolveClientUrl('Customer/UpdateSuspendMultiple'),
            dataType: "json",
            data: params,
            success: function (result) {
                for (var i = 0 ; i < result.length; i++) {
                    if (result[i].IsSuccess) {
                        self.DOMElements.successMsg.append('<p style="margin: 0px">' + result[i].ErrorMessage + '</p>').show();
                    }
                    else {
                        self.DOMElements.errMsg.append('<p style="margin: 0px">' + result[i].ErrorMessage + '</p>').show();
                    }
                }
            },
            complete: function () {
                self.DOMElements.btnReset.removeAttr("disabled");
                self.DOMElements.btnSuspendMultiple.removeAttr("disabled");
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    };

    self.btnSuspendStatus_OnClick = function () {
        var $this = $(this);
        var custId = $this.attr('data-custid');
        $this.toggleClass('suspend-switch unsuspend-switch');

        if ($this.hasClass('suspend-switch')) {
            $this.html('  Suspended');
            $('#row-remain-unchanged-all-' + custId).show();
            $('#row-remain-unchanged-SMA-' + custId).hide();
            $('#row-affected-all-' + custId).show();
            $('#row-affected-SMA-' + custId).hide();
        }
        else {
            $this.html('Unsuspended');
            $('#row-remain-unchanged-all-' + custId).hide();
            $('#row-remain-unchanged-SMA-' + custId).show();
            $('#row-affected-all-' + custId).hide();
            $('#row-affected-SMA-' + custId).show();
        }
    };

    self.btnSuspendAll_OnClick = function () {
        $('.button-suspend-status:not(.suspend-switch)').click();
    };

    self.btnUnsuspendAll_OnClick = function () {
        $('.button-suspend-status:not(.unsuspend-switch)').click();
    };

    self.informationBox_OnClick = function () {
        Fanex.ExtAcc.Form.Alert('Information', top.window.LanguageResources.SuspendInfoMessage, 750, 350, 'suspend-information-box');
    }

    self.Init = function (custIdList, roleId) {
        self.cachedDownlinesDetailInfoData = {
            custId: 0,
            isAffected: false,
            downlineRoleId: 0,
        }

        self.DOMElements = {
            btnUnsuspend: $('#btnUnsuspend'),
            custId: $('#custId'),
            successMsg: $('#successMsg'),
            errMsg: $('#errMsg'),
            btnBack: $('#btnBack'),
            tabDetail: $('#tab-detail'),
            tabMain: $('#tab-main'),
            gridContainer: $('#gridContainer'),
            gridId: $('#gridId'),
            btnReset: $('#btnReset'),
            btnSuspendMultiple: $('#btnSuspendMultiple'),
            btnSuspendStatus: $('.button-suspend-status'),
            btnSuspendAll: $('#button-suspend-all'),
            btnUnsuspendAll: $('#button-unsuspend-all'),
            informationBox: $('#information-box')
        }

        self.DOMElements.btnUnsuspend.click(function () {
            return self.btnUnsuspend_OnClick();
        });

        self.DOMElements.btnBack.click(function () {
            return self.btnBack_OnClick();
        });

        self.DOMElements.btnReset.click(function () {
            return self.btnReset_OnClick();
        });

        self.DOMElements.btnSuspendMultiple.click(function () {
            return self.btnSuspendMultiple_OnClick();
        });

        self.DOMElements.btnSuspendStatus.click(self.btnSuspendStatus_OnClick);

        self.DOMElements.btnSuspendAll.click(function () {
            return self.btnSuspendAll_OnClick();
        });

        self.DOMElements.btnUnsuspendAll.click(function () {
            return self.btnUnsuspendAll_OnClick();
        });

        self.DOMElements.informationBox.click(function () {
            return self.informationBox_OnClick();
        });
    }

    self.ViewSuspendSingle = function (custId, userName) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/UnsuspendSingle') + '?custId=' + custId + '&userName=' + userName, 'Update Unsuspend Status for ' + userName, 'auto', 'auto', false, self.Init);
    }

    self.ViewSuspendMultiple = function (custIdList, roleId) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/MultipleSuspend?custIdList=' + custIdList + '&roleid=' + roleId), 'Edit Multiple Suspend Status', 'auto', 'auto', false, self.Init);
        self.CustIdList = custIdList.join();
        self.RoleId = roleId;
    }

    self.RerenderPagingDownlineDetailInfo = function (currentPageIndex, itemsPerPage) {
        self.GetDownlinesDetailInfo(self.cachedDownlinesDetailInfoData.custId, self.cachedDownlinesDetailInfoData.isAffected, self.cachedDownlinesDetailInfoData.downlineRoleId, itemsPerPage, currentPageIndex);
    }

    self.GetDownlinesDetailInfo = function (custId, isAffected, downlineRoleId, pageSize, pageIndex) {
        parent.Fanex.ExtAcc.BlockUI();
        var params = {
            custId: custId,
            isAffected: isAffected,
            downlineRoleId: downlineRoleId,
            pageSize: pageSize,
            pageIndex: pageIndex
        };

        self.cachedDownlinesDetailInfoData.custId = custId;
        self.cachedDownlinesDetailInfoData.isAffected = isAffected;
        self.cachedDownlinesDetailInfoData.downlineRoleId = downlineRoleId;
        $.ajax({
            type: 'POST',
            url: resolveClientUrl('Customer/GetSuspendDownlinesDetail'),
            dataType: 'json',
            data: params,
            success: function (result) {
                var bodyRows = [];
                var rowCount = result.AffectedDownlines.length;
                for (var i = 0 ; i < rowCount; i++) {
                    var row = { Columns: [] };
                    row.Columns.push({ Name: 'Level', Value: result.AffectedDownlines[i].LevelName });
                    row.Columns.push({ Name: 'Username', Value: result.AffectedDownlines[i].UserName });
                    bodyRows.push(row);
                }

                var gridOptions =
                {
                    columns: [
                        { HeaderText: "Level", Width: 150, Name: "Level", HeaderAlign: "center" },
                        { HeaderText: "Username", Width: 300, Name: "Username", HeaderAlign: "center" }
                    ],
                    bodyRows: bodyRows,
                    showDistinctRow: false,
                    showPager: true,
                    customPager: true,
                    pagerOption: {
                        itemsPerPage: pageSize,
                        currentPage: pageIndex,
                        itemsPerPageArray: [50, 100, 150, 200],
                        pagerSelectCallBack: self.RerenderPagingDownlineDetailInfo,
                        totalItem: result.TotalRows
                    }
                };

                self.DOMElements.tabMain.fadeOut(function () {
                    self.DOMElements.tabDetail.fadeIn(function () {
                        self.DOMElements.tabDetail.show();
                        self.DOMElements.gridContainer.show();
                        self.DOMElements.gridId.fanexGrid(gridOptions);
                    });
                });
            },
            complete: function () {
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
}