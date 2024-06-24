/// <reference path="../_references.js" />
if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Permission = {
    FunctionFrom: null,
    FunctionTo: null,
    PolicyMessage: null,
    PolicyTabs: null,
    GroupPolicyGridOptions: null,
    GroupPolicyGridContainer: null,
    GroupPolicyGrid: null,
    UserPolicyGridOptions: null,
    UserPolicyGridContainer: null,
    UserPolicyGrid: null,
    GridExpandHeight: null,

    Init: function () {
        var self = this;

        this.CacheDOMElements();
        this.GridExpandHeight = parseInt($("#gridExpandHeightHiddenField").val());
        this.GroupPolicyGridOptions = this.InitGroupPolicyGridOptions();
        this.UserPolicyGridOptions = this.InitUserPolicyGridOptions();

        this.RegisterEvents();
    },

    CacheDOMElements: function () {
        this.FunctionFrom = $("#functionFrom");
        this.FunctionTo = $("#functionTo");
        this.PolicyMessage = $("#err-message");
        this.PolicyTabs = $("#tabs").tabs();
        this.GroupPolicyGridContainer = $("#groupPolicyGrid");
        this.GroupPolicyGrid = $("#jqgrid-groupPolicy");
        this.UserPolicyGridContainer = $("#userPolicyGrid");
        this.UserPolicyGrid = $("#jqgrid-userPolicy");
    },

    RegisterEvents: function () {
        var self = this;

        var functionChange = function () {
            self.HideMessage();
            var funcFrom = self.FunctionFrom.val();
            var funcTo = self.FunctionTo.val();

            if (funcFrom !== "0" && funcTo !== "0") {
                if (self.PolicyTabs.tabs("option", "active") === 0) {
                    self.RenderGroupPolicyGrid(funcFrom);
                } else {
                    self.RenderUserPolicyGrid(funcFrom);
                }

                self.PolicyTabs.removeClass("hide");
            }
        };

        this.FunctionFrom.change(functionChange);
        this.FunctionTo.change(functionChange);

        this.PolicyTabs.on("tabsactivate", function (event, ui) {
            self.HideMessage();
            if (self.PolicyTabs.tabs("option", "active") === 0) {
                self.RenderGroupPolicyGrid(self.FunctionFrom.val());
            } else {
                self.RenderUserPolicyGrid(self.FunctionFrom.val());
            }
        });

        $("#updateGroupPolicy").click(function () {
            var funcName = self.FunctionTo.find("option:selected").html();

            Fanex.ExtAcc.Form.ConfirmBox(
                "Group Permission",
                "Are you sure to set group permission for <b>" + funcName + "</b> function?",
                420,
                150,
                false,
                function () {
                    self.OnSubmit(self.GroupPolicyGrid, resolveClientUrl("Permission/UpdateGroupPolicy"));
                });
        });

        $("#updateUserPolicy").click(function () {
            var funcName = self.FunctionTo.find("option:selected").html();

            Fanex.ExtAcc.Form.ConfirmBox(
                "User Permission",
                "Are you sure to set user permission for <b>" + funcName + "</b> function?",
                420,
                150,
                false,
                function () {
                    self.OnSubmit(self.UserPolicyGrid, resolveClientUrl("Permission/UpdateUserPolicy"));
                });
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                if (self.PolicyTabs.tabs("option", "active") === 0) {
                    self.GroupPolicyGrid.fanexGrid("resizeGrid");
                } else {
                    self.UserPolicyGrid.fanexGrid("resizeGrid");
                }
            }
            catch (err) {
            }
        }

        $(window).bind("orientationchange", function (e) {
            top.landscapeHeight = 260;

            var calculatePopupheight = top.shouldCalculatePopupHeight && isMobile.phone;
            if (!calculatePopupheight) {
                return;
            }

            var height;
            var isLandscape = window.matchMedia("(orientation: landscape)").matches;
            if (isMobile.android.phone) {
                height = isLandscape ? top.portraitHeight : top.landscapeHeight;
            }
            else {
                height = isLandscape ? top.landscapeHeight : top.portraitHeight;
            }

            var dialog = top.$(".ui-dialog");
            dialog.height((height + 34) + "px");

            top.$("#windowPopup").css("height", height + "px");
        });
    },

    GetGroupPolicyGridGroupHeaderRows: function (funcFrom, funcTo) {
        return [
            {
                Columns: [
                    { HeaderText: "Group Name", Name: "colGroupName", MergeRowNumber: 2 },
                    { HeaderText: "Group Permission<br/>(From " + funcFrom + ")", Name: "colSourcePermissionRead", MergeColNumber: 2 },
                    { HeaderText: "Group Permission<br/>(For " + funcTo + ")", Name: "colDestinationPermissionRead", MergeColNumber: 2 }
                ]
            }
        ];
    },

    InitGroupPolicyGridOptions: function () {
        return {
            customizeHeader: true,
            columns: [
                { HeaderText: "Group Name", Name: "colGroupName", Width: 200 },
                {
                    HeaderText: "Read",
                    Name: "colSourcePermissionRead",
                    Width: 90,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (policy) {
                        if (policy.Value === 0) {
                            return '<div class="permInactive" style="cursor:default"></div>';
                        }

                        return policy.Value > 0
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : "";
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colSourcePermissionWrite",
                    Width: 90,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (policy) {
                        return policy.Value === 2
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : '<div class="permInactive" style="cursor:default"></div>';
                    }
                },
                {
                    HeaderText: "Read",
                    Name: "colDestinationPermissionRead",
                    Width: 90,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.Policy === 0 ? "permInactive" : "permActive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnReadChanged(this);" id="read',
                            cell.Value.GroupId,
                            '" data-id="',
                            cell.Value.GroupId,
                            '" data-read="1"></div>'
                        ].join("");
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colDestinationPermissionWrite",
                    Width: 90,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.Policy === 2 ? "permActive" : "permInactive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnWriteChanged(this);" id="write',
                            cell.Value.GroupId,
                            '" data-id="',
                            cell.Value.GroupId,
                            '"></div>'
                        ].join("");
                    }
                }
            ],
            gridExpandHeight: this.GridExpandHeight,
            showDistinctRow: false,
            allowTruncate: true
        };
    },

    RenderGroupPolicyGrid: function (funcId) {
        var self = this;

        Fanex.ExtAcc.BlockUI();
        this.GroupPolicyGridContainer.css("visibility", "hidden");

        $.ajax({
            type: "GET",
            url: resolveClientUrl("Permission/GetGroupPolicies?funcId=" + funcId),
            dataType: "json"
        })
        .success(function (data) {
            var group;
            var bodyRows = [];

            for (var i = 0, max = data.length; i < max; i++) {
                group = data[i];
                bodyRows.push({
                    Columns: [
                        { Name: "colGroupName", Value: group.GroupName },
                        { Name: "colSourcePermissionRead", Value: group.Policy },
                        { Name: "colSourcePermissionWrite", Value: group.Policy },
                        { Name: "colDestinationPermissionRead", Value: group },
                        { Name: "colDestinationPermissionWrite", Value: group }
                    ]
                });
            }

            self.GroupPolicyGridOptions.groupHeaderRows = self.GetGroupPolicyGridGroupHeaderRows(
                self.FunctionFrom.find("option:selected").html(),
                self.FunctionTo.find("option:selected").html());
            self.GroupPolicyGridOptions.bodyRows = bodyRows;
            self.GroupPolicyGrid.fanexGrid(self.GroupPolicyGridOptions);

            Fanex.ExtAcc.UnBlockUI();
            self.GroupPolicyGridContainer.css("visibility", "visible");
        });
    },

    GetUserPolicyGridGroupHeaderRows: function (funcFrom, funcTo) {
        return [
            {
                Columns: [
                    { HeaderText: "Username", Name: "colUsername", MergeRowNumber: 2 },
                    { HeaderText: "Group Name", Name: "colGroupName", MergeRowNumber: 2 },
                    { HeaderText: "Active", Name: "colActive", MergeRowNumber: 2 },
                    { HeaderText: "User Permission<br/>(From " + funcFrom + ")", Name: "colSourcePermissionRead", MergeColNumber: 2 },
                    { HeaderText: "User Permission<br/>(For " + funcTo + ")", Name: "colDestinationPermissionRead", MergeColNumber: 2 }
                ]
            }
        ];
    },

    InitUserPolicyGridOptions: function () {
        return {
            customizeHeader: true,
            columns: [
                { HeaderText: "Username", Name: "colUsername", Width: 200 },
                { HeaderText: "Group Name", Name: "colGroupName", Width: 120 },
                {
                    HeaderText: "Active",
                    Name: "colActive",
                    Width: 50,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (enabled) {
                        return '<div class="' + (enabled.Value ? "permActive" : "permInactive") + '" style="cursor:default"></div>';
                    }
                },
                {
                    HeaderText: "Read",
                    Name: "colSourcePermissionRead",
                    Width: 90,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (policy) {
                        if (policy.Value === 0) {
                            return '<div class="permInactive" style="cursor:default"></div>';
                        }

                        return policy.Value > 0
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : "";
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colSourcePermissionWrite",
                    Width: 90,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (policy) {
                        return policy.Value === 2
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : '<div class="permInactive" style="cursor:default"></div>';
                    }
                },
                {
                    HeaderText: "Read",
                    Name: "colDestinationPermissionRead",
                    Width: 90,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.Policy === 0 ? "permInactive" : "permActive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnReadChanged(this);" id="read',
                            cell.Value.CustId,
                            '" data-id="',
                            cell.Value.CustId,
                            '" data-read="1"></div>'
                        ].join("");
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colDestinationPermissionWrite",
                    Width: 90,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.Policy === 2 ? "permActive" : "permInactive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnWriteChanged(this);" id="write',
                            cell.Value.CustId,
                            '" data-id="',
                            cell.Value.CustId,
                            '"></div>'
                        ].join("");
                    }
                }
            ],
            gridExpandHeight: this.GridExpandHeight,
            showDistinctRow: false,
            allowTruncate: true
        };
    },

    RenderUserPolicyGrid: function (funcId) {
        var self = this;

        Fanex.ExtAcc.BlockUI();
        this.UserPolicyGridContainer.css("visibility", "hidden");

        $.ajax({
            type: "GET",
            url: resolveClientUrl("Permission/GetUserPolicies?funcId=" + funcId),
            dataType: "json"
        })
        .success(function (data) {
            var user;
            var bodyRows = [];

            for (var i = 0, max = data.length; i < max; i++) {
                user = data[i];
                bodyRows.push({
                    Columns: [
                        { Name: "colUsername", Value: user.Username },
                        { Name: "colGroupName", Value: user.GroupName },
                        { Name: "colActive", Value: user.Enabled },
                        { Name: "colSourcePermissionRead", Value: user.Policy },
                        { Name: "colSourcePermissionWrite", Value: user.Policy },
                        { Name: "colDestinationPermissionRead", Value: user },
                        { Name: "colDestinationPermissionWrite", Value: user }
                    ]
                });
            }

            self.UserPolicyGridOptions.groupHeaderRows = self.GetUserPolicyGridGroupHeaderRows(
                self.FunctionFrom.find("option:selected").html(),
                self.FunctionTo.find("option:selected").html());
            self.UserPolicyGridOptions.bodyRows = bodyRows;
            self.UserPolicyGrid.fanexGrid(self.UserPolicyGridOptions);

            Fanex.ExtAcc.UnBlockUI();
            self.UserPolicyGridContainer.css("visibility", "visible");
        });
    },

    OnReadChanged: function (target) {
        var $target = this.OnPermissionChanged(target);
        if ($target.hasClass("permInactive")) {
            $("#write" + $target.data("id"))
                .removeClass()
                .addClass("permInactive");
        }
    },

    OnWriteChanged: function (target) {
        var $target = this.OnPermissionChanged(target);
        if ($target.hasClass("permActive")) {
            $("#read" + $target.data("id"))
                .removeClass()
                .addClass("permActive");
        }
    },

    OnPermissionChanged: function (target) {
        var $target = $(target);
        var checked = $target.hasClass("permActive");
        return $target
            .removeClass()
            .addClass(checked ? "permInactive" : "permActive");
    },

    OnSubmit: function (table, url) {
        Fanex.ExtAcc.Form.Close();
        Fanex.ExtAcc.BlockUI();

        var jqxhr = $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify({
                FuncId: this.FunctionTo.val(),
                Policies: this.GetFormData(table)
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        });

        var self = this;
        jqxhr
            .done(function (data) {
                if (data.IsSuccess) {
                    self.ShowMessage(data.ErrorMessage);
                    self.HideMessage(3000);
                } else {
                    self.ShowMessage(data.ErrorMessage, 1);
                }
            })
            .always(Fanex.ExtAcc.UnBlockUI);
    },

    GetFormData: function (table) {
        var formData = [];
        var resultArray = table.find('[data-read="1"]');

        for (var i = 0, max = resultArray.length; i < max; i++) {
            formData.push(this.GetFormDataRow(table, $(resultArray[i]).data("id")));
        }

        return formData.join(";");
    },

    GetFormDataRow: function (table, id) {
        var policy = 0;
        var readElement = table.find("#read" + id);

        if (readElement.hasClass("permActive")) {
            policy = 1;

            var writeElement = $("#write" + id);

            if (writeElement.hasClass("permActive")) {
                policy = 2;
            }
        }

        return id + "," + policy;
    },

    ShowMessage: function (message, code) {
        this.PolicyMessage
            .html(message)
            .removeClass()
            .addClass(code ? "errMsg" : "successMsg");
    },

    HideMessage: function (timeout) {
        if (timeout) {
            var self = this;

            setTimeout(function () {
                self.PolicyMessage.removeClass().addClass("hide").html("");
            }, timeout);
        } else {
            this.PolicyMessage.removeClass().addClass("hide").html("");
        }
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Permission.Init();
});