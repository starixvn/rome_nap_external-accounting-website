/// <reference path="../_references.js" />
if (typeof Fanex === 'undefined' || typeof Fanex.ExtAcc === 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.SubGroup = {
    URL: '',
    $this: null,
    params: null,
    customerList: [],

    Init: function () {
        Fanex.ExtAcc.BlockUI();
        $('#mainGrid').css('visibility', 'hidden');
        $this = this;
        $this.URL = resolveClientUrl("SubGroup/SearchSubGroupCustomer");
        $('#btnSearch').click(function () {
            $this.GetCustomerList(1, 50);
        });
        $this.InitSubGroup();
    },
    InitSubGroup: function () {
        var groupId = $('#selectedGroupName').html() === '' ? '17' : $('#ddlGroupName').val();
        var params = new Object();
        params.groupId = groupId;
        $.ajax({
            url: resolveClientUrl("SubGroup/SubGroupPost"),
            data: params,
            dataType: 'json',
            type: 'post'
        })
            .success(function (data) {
                if (data.errorCode === "401" || data.errorCode === "204") {
                    $this.ShowError(data.errorCode);
                    return;
                }
                $this.HideNoPermission();
                $.each(data, function (_, subGroup) {
                    $('#selSubGroup').append($('<option>', {
                        value: subGroup.groupName,
                        text: subGroup.groupName
                    }));
                });
                Fanex.ExtAcc.UnBlockUI();
            });
    },
    GetCustomerList: function (pageIndex, pageSize) {
        Fanex.ExtAcc.BlockUI();
        var groupName = $('#selectedGroupName').html();
        var subGroupName = $('#selSubGroup').val();
        var username = $('#txtUserName').val();

        $this.params = new Object();
        $this.params.groupName = groupName;
        $this.params.subGroupName = subGroupName;
        $this.params.username = username;
        $this.params.pageSize = pageSize;
        $this.params.pageIndex = pageIndex;

        var header = [
            { HeaderText: 'Username', Name: 'Username', Width: 225 },
            {
                HeaderText: 'Status', Name: 'Status', Width: 125, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return !cell.Value.isEnabled ? 'Disabled' : cell.Value.isClosed ? 'Closed' : cell.Value.isSuspended ? 'Suspended' : 'Open';
                }
            }
        ];

        $.ajax({
            url: $this.URL,
            data: $this.params,
            dataType: 'json',
            type: 'post'
        })
            .success(function (data) {
                
                var subGroups = data.subGroups;
                for (var i = 0; i < subGroups.length; i++) {
                    header.push({
                        HeaderText: subGroups[i],
                        Name: subGroups[i],
                        CellAlign: 'center',
                        Width: 225,
                        CustomHeaderRender: function (cell) {
                            return '<div>' + cell.HeaderText + '</div><div id="editMult' + cell.HeaderText + '" class="editMult" data-subGroupName="' + cell.HeaderText + '"></div>';
                        },
                        CustomCellRender: function (cell) {
                            var checked = cell.Value.SubGroupStatus.isAssigned ? "checked" : "";
                            var subGroupName = cell.Value.SubGroupStatus.groupName;
                            var id = subGroupName + cell.Value.CustId;
                            var onclick = "javascript:Fanex.ExtAcc.SubGroup.UpdateCustomerSubGroup(this, event)";

                            return '<input type="checkbox" id="' + id + '" data-custId="' + cell.Value.CustId + '" data-subGroupName="' + subGroupName + '" data-isAssigned="' + !cell.Value.SubGroupStatus.isAssigned + '" onclick="' + onclick + '" ' + checked + '></div>';
                        }
                    });
                }

                var bodyRows = [];
                $this.customerList = [];
                for (var i = 0; i < data.subGroupCustomers.length; i++) {
                    var customer = data.subGroupCustomers[i];
                    $this.customerList.push(customer.custId);
                    var columns = [
                        { Name: 'Username', Value: customer.username },
                        { Name: 'Status', Value: customer }
                    ];

                    for (var j = 0; j < customer.subGroupStatusList.length; j++) {
                        columns.push({ Name: customer.subGroupStatusList[j].groupName, Value: { SubGroupStatus: customer.subGroupStatusList[j], CustId: customer.custId } });
                    }

                    bodyRows.push({
                        Columns: columns
                    });
                }

                var options = {
                    columns: header,
                    bodyRows: bodyRows,
                    autoCalculateGridExpandHeight: true,
                    marginRight: 10,
                    showDistinctRow: false,
                    allowTruncate: true,
                    showPager: true,
                    customPager: true,
                    pagerOption: {
                        itemsPerPageArray: [50, 100, 500],
                        currentPage: pageIndex - 1,
                        totalItem: data.totalItems,
                        itemsPerPage: pageSize,
                        pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                            $this.GetCustomerList(currentPageIndex + 1, itemsPerPage);
                        }
                    }
                };

                $this.RenderGrid(options);
                $this.InitHeaderEvent();

                if (data.subGroupCustomers.length === 0) {
                    $("div[id^='editMult']").hide();
                }
            });

    },
    RenderGrid: function (options) {
        try {
            // Init Grid jqgrid
            $('#jqgrid-group').fanexGrid(options);
            Fanex.ExtAcc.UnBlockUI();

            $('#mainGrid').css('visibility', 'visible');
        }
        catch (e) {
            if (typeof console !== 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },
    InitHeaderEvent: function () {
        $("div[id^='editMult']").attr('style', 'cursor: pointer !important');
        var menuItems = new Object();
        menuItems.assignAll = {
            name: "Assign all supers (this page)",
            className: "",
            callback: function () {
                var updateRequest = {
                    CustIds: $this.customerList.join(),
                    SubGroupName: this.attr("data-subGroupName"),
                    IsAssigned: true
                };

                $this.ShowAssignAllCustomerPopup(updateRequest, "Assign all supers (all pages)", "Are you sure you want to Assign all supers (this page)?<br>Please be reminded that newly created super must be manually assigned to sub accounts.");
            }
        };
        menuItems.unassignAll = {
            name: "Unassign all supers (this page)",
            className: "",
            callback: function () {
                var updateRequest = {
                    CustIds: $this.customerList.join(),
                    SubGroupName: this.attr("data-subGroupName"),
                    IsAssigned: false
                };

                $this.ShowAssignAllCustomerPopup(updateRequest, "Unassign all supers (all pages)", "Are you sure you want to Unassign all supers (this page)?<br>Please be reminded that newly created super must be manually assigned to sub accounts.");
            }
        };
        $.contextMenu({
            selector: "div[id^='editMult']",
            items: menuItems,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
        $("div[id^='editMult']").contextMenu(true);
    },
    ShowAssignAllCustomerPopup: function (updateRequest, title, msg) {
        $('#errMsg').hide();
        Fanex.ExtAcc.Form.ConfirmBox(title, msg, 450, 170, false, function () {
            Fanex.ExtAcc.Form.Close(false);
            Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: resolveClientUrl('SubGroup/SubGroupUpdate'),
                dataType: 'json',
                type: 'post',
                data: updateRequest
            })
                .success(function (result) {
                    if (result.isSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            $this.GetCustomerList($this.params.pageIndex, $this.params.pageSize);
                            $('#errMsg').attr('class', 'successMsg');
                            $('#errMsg').html("The action has been successfully done.");
                            $('#errMsg').show();
                            var showMsgTimout = setTimeout(function () {
                                clearTimeout(showMsgTimout);
                                $('#errMsg').hide();
                            }, 2000);
                        }, 2000);
                    } else {
                        Fanex.ExtAcc.UnBlockUI();
                        Fanex.ExtAcc.Form.Alert('Error', result.errorMessage.formatErrorMessage(), 300, 150, 'alert');
                    }
                });
        });
    },
    UpdateCustomerSubGroup: function (div, event) {
        event.preventDefault();
        Fanex.ExtAcc.BlockUI();
        var url = resolveClientUrl("SubGroup/SubGroupUpdate");
        var checkBox = $(div);

        var updateRequest = {
            CustIds: checkBox.attr("data-custId"),
            SubGroupName: checkBox.attr("data-subGroupName"),
            IsAssigned: checkBox.attr("data-isAssigned")
        };

        $.ajax({
            url: url,
            data: updateRequest,
            dataType: 'json',
            type: 'post'
        }).success(function (data) {
            if (!data.isSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                checkBox.prop("checked", updateRequest.IsAssigned === "true");
                checkBox.attr("data-isAssigned", updateRequest.IsAssigned !== "true");
            }
            Fanex.ExtAcc.UnBlockUI();
        });
    },
    ShowError: function (errorCode) {
        if (errorCode === "401") {
            $("#divNoPermission").find('h3').html("You don't have permission to access.");
        } else if (errorCode === "204") {
            $("#divNoPermission").find('h3').html("There is no Sub Group to assign super.");
        }

        $("#jqWidget").hide();
        $("#divNoPermission").show();
        Fanex.ExtAcc.UnBlockUI();
    },
    HideNoPermission: function () {
        $("#jqWidget").show();
        $("#divNoPermission").hide();
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.SubGroup.Init();
});