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

        var links = $('a');

        // Menu
        $("#functionsMenu").superfish({
        });

        if (Fanex.ExtAcc.IsITAdmin == 'True' || Fanex.ExtAcc.IsInternalAdmin == 'True') {
            // Group
            $.ajax({
                url: resolveClientUrl('Account/ExternalGroupList'),
                type: 'GET',
                dataType: 'html',
                cache: false,
                success: function (data) {
                    var items = eval(data);
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
                        $("#ddlGroupName").append('<li class="divUL"><div class="sf-menu divLI" groupid="' + items[i].GroupId + '" id="group' + Fanex.ExtAcc.ReplaceAll(items[i].GroupName, ' ', '') + '" ignorehref="1">' + items[i].GroupName + '</div></li>');
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

                    $("#ddlGroupName").bind('click', function (event) {
                        if (event.target.id.indexOf('group') == 0) {
                            $('#ddlGroupName').hide();

                            for (var i = 0; i < Fanex.ExtAcc.Header.GroupLength; i++) {
                                $('#group' + Fanex.ExtAcc.ReplaceAll(Fanex.ExtAcc.Header.Groups[i].GroupName, ' ', '')).css('font-weight', 'normal');
                                $('#group' + Fanex.ExtAcc.ReplaceAll(Fanex.ExtAcc.Header.Groups[i].GroupName, ' ', '')).css('color', '#acacac');
                            }

                            var id = event.target.id;
                            var groupId = event.target.getAttribute('groupid');
                            selectedGroupName = event.target.innerHTML;
                            $('#' + Fanex.ExtAcc.ReplaceAll(id, ' ', '')).css('font-weight', 'bold');
                            $('#' + Fanex.ExtAcc.ReplaceAll(id, ' ', '')).css('color', '#fff');
                            $('#selectedGroup').html(selectedGroupName + '<span class="sf-sub-indicator"> »</span>');
                            $('#selectedGroupIndex').html(groupId);
                            $('#selectedGroupName').html(selectedGroupName);

                            if ((location.href.indexOf('Home/Index') != -1
                                || location.href == (location.protocol + '//' + location.host + resolveClientUrl(''))
                                || location.href == (location.protocol + '//' + location.host + '/'))
                                && !document.getElementById('customerListGrid') && !document.getElementById('txtUserName')
                                && (document.getElementById('leftFrame').getAttribute('src') == '' || !document.getElementById('leftFrame').getAttribute('src'))) {
                                if (window.history.pushState) {
                                    window.history.pushState("object or string", "Title", location.href.substr(0, location.href.lastIndexOf('/')) + "/" + selectedGroupName);
                                }
                                else {
                                    location.hash = id.split('group')[1];
                                }
                            }
                            else {
                                location.href = location.protocol + '//' + location.host + resolveClientUrl('') + 'Home/Index/' + selectedGroupName;
                            }
                        }
                    });
                },
                error: function (xhr, status, data) {
                    if (typeof console != 'undefined') console.log(data);
                }
            });
        }
        else {
            $('#selectedGroupName').html($('#groupName').html());

            $('#userMenu').superfish();
            $("#accMenu").css('visibility', 'visible');

            $('#accMenuGroup').css('display', 'none');
            $('#grpText').css('display', 'none');
        }

        $("#mainContent").css("display", "block");
    }
}