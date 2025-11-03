/// <reference path="../_references.js" />
if (typeof Fanex === "undefined" || typeof Fanex.ExtAcc === "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

(function ($) {
    var cacheDOMElements = {},
        $popupSetToReviewed = null,
        filterParams = {
            3: null,
            2: null,
            1: null
        },
        hasHostPermission = false;

    function selector(query) {
        var element = cacheDOMElements[query];
        if (!element) {
            element = $(query);
            cacheDOMElements[query] = element;
        }

        return element;
    }

    function initSMATab($form, roleId) {
        initFromToDatePicker(selector("#dtp-fromdate-" + roleId), selector("#dtp-todate-" + roleId));
        initCombobox(roleId);
        bindEvents($form, roleId);

        roleId === 3 && setTimeout(function () { $form.trigger("submit"); }, 100);
    }

    function initFromToDatePicker($dtpFrom, $dtpTo) {
        initDatePicker($dtpFrom, {
            maxDate: new Date($dtpTo.val()),
            onSelect: function (dateText) {
                $dtpTo.datepicker("option", "minDate", new Date(dateText));
            }
        });

        initDatePicker($dtpTo, {
            minDate: new Date($dtpFrom.val()),
            onSelect: function (dateText) {
                $dtpFrom.datepicker("option", "maxDate", new Date(dateText));
            }
        });
    }

    function initDatePicker($input, options) {
        $input.datepicker($.extend({
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: ""
        }, options));
    }

    function initCombobox(roleId) {
        var $ddlReviewStatus = selector("#ddl-reviewStatus-" + roleId).combobox();
        $ddlReviewStatus.next("input.ui-autocomplete-input")
            .val($ddlReviewStatus.find("option:selected").text())
            .attr("readonly", "true")
            .bind("click", function () {
                $(this).autocomplete("search", "");
            });
    }

    function bindEvents($form, roleId) {
        $form.submit(function (e) {
            e.preventDefault();

            var formData = getFilterParams(roleId);
            filterParams[roleId] = formData;
            getProblemAccounts(formData);
        });

        selector("#icon-excel-" + roleId).click(function () {
            var formData = filterParams[roleId];
            var url = resolveClientUrl("ProblemAccount/ExportExcel?") + $.param(formData);
            top.exportExcel(url, document);
        });

        hasHostPermission && selector("#freegrid-container-" + roleId)
            .find(".status-select")
            .click(function () {
                var $this = $(this);
                var checkAll = $this.text() === "All";
                selector("#freegrid-" + roleId)
                    .find(".set-to-reviewed")
                    .removeClass()
                    .addClass(checkAll ? "menuActive set-to-reviewed" : "menuInactive set-to-reviewed");
            });
    }

    function getFilterParams(roleId, pageIndex, pageSize) {
        if (!pageSize) {
            pageSize = filterParams[roleId] ? filterParams[roleId].pageSize : 50;
        }

        return {
            groupName: Fanex.ExtAcc.Customer.GroupName().groupName,
            searchRoleId: roleId,
            uplineUsername: selector("#txt-uplineUsername-" + roleId).val(),
            fromDate: selector("#dtp-fromdate-" + roleId).val(),
            toDate: selector("#dtp-todate-" + roleId).val(),
            reviewStatus: selector("#ddl-reviewStatus-" + roleId).val(),
            searchUsername: selector("#txt-searchUsername-" + roleId).val(),
            pageIndex: pageIndex || 0,
            pageSize: pageSize
        };
    }

    function getProblemAccounts(formData) {
        Fanex.ExtAcc.BlockUI();
        var roleId = formData.searchRoleId;
        var $form = selector("#form-problem-account-" + roleId);
        var url = $form.attr("action") + "?" + $.param(formData);
        $.getJSON(url, function (data) {
            if (data.TotalItems) {
                renderProblemAccounts(formData, data);
            } else {
                renderNoData(roleId);
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    }

    function renderProblemAccounts(formData, data) {
        var roleId = formData.searchRoleId;
        var isNotMember = roleId !== 1;

        var bodyRows = [];
        var startNumber = formData.pageIndex * formData.pageSize;
        for (var i = 0, dataLength = data.ProblemAccounts.length; i < dataLength; i++) {
            var datum = data.ProblemAccounts[i];
            var columns = [{ Name: "colNo", Value: startNumber + i + 1 }];
            hasHostPermission && columns.push({ Name: "colSetReviewed", Value: datum });

            columns.push({ Name: "colUsername", Value: datum });
            columns.push({ Name: "colFirstName", Value: datum.FirstName });
            columns.push({ Name: "colLastName", Value: datum.LastName });
            columns.push({ Name: "colStatus", Value: datum.StatusName });
            columns.push({ Name: "colDateNotified", Value: datum.CreatedDateStr });

            isNotMember && columns.push({ Name: "colProblemAccountsNo", Value: datum.NumberOfDownline });

            columns.push({ Name: "colReviewStatus", Value: datum.ReviewStatus });

            bodyRows.push({ Columns: columns, CssClass: getAccountStatusCssClass(datum) });
        }

        var pagerOption = {
            currentPage: formData.pageIndex,
            itemsPerPage: formData.pageSize,
            totalItem: data.TotalItems
        };

        var $grid = selector("#freegrid-" + roleId);
        $grid.fanexGrid(initGridOptions(roleId, bodyRows, pagerOption));
        bindEventsAfterRenderProblemAccountsSuccessfully(roleId, $grid);

        selector("#icon-excel-" + roleId).removeClass("hide");
        selector("#freegrid-container-" + roleId).removeClass("hide");
        selector("#div-noData-" + roleId).addClass("hide");
    }

    function renderNoData(roleId) {
        selector("#icon-excel-" + roleId).addClass("hide");
        selector("#freegrid-container-" + roleId).removeAttr("style").addClass("hide");
        selector("#freegrid-" + roleId).removeAttr("style").html("");
        selector("#div-noData-" + roleId).removeClass("hide");
    }

    function bindEventsAfterRenderProblemAccountsSuccessfully(roleId, $grid) {
        $grid
            .find(".drow-username")
            .off("click")
            .on("click", function () {
                var $this = $(this);
                setFilterUIControls(roleId, $this.text());

                if (roleId === 3) {
                    selector("#problem-account-tabs").tabs("option", "active", 1);
                } else {
                    selector("#problem-account-tabs").tabs("option", "active", 2);
                }
            });

        if (hasHostPermission) {
            $grid
                .find("#icon-review-user-" + roleId)
                .off("click")
                .on("click", function () {
                    var checkboxes = $grid.find(".menuActive");
                    checkboxes.length && openSetToReviewedPopup(roleId, checkboxes);
                });

            $grid
                .find(".menuInactive")
                .off("click")
                .on("click", function () {
                    var $this = $(this);
                    var checked = $this.hasClass("menuActive");
                    $this.removeClass().addClass(checked ? "menuInactive set-to-reviewed" : "menuActive set-to-reviewed");
                });
        }
    }

    function getDirectMemberFormat(datum) {
        if (datum.AgentId < 0) {
            return datum.Username + "<span class='icon-directmember'></span>";
        }
        return datum.Username;
    }

    function getAccountStatusCssClass(datum) {
        switch (datum.Status) {
            case 2:
                return "rowSuspended";
            case 3:
                return Fanex.ExtAcc.Customer.GetClosedClassName(datum);
            case 4:
                return "rowDisabled";
            default:
                return datum.LockedClassName || "";
        }
    }

    function initGridOptions(roleId, bodyRows, pagerOption) {
        var columns = [{ HeaderText: "#", Name: "colNo", Width: 40, CellAlign: "center" }];
        hasHostPermission && columns.push({
            HeaderText: '<span class="icon-review-user" id="icon-review-user-' + roleId + '" title="Set to reviewed"></span>',
            Name: "colSetReviewed",
            Width: 60,
            CellAlign: "center",
            CustomCellRender: function (cell) {
                var datum = cell.Value;
                if (datum.IsReviewed) {
                    return "";
                }

                return [
                    '<div class="menuInactive set-to-reviewed" data-custid="',
                    datum.CustId,
                    '" data-username="',
                    datum.Username,
                    '"></div>'
                ].join("");
            }
        });
        columns.push({
            HeaderText: "Username",
            Name: "colUsername",
            Width: 200,
            CustomCellRender: function (cell) {
                return roleId === 1
                    ? getDirectMemberFormat(cell.Value)
                    : '<a href="javascript:;" class="drow-username">' + cell.Value.Username + '</a>';
            }
        });
        columns.push({ HeaderText: "First Name", Name: "colFirstName", Width: 160 });
        columns.push({ HeaderText: "Last Name", Name: "colLastName", Width: 160 });
        columns.push({ HeaderText: "Status", Name: "colStatus", Width: 100, CellAlign: "center" });
        columns.push({ HeaderText: "Date Notified", Name: "colDateNotified", Width: 140, CellAlign: "center" });

        if (roleId === 2 || roleId === 3) {
            columns.push({
                HeaderText: roleId === 2 ? "Problem Member Accounts" : "Problem Agent Accounts",
                Name: "colProblemAccountsNo",
                Width: 180,
                CellAlign: "right",
                HeaderAlign: "center"
            });
        }

        columns.push({ HeaderText: "Review Status", Name: "colReviewStatus", Width: 120, CellAlign: "center" });

        return {
            columns: columns,
            bodyRows: bodyRows,
            gridExpandHeight: 236,
            showDistinctRow: false,
            allowTruncate: true,
            showPager: true,
            customPager: true,
            pagerOption: $.extend({
                itemsPerPageArray: [50, 100, 500],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                    var formData = filterParams[roleId];
                    formData.pageIndex = pageIndex;
                    formData.pageSize = pageSize;

                    getProblemAccounts(formData);
                }
            }, pagerOption)
        };
    }

    function setFilterUIControls(roleId, uplineName) {
        var formData = filterParams[roleId];
        var targetRoleId = roleId - 1;
        selector("#txt-uplineUsername-" + targetRoleId).val(uplineName);

        var uplineFromDate = formData.fromDate;
        var uplineToDate = formData.toDate;
        var $dtpFromDate = selector("#dtp-fromdate-" + targetRoleId);
        $dtpFromDate
            .datepicker("setDate", uplineFromDate)
            .datepicker("option", "maxDate", new Date(uplineToDate));
        var $dtpToDate = selector("#dtp-todate-" + targetRoleId);
        $dtpToDate
            .datepicker("setDate", uplineToDate)
            .datepicker("option", "minDate", new Date(uplineFromDate));

        var uplineReviewStatus = formData.reviewStatus;
        var $ddlReviewStatus = selector("#ddl-reviewStatus-" + targetRoleId);
        $ddlReviewStatus
            .val(uplineReviewStatus)
            .next("input.ui-autocomplete-input").val($ddlReviewStatus.find("option:selected").text());

        selector("#txt-searchUsername-" + targetRoleId).val("");

        selector("#form-problem-account-" + targetRoleId).trigger("submit");
    }

    function openSetToReviewedPopup(roleId, checkboxes) {
        var custIds = [];
        var usernames = [];
        $.each(checkboxes, function () {
            var $this = $(this);
            custIds.push($this.data("custid"));
            usernames.push($this.data("username"));
        });

        Fanex.ExtAcc.Form.ConfirmBox(
            "SET TO REVIEWED",
            $popupSetToReviewed.html(),
            600,
            roleId === 1 ? 200 : 300,
            false,
            function () {
                setToReviewedSubmit(roleId);
            },
            null,
            null,
            true,
            null,
            true);

        initSetToReviewedPopup(roleId, custIds, usernames);
    }

    function initSetToReviewedPopup(roleId, custIds, usernames) {
        roleId === 1 && $("#reviewed-option-container").remove();

        var defaultShowUsernameNumber = 10;
        var totalUsername = usernames.length;
        var selectedAccountsValue = usernames.slice(0, defaultShowUsernameNumber).join(", ");
        var $spanSelectedAccounts = $("#span-selected-accounts");

        if (totalUsername > defaultShowUsernameNumber) {
            selectedAccountsValue += ", ...";

            var linkShowAllAccounts = $("#link-show-all-accounts");
            linkShowAllAccounts
                .text(linkShowAllAccounts.text().replace("{0}", totalUsername))
                .removeClass("hide")
                .off("click")
                .on("click", function () {
                    $spanSelectedAccounts.text(usernames.join(", "));
                    linkShowAllAccounts.remove();
                });
        }

        $spanSelectedAccounts.text(selectedAccountsValue);
        $("#hidden-set-to-reviewed-custIds").val(custIds.join(","));
        $("#hidden-set-to-reviewed-roleId").val(roleId);
    }

    function setToReviewedSubmit(roleId) {
        Fanex.ExtAcc.BlockUI();
        var formSetToReviewed = $("#form-set-to-reviewed");
        $.post(formSetToReviewed.attr("action"), formSetToReviewed.serialize())
            .done(function (data) {
                var errMsg = data.errorMessage || data.ErrorMessage;
                if (errMsg) {
                    $("#msgContainer")
                        .html(errMsg)
                        .removeClass()
                        .addClass("errMsg");
                } else {
                    selector("#form-problem-account-" + roleId).trigger("submit");
                    $("#msgContainer")
                        .html("Update Successfully.")
                        .removeClass()
                        .addClass("successMsg");
                    setTimeout(Fanex.ExtAcc.Form.Close, 1000 * 3);
                }
            })
            .always(Fanex.ExtAcc.UnBlockUI);
    }

    $(function () {
        var $popupContainer = $("#div-popup-container");
        $popupSetToReviewed = $popupContainer.clone();
        hasHostPermission = $popupSetToReviewed.length;

        selector("#problem-account-tabs").tabs();
        initSMATab(selector("#form-problem-account-1"), 1);
        initSMATab(selector("#form-problem-account-2"), 2);
        initSMATab(selector("#form-problem-account-3"), 3);

        $popupContainer.remove();
    })
})(jQuery);