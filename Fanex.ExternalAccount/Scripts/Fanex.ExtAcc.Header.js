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
                    var index = 0;
                    var items = eval(data);
                    Fanex.ExtAcc.Header.Groups = items;

                    Fanex.ExtAcc.Header.GroupLength = items.length;

                    // Get current Group Name
                    var indexes = location.href.split('/');
                    var selectedGroupName = indexes[indexes.length - 1];

                    if (selectedGroupName.indexOf('#') != -1) {
                        selectedGroupName = selectedGroupName.split('#')[1];
                    }

                    for (var i = 0; i < items.length; i++) {
                        $("#ddlGroupName").append('<li class="divUL"><div class="sf-menu divLI" id="group' + Fanex.ExtAcc.ReplaceAll(items[i], ' ', '') + '" ignorehref="1">' + items[i] + '</div></li>');
                    }

                    $('#userMenu').superfish();
                    $("#accMenu").css('visibility', 'visible');
                    // Set Group name
                    $('#selectedGroupName').html(selectedGroupName);
                    $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', '')).css('font-weight', 'bold');
                    $('#group' + Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', '')).css('color', '#fff');
                    $('#selectedGroup').html(Fanex.ExtAcc.ReplaceAll(selectedGroupName, '%20', ' '));
                    $("#groupsMenu").superfish();
                    $("#groupsMenu").css('visibility', 'visible');

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

                    for (var i = 0; i < links.length; i++) {
                        try {
                            if ($(links[i]).attr('ignorehref') != '1' && links[i].href.indexOf('javascript') < 0 && links[i].href.indexOf('Fanex') < 0) {
                                var originalURL = links[i].href;
                                var indexOriUrl = originalURL.split('/')[3];
                                if (indexOriUrl.toLowerCase() == "statement") {
                                    links[i].href = originalURL + "/1539009/" + selectedGroupName;
                                }
                                else {
                                    links[i].href = originalURL + "/" + selectedGroupName; // queryString;
                                }
                            }
                            else {
                                links[i].href = 'javascript:;';
                            }
                        }
                        catch (err) {
                        }
                    }

                    $('#dllUsername').bind('click', function (event) {
                        $('#dllUsername').hide();
                    });

                    $("#ddlGroupName").bind('click', function (event) {
                        if (event.target.id.indexOf('group') == 0) {
                            $('#ddlGroupName').hide();

                            for (var i = 0; i < Fanex.ExtAcc.Header.GroupLength; i++) {
                                $('#group' + Fanex.ExtAcc.ReplaceAll(Fanex.ExtAcc.Header.Groups[i], ' ', '')).css('font-weight', 'normal');
                                $('#group' + Fanex.ExtAcc.ReplaceAll(Fanex.ExtAcc.Header.Groups[i], ' ', '')).css('color', '#acacac');
                            }

                            var id = event.target.id;
                            selectedGroupName = event.target.innerHTML;
                            $('#' + Fanex.ExtAcc.ReplaceAll(id, ' ', '')).css('font-weight', 'bold');
                            $('#' + Fanex.ExtAcc.ReplaceAll(id, ' ', '')).css('color', '#fff');
                            $('#selectedGroup').html(selectedGroupName + '<span class="sf-sub-indicator"> »</span>');
                            $('#selectedGroupIndex').html(id);
                            $('#selectedGroupName').html(selectedGroupName);

                            if ((location.href.indexOf('Home/Index') != -1
                                || location.href == (location.protocol + '//' + location.host + resolveClientUrl(''))
                                || location.href == (location.protocol + '//' + location.host + '/')) && !document.getElementById('mainGrid')) {                                
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

                            var links = $('a');
                            for (var i = 0; i < links.length; i++) {
                                if ($(links[i]).attr('ignorehref') != '1' && links[i].href.indexOf('javascript') < 0 && links[i].href.indexOf('Fanex') < 0) {
                                    links[i].href = links[i].href.substr(0, links[i].href.lastIndexOf('/')) + "/" + selectedGroupName;
                                }
                                else {
                                    links[i].href = 'javascript:;';
                                }
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