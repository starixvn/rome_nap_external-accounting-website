var agentMinMaxPT = {
    initMinMaxPTSetting: function (sportId, data) {
        var winId = sportId + '_' + data.Bettypes[0].Id;
        var placeId = sportId + '_' + data.Bettypes[1].Id;
        return '<table class="tblcollapse w800">\
            <tr class="title">\
                <td colspan="3">Position Taking</td>\
            </tr>\
            <tr>\
                <td></td>\
                <td><b>Win</b></td>\
                <td><b>Place</b></td>\
            </tr>\
            <tr>\
                <td class="l">Max Position</td>\
                <td>\
                    <select onclick="minMaxPTRacingSetting.changeMaxPT(this);" max="' + data.Bettypes[0].PositionTaking.MaxUplinePt + '" min="' + data.Bettypes[0].PositionTaking.MinUplinePt + '" class="pt" id="' + winId + '_maxpt">\
                        <option value="' + data.Bettypes[0].PositionTaking.UplinePt + '">' + data.Bettypes[0].PositionTaking.UplinePt + '</option>\
                    </select>\
                </td>\
                <td>\
                    <select onclick="minMaxPTRacingSetting.changeMaxPT(this);" max="' + data.Bettypes[1].PositionTaking.MaxUplinePt + '" min="' + data.Bettypes[1].PositionTaking.MinUplinePt + '" class="pt" id="' + placeId + '_maxpt">\
                        <option value="' + data.Bettypes[1].PositionTaking.UplinePt + '">' + data.Bettypes[1].PositionTaking.UplinePt + '</option>\
                    </select>\
                </td>\
            </tr>\
            <tr>\
                <td class="l">Min Position</td>\
                <td>\
                    <select onclick="minMaxPTRacingSetting.changeMinPT(this);" max="' + data.Bettypes[0].PositionTaking.MaxPt + '" min="' + data.Bettypes[0].PositionTaking.MinPt + '" class="pt" id="' + winId + '_minpt">\
                        <option value="' + data.Bettypes[0].PositionTaking.Pt + '">' + data.Bettypes[0].PositionTaking.Pt + '</option>\
                    </select>\
                </td>\
                <td>\
                    <select onclick="minMaxPTRacingSetting.changeMinPT(this);" max="' + data.Bettypes[1].PositionTaking.MaxPt + '" min="' + data.Bettypes[1].PositionTaking.MinPt + '" class="pt" id="' + placeId + '_minpt">\
                        <option value="' + data.Bettypes[1].PositionTaking.Pt + '">' + data.Bettypes[1].PositionTaking.Pt + '</option>\
                    </select>\
                </td>\
            </tr>\
            <tr>\
                <td></td>\
                <td colspan="2" class="l"><input type="checkbox" onclick="minMaxPTRacingSetting.chkCopyFromWin(this)" id="' + winId + '_chkCopyFromWin" />&nbsp;<label for="' + winId + '_chkCopyFromWin">Copy from ' + data.Bettypes[0].Name + '</td>\
            </tr>\
        </table>';
    }
};