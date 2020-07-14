if (typeof Fanex === 'undefined') {
    Fanex = {};
}

if (typeof Fanex.ExtAcc === 'undefined') {
    Fanex.ExtAcc = {};
}

Fanex.ExtAcc.SSO = {
    imageAddress: "/assets/styles/images/sample_test_network_speed_1.jpg",
    lines: [],
    maxTimesLength: 5,
    addedErrorTime: 1000 * 60 * 5, // 5 minutes
    intervalTime: 1000 * 5, // 1 minute

    Init: function () {
        Fanex.ExtAcc.SSO.InitDropdownList();

        $.each(this.lines, function (index, value) {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(value);
        });
    },

    InitDropdownList: function () {
        var ddlSwitchLines = $(".ddl-switchLine").select2({
            theme: "switchline",
            minimumResultsForSearch: -1,
            templateResult: function (line) {
                if (!line.id) {
                    return null;
                }

                var $origOption = $(line.element);
                var classname = $origOption.attr("data-networkspeed");
                var text = line.text + (line.selected ? " - (In use)" : '');

                return $('<span>' + text + '</span><span class="' + classname + '"></span>');
            }
        });

        $(ddlSwitchLines[0]).find("option").each(function () {
            Fanex.ExtAcc.SSO.lines.push({
                elements: $("." + this.className),
                url: this.value,
                times: [],
                index: 0,
                average: 0
            });
        });
    },

    MeasureConnectionSpeed: function (line) {
        var startTime;
        var download = new Image();
        download.onload = function () {
            var endTime = (new Date()).getTime();
            Fanex.ExtAcc.SSO.UpdateConnectionSpeed(line, endTime - startTime);
        };

        download.onerror = function () {
            Fanex.ExtAcc.SSO.UpdateConnectionSpeed(line, Fanex.ExtAcc.SSO.addedErrorTime);
        };

        startTime = (new Date()).getTime();
        var cacheBuster = "?v=" + startTime;
        download.src = line.url + Fanex.ExtAcc.SSO.imageAddress + cacheBuster;
    },

    UpdateConnectionSpeed: function (line, value) {
        if (line.index >= Fanex.ExtAcc.SSO.maxTimesLength) {
            line.index = 0;
        }

        line.times[line.index++] = value;
        line.average = Fanex.ExtAcc.SSO.CalculateSpeedAverage(line.times);
        Fanex.ExtAcc.SSO.AssignNetworkSpeedIcon(line);

        setTimeout(function () {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(line);
        }, Fanex.ExtAcc.SSO.intervalTime);
    },

    CalculateSpeedAverage: function (values) {
        var total = 0;
        var max = values.length;

        for (var i = 0; i < max; i++) {
            total += values[i];
        }

        return Math.round(total / max);
    },

    AssignNetworkSpeedIcon: function (currentLine) {
        var line = Fanex.ExtAcc.SSO.lines[0];
        var min = line.average;
        var max = line.average;

        for (var i = 1, maxLen = Fanex.ExtAcc.SSO.lines.length; i < maxLen; i++) {
            line = Fanex.ExtAcc.SSO.lines[i];

            if (line.average < min) {
                min = line.average;
            }

            if (line.average > max) {
                max = line.average;
            }
        }

        if (currentLine.average <= min) {
            currentLine.elements.attr("data-networkspeed", "icon-wifi");
        } else if (currentLine.average >= max) {
            currentLine.elements.attr("data-networkspeed", "icon-network-weak");
        } else {
            currentLine.elements.attr("data-networkspeed", "icon-network-medium");
        }
    }
}

$(function () {
    Fanex.ExtAcc.SSO.Init();
});