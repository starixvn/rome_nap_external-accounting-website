/// <reference path="../_references.js" />
if (typeof Fanex === "undefined" || typeof Fanex.ExtAcc === "undefined") {
    alert("Fanex.ExtAcc.js module is required!")
}

(function ($) {
    var $problemAccountTabs = $("#problem-account-tabs"),
        $formMemberList = $("#form-member-list"),
        $formAgentList = $("#form-agent-list");

    function initSMATab($form, roleId) {
        initFromToDatePicker($form.find(".dtp-from"), $form.find(".dtp-to"));
        initCombobox($form);

        bindEvents($form, roleId);

        roleId === 3 && $form.trigger("submit");
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
            buttonText: "",
            dateFormat: "MM/dd/yyyy"
        }, options));
    }

    function initCombobox($form) {
        var $selectBox = $form.find(".select-box");
        var $ddlReviewStatus = $selectBox.find("select").combobox();
        $selectBox.find("input")
            .val($ddlReviewStatus.find("option:selected").text())
            .attr("readonly", "true")
            .bind("click", function () {
                $(this).autocomplete("search", "");
            });
    }

    function bindEvents($form, roleId) {
        var $gridContainer = $form.next(".freegrid-container");
        var $grid = $gridContainer.find(".freegrid");
        var $divNoData = $gridContainer.next(".div-noData");

        $form.submit(function (e) {
            e.preventDefault();
            Fanex.ExtAcc.BlockUI();
            var queryString = "?groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName + "&" + $form.serialize();
            $.getJSON(
                $form.attr("action") + queryString,
                function (data) {
                    onSubmitSuccess($gridContainer, $grid, $divNoData, roleId, data);
                }).done(Fanex.ExtAcc.UnBlockUI);
        });

        $form.find(".icon-excel").click(function () {
            var queryString = "?groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName + "&" + $form.serialize();
            var url = resolveClientUrl("ProblemAccount/ExportExcel") + queryString;
            top.exportExcel(url, document);
        });

        $gridContainer
            .find(".status-select")
            .off("click")
            .on("click", function () {
                var $this = $(this);
                var checkAll = $this.text() === "All";
                $grid
                    .find(".set-to-reviewed")
                    .removeClass()
                    .addClass(checkAll ? "menuActive set-to-reviewed" : "menuInactive set-to-reviewed");
            });
    }

    function onSubmitSuccess($gridContainer, $grid, $divNoData, roleId, data) {
        var bodyRows = [];
        var dataLength = data.length;
        for (var i = 0; i < dataLength; i++) {
            var datum = data[i];
            bodyRows.push({
                Columns: [
                    { Name: "colNo", Value: i + 1 },
                    { Name: "colUsername", Value: datum },
                    { Name: "colFirstName", Value: datum.FirstName },
                    { Name: "colLastName", Value: datum.LastName },
                    { Name: "colStatus", Value: datum.StatusName },
                    { Name: "colDateNotified", Value: datum.CreatedDateStr },
                    { Name: "colProblemAgentAccounts", Value: datum.NumberOfDownline },
                    { Name: "colReviewStatus", Value: datum.ReviewStatus },
                    { Name: "colSetReviewed", Value: datum }
                ],
                CssClass: getAccountStatusCssClass(datum)
            });
        }

        $grid.fanexGrid(initGridOptions(roleId, bodyRows));

        if (dataLength) {
            $grid
                .find(".drow-username")
                .off("click")
                .on("click", function () {
                    var $this = $(this);
                    setFormUIControls(roleId, $this.text(), undefined, undefined, 0);

                    if (roleId === 3) {
                        $problemAccountTabs.tabs("option", "active", 1);
                    } else {
                        $problemAccountTabs.tabs("option", "active", 2);
                    }
                });

            $grid
                .find(".menuInactive")
                .off("click")
                .on("click", function () {
                    var $this = $(this);
                    var checked = $this.hasClass("menuActive");
                    $this.removeClass().addClass(checked ? "menuInactive set-to-reviewed" : "menuActive set-to-reviewed")
                });

            $gridContainer.removeClass("hide");
            $divNoData.addClass("hide");
        } else {
            $gridContainer.addClass("hide");
            $divNoData.removeClass("hide");
        }
    }

    function getAccountStatusCssClass(datum) {
        if (datum.Status === 2) {
            return "rowSuspended";
        }

        if (datum.Status === 3) {
            return Fanex.ExtAcc.Customer.GetClosedClassName(datum);
        }

        if (datum.Status === 4) {
            return "rowDisabled";
        }

        return "";
    }

    function initGridOptions(roleId, bodyRows) {
        return {
            columns: [
                { HeaderText: "#", Name: "colNo", Width: 60, CellAlign: "center" },
                {
                    HeaderText: "Username",
                    Name: "colUsername",
                    Width: 200,
                    CustomCellRender: function (cell) {
                        return roleId === 1
                            ? cell.Value.Username
                            : '<a href="javascript:;" class="drow-username">' + cell.Value.Username + '</a>';
                    }
                },
                { HeaderText: "First Name", Name: "colFirstName", Width: 200 },
                { HeaderText: "Last Name", Name: "colLastName", Width: 200 },
                { HeaderText: "Status", Name: "colStatus", Width: 100, CellAlign: "center" },
                { HeaderText: "Date Notified", Name: "colDateNotified", Width: 140, CellAlign: "center" },
                { HeaderText: "Problem Agent Accounts", Name: "colProblemAgentAccounts", Width: 180, CellAlign: "right", HeaderAlign: "center" },
                { HeaderText: "Review Status", Name: "colReviewStatus", Width: 120, CellAlign: "center" },
                {
                    HeaderText: "",
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
                }
            ],
            gridExpandHeight: 256,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: bodyRows
        };
    }

    function setFormUIControls(roleId, uplineName, from, to, reviewStatus) {
        var $form = roleId === 3 ? $formAgentList : $formMemberList;
        var targetRoleId = roleId - 1;
        $form.find("#txt-uplineName-" + targetRoleId).val(uplineName);
        //$form.find("#txt-uplineName-" + targetRoleId).val(uplineName);
        //$form.find("#txt-uplineName-" + targetRoleId).val(uplineName);
        $form.find("#ddl-reviewStatus-" + targetRoleId).find("input").val(reviewStatus);
    }

    $(function () {
        $problemAccountTabs.tabs();

        initSMATab($formMemberList, 1);
        initSMATab($formAgentList, 2);
        initSMATab($("#form-master-list"), 3);
    })
})(jQuery);
