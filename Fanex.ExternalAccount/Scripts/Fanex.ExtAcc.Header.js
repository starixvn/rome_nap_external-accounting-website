/// <reference path="_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Header = {
    GroupLength: 0,
    theme: Fanex.ExtAcc.theme,
    Init: function () {
        var $this = this;

        var queryString = "#0";
        var items = location.href.split("#");

        if (location.href.indexOf("#") >= 0) {
            queryString = "#" + items[items.length - 1];
        }

        var links = $('a');

        $("#selectedGroupIndex").html(items.length == 1 ? "0" : items[1]);

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

                    $this.GroupLength = items.length;

                    // Get current Group Name
                    var indexes = location.href.split('/');
                    var selectedGroupName = indexes[indexes.length - 1];

                    for (var i = 0; i < items.length; i++) {
                        $("#ddlGroupName").append('<li class="divUL"><div class="sf-menu divLI" id="group' + items[i] + '" ignorehref="1">' + items[i] + '</div></li>');
                    }

                    $('#userMenu').superfish();
                    $("#accMenu").css('visibility', 'visible');

                    // Set Group Name
                    $('#selectedGroupName').html(selectedGroupName);
                    $('#group' + selectedGroupName).css('font-weight', 'bold');
                    $('#group' + selectedGroupName).css('color', '#fff');
                    $('#selectedGroup').html(selectedGroupName);
                    $("#groupsMenu").superfish();
                    $("#groupsMenu").css('visibility', 'visible');

                    for (var i = 0; i < links.length; i++) {
                        if ($(links[i]).attr('ignorehref') != '1' && links[i].href.indexOf('javascript') < 0 && links[i].href.indexOf('Fanex') < 0) {
                            var originalURL = links[i].href.split('#')[0];
                            links[i].href = originalURL + "/" + selectedGroupName; // queryString;
                        }
                    }

                    $('#dllUsername').bind('click', function (event) {
                        $('#dllUsername').hide();
                    });

                    $("#ddlGroupName").bind('click', function (event) {
                        if (event.target.id.indexOf('group') == 0) {
                            $('#ddlGroupName').hide();

                            for (var i = 0; i < $this.GroupLength; i++) {
                                $('#group' + i).css('font-weight', 'normal');
                                $('#group' + i).css('color', '#acacac');
                            }

                            var id = event.target.id;
                            selectedGroupName = event.target.innerHTML;

                            $(id).css('font-weight', 'bold');
                            $(id).css('color', '#fff');
                            $('#selectedGroup').html(selectedGroupName);
                            //$('#selectedGroup').html($(id).html() + '<span class="sf-sub-indicator"></span>');
                            $('#selectedGroupIndex').html(id);
                            $('#selectedGroupName').html(selectedGroupName);

                            if (location.href.indexOf('Home/Index') != -1
                                || location.href == (location.protocol + '//' + location.host + resolveClientUrl(''))
                                || location.href == (location.protocol + '//' + location.host + '/')) {
                                //|| location.href.indexOf(location.protocol + '//' + location.host + '/') != -1) {
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