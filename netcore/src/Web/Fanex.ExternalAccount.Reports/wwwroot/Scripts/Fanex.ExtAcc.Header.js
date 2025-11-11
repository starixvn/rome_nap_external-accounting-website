/// <reference path="_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Header = {
    Groups: [],
    GroupLength: 0,
    theme: Fanex.ExtAcc.theme,
    $this: null,

    Init: function () {
        $this = this;

        // Group
        $.ajax({
            url: resolveClientUrl('Account/ExternalGroupList'),
            type: 'GET',
            dataType: 'json',
            cache: false,
            success: function (data) {
                var items = data;

                if (!Array.isArray(items) || !items.length) {
                    $('#selectedGroupName').html($('#groupName').html());

                    $('#userMenu').superfish();
                    $("#accMenu").css('visibility', 'visible');

                    $('#accMenuGroup').css('display', 'none');
                    $('#grpText').css('display', 'none');

                    return;
                }

                Fanex.ExtAcc.Header.Groups = items;
                Fanex.ExtAcc.Header.GroupLength = items.length;

                // Get current Group Name
                var indexes = location.href.split('/');
                var groupNameSelected = indexes[indexes.length - 1];
                groupNameSelected = Fanex.ExtAcc.ReplaceAll(Fanex.ExtAcc.ReplaceAll(groupNameSelected, "%20", " "), "#", "");
                var hasGroup = false;
                $(items).each(function (i, v) {
                    if (v.GroupName == groupNameSelected)
                        hasGroup = true;
                });
                var selectedGroupName = groupNameSelected == '' || !hasGroup ? items[0].GroupName : groupNameSelected;

                if (selectedGroupName.indexOf('#') != -1) {
                    selectedGroupName = selectedGroupName.split('#')[1];
                }

                for (var i = 0; i < items.length; i++) {
                    $("#ddlGroupName").append('<option ' + (selectedGroupName == items[i].GroupName ? 'selected' : '') + ' value="' + items[i].GroupId + '" id="group' + Fanex.ExtAcc.ReplaceAll(items[i].GroupName, ' ', '') + '" >' + items[i].GroupName + '</option>');
                }

                $('#userMenu').superfish();
                $("#accMenu").css('visibility', 'visible');
                // Set Group name
                $('#selectedGroupName').html(selectedGroupName);
                $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', '')).css('font-weight', 'bold');
                $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', '')).css('color', '#fff');
                $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, ' ', '')).css('font-weight', 'bold');
                $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, ' ', '')).css('color', '#fff');
                $('#selectedGroup').html(Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', ' '));
                $("#groupsMenu").superfish();
                $("#groupsMenu").css('visibility', 'visible');
                var groupId = $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', ' ')).attr('groupid');
                $('#selectedGroupIndex').html(groupId);
                $("#ddlGroupName").select2();
                $('#userMenu').bind('mouseover', function () {
                    $('#ddlGroupName').hide();
                    $('#groupNameMenu').removeClass('sfHover');
                });

                $('#userMenu').bind('click', function () {
                    $('#dllUsername').hide();
                })

                $('#groupsMenu').bind('mouseover', function () {
                    $('#dllUsername').hide();
                    $('#userNameMenu').removeClass('sfHover');
                });

                $('#dllUsername').bind('click', function (event) {
                    $('#dllUsername').hide();
                });

                $("#ddlGroupName").on('select2:select', function (event) {
                    selectedGroupName = event.params.data.text;
                    $('#selectedGroupIndex').html(event.params.data.id);
                    $('#selectedGroupName').html(selectedGroupName);
                    $.ajax({
                        url: resolveClientUrl('Shared/ChangeSelectedGroup?groupName=' + selectedGroupName),
                        type: 'GET',
                        dataType: 'json',
                        cache: false,
                        complete: function () {
                            if ((location.href.indexOf('Home/Index') != -1
                                || location.href == (location.protocol + '//' + location.host + resolveClientUrl(''))
                                || location.href == (location.protocol + '//' + location.host + '/'))
                                && !document.getElementById('customerListGrid') && !document.getElementById('txtUserName')
                                && (document.getElementById('leftFrame').getAttribute('src') == '' || !document.getElementById('leftFrame').getAttribute('src'))) {
                                if (window.history.pushState) {
                                    var currentURL = location.href;

                                    if (!currentURL.toLowerCase().endsWith("home/index")) {
                                        currentURL = currentURL.substr(0, currentURL.lastIndexOf('/'));
                                    }

                                    window.history.pushState("object or string", "Title", currentURL + "/" + selectedGroupName);
                                }
                                else {
                                    location.hash = id.split('group')[1];
                                }
                                document.location.reload();
                            }
                            else {
                                location.href = location.protocol + '//' + location.host + resolveClientUrl('') + 'Home/Index/' + selectedGroupName;
                            }
                        }
                    });
                });
            }
        });

        $("#mainContent").css("display", "block");
    }
}