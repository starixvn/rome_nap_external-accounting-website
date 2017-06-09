Fanex.ExtAcc.Customer.EditMultiple = function (div) {
    this.ResetFlags();
    var menuItems = {};

    this.isSuperEditMember = false;
    this.isMasterEditMember = false;
    this.IsFullInfo = false;
    this.IsMultiple = true;
    var ret = this.IdsAndNames();
    var info = this.CustomerInfo();
    // this.HideGoldDeluxe = showGoldDeluxeSetting == 0;
 
    if (this.CurrentLevel == this.Super) {
        menuItems = {
            doubleComm: {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            }
        };
    } else if (this.CurrentLevel == this.Master) {

        menuItems = {
            sportbook: {
                name: "Edit Multiple Sportsbook",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                }
            },
            racing: {
                name: "Edit Multiple Racing",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/master?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                }
            }
        };

        if (this.superSyncCasino) {
            menuItems.casino = {
                name: "Edit Multiple Casino",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                }
            };
        }

        if (this.superSyncLiveCasino) {
            menuItems.liveCasino = {
                name: "Edit Multiple Live Casino",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.superId, Fanex.ExtAcc.Customer.superId, 4, 3);
                }
            };
        }

        if (!this.HideVirtualSports) {
            menuItems.virtualSport = {
                name: "Edit Multiple Virtual Sports",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                }
            };
        }

        if (!this.HideKeno) {
            menuItems.keno = {
                name: "Edit Multiple Keno",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                }
            };

        }

        if (!this.HideGoldDeluxe) {
            menuItems.goldDeluxe = {
                name: "Edit Multiple Gold Deluxe",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 4, 3);
                }
            };

        }

        if (!this.HideColossusBet) {
            menuItems.colossusBets = {
                name: "Edit Multiple ColossusBets",
                className: "iconEnableEdit",
                callback: function () {
                    url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&srecommend=" + Fanex.ExtAcc.Customer.superId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=3&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                }
            };

        }

        if (!this.HideSportsbook2) {
            menuItems.sportsbook2 = {
                name: "Edit Multiple Sportsbook2",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2(4, 3);
                }
            };
        }

        if (!this.HideCockfighting) {
            menuItems.cockFighting = {
                name: "Edit Multiple Cockfighting",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting(4, 3);
                }
            };
        }

        if (!this.HideCricket) {
            menuItems.cricket = {
                name: "Edit Multiple Cricket",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCricket(4, 3);
                }
            };
        }

        if (!this.HideAllbet) {
            menuItems.allbet = {
                name: "Edit Multiple Allbet",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleAllbet(4, 3);
                }
            };
        }

        if (!this.HideVoidbridge) {
            menuItems.voidbridge = {
                name: "Edit Multiple Macau Games",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge(4, 3);
                }
            };
        }

        if (!this.HideNG2) {
            menuItems.ng2 = {
                name: "Edit Multiple " + ng2Text,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleNG2(4, 3);
                }
            };
        }

        if (!this.HideFishingWorld) {
            menuItems.fishingWorld = {
                name: "Edit Multiple Fishing World",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld(4, 3);
                }
            };
        }
        
        if (this.AllowDoubleCommMaster) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }
    else if (this.CurrentLevel == this.Agent) {
        menuItems = {
            sportbook: {
                name: "Edit Multiple Sportsbook",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                }
            },
            racing: {
                name: "Edit Multiple Racing",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/agent?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                }
            }
        };

        if (this.masterSyncCasino) {
            menuItems.casino = {
                name: "Edit Multiple Casino",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                }
            };
        }

        if (this.masterSyncLiveCasino) {
            menuItems.liveCasino = {
                name: "Edit Multiple Live Casino",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.masterId, Fanex.ExtAcc.Customer.masterId, 3, 2);
                }
            };
        }

        if (!this.HideVirtualSports) {
            menuItems.virtualSport = {
                name: "Edit Multiple Virtual Sports",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Agent + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                }
            };
        }

        if (!this.HideKeno) {
            menuItems.keno = {
                name: "Edit Multiple Keno",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditAgent?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                }
            };

        }

        if (!this.HideGoldDeluxe) {
            menuItems.goldDeluxe = {
                name: "Edit Multiple Gold Deluxe",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.masterId, 3, 2);
                }
            };

        }


        if (!this.HideColossusBet) {
            menuItems.colossusBets = {
                name: "Edit Multiple ColossusBets",
                className: "iconEnableEdit",
                callback: function () {
                    url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&mrecommend=" + Fanex.ExtAcc.Customer.masterId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=2&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                }
            };

        }

        if (!this.HideSportsbook2) {
            menuItems.sportsbook2 = {
                name: "Edit Multiple Sportsbook2",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2(3, 2);
                }
            };
        }

        if (!this.HideCockfighting) {
            menuItems.cockFighting = {
                name: "Edit Multiple Cockfighting",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting(3, 2);
                }
            };
        }

        if (!this.HideCricket) {
            menuItems.cricket = {
                name: "Edit Multiple Cricket",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleCricket(3, 2);
                }
            };
        }

        if (!this.HideAllbet) {
            menuItems.allbet = {
                name: "Edit Multiple Allbet",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleAllbet(3, 2);
                }
            };
        }

        if (!this.HideVoidbridge) {
            menuItems.voidbridge = {
                name: "Edit Multiple Macau Games",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge(3, 2);
                }
            };
        }

        if (!this.HideNG2) {
            menuItems.ng2 = {
                name: "Edit Multiple " + ng2Text,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleNG2(3, 2);
                }
            };
        }

        if (!this.HideFishingWorld) {
            menuItems.fishingWorld = {
                name: "Edit Multiple Fishing World",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld(3, 2);
                }
            };
        }
        
        if (this.AllowDoubleCommAgent) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }
    else {
        menuItems = {
            sportbook: {
                name: "Sportsbook",
                className: "iconEnableEdit",
                items: {
                    agentSB: {
                        name: "Edit Multiple Sportsbook",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                        }
                    },
                    superSB: {
                        name: "Super Edit Multiple Sportsbook",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT();
                        }
                    },
                    maserSB: {
                        name: "Master Edit Multiple Sportsbook",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT();
                        }
                    }
                }
            },
            racing: {
                name: "Racing",
                className: "iconEnableEdit",
                items: {
                    agentRC: {
                        name: "Edit Multiple Racing",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/member?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                        }
                    },
                    superRC: {
                        name: "Super Edit Multiple Racing",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/supereditmember?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Super Edit Multiple Racing Setting' : 'Super Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                        }
                    },
                    masterRC: {
                        name: "Master Edit Multiple Racing",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/mastereditmember?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Master Edit Multiple Racing Setting' : 'Master Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                        }
                    }
                }
            }
        };

        if (this.agentSyncCasino) {
            menuItems.casino = {
                name: "Casino",
                className: "iconEnableEdit",
                items: {
                    agentCS: {
                        name: "Edit Multiple Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                        }
                    },
                    superCS: {
                        name: "Super Edit Multiple Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                        }
                    },
                    masterCS: {
                        name: "Master Edit Multiple Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                        }
                    }
                }
            };
        }

        if (this.agentSyncLiveCasino) {
            menuItems.liveCasino = {
                name: "Live Casino",
                className: "iconEnableEdit",
                items: {
                    agentLC: {
                        name: "Edit Multiple Live Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.agentId, Fanex.ExtAcc.Customer.agentId, 2, 1);
                        }
                    },
                    superLC: {
                        name: "Super Edit Multiple Live Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.superId, Fanex.ExtAcc.Customer.agentId, 4, 1);
                        }
                    },
                    masterLC: {
                        name: "Master Edit Multiple Live Casino",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.masterId, Fanex.ExtAcc.Customer.agentId, 3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideVirtualSports) {
            menuItems.virtualSport = {
                name: "Virtual Sports",
                className: "iconEnableEdit",
                items: {
                    agentVS: {
                        name: "Edit Multiple Virtual Sports",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.agentId + '&roleId=' + Fanex.ExtAcc.Customer.Agent + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                        }
                    },
                    superVS: {
                        name: "Super Edit Multiple Virtual Sports",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Super Edit Multiple Virtual Sports Setting' : 'Super Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                        }
                    },
                    masterVS: {
                        name: "Master Edit Multiple Virtual Sports",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Master Edit Multiple Virtual Sports Setting' : 'Master Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideKeno) {
            menuItems.keno = {
                name: "Keno",
                className: "iconEnableEdit",
                items: {
                    agentKN: {
                        name: "Edit Multiple Keno",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                        }
                    },
                    superKN: {
                        name: "Super Edit Multiple Keno",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/SuperEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                        }
                    },
                    masterKN: {
                        name: "Master Edit Multiple Keno",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/MasterEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideGoldDeluxe) {
            menuItems.goldDeluxe = {
                name: "Gold Deluxe",
                className: "iconEnableEdit",
                items: {
                    agentGD: {
                        name: "Edit Multiple Gold Deluxe",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 2, 1);
                        }
                    },
                    superGD: {
                        name: "Super Edit Multiple Gold Deluxe",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 4, 1);
                        }
                    },
                    masterGD: {
                        name: "Master Edit Multiple Gold Deluxe",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 3, 1);
                        }
                    }
                }
            };
        }


        if (!this.HideColossusBet) {
            menuItems.colossusBets = {
                name: "Edit Multiple ColossusBets",
                className: "iconEnableEdit",
                callback: function () {
                    url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=1&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                }
            };
        }

        if (!this.HideSportsbook2) {
            menuItems.sportsbook2 = {
                name: "Sportsbook2",
                className: "iconEnableEdit",
                items: {
                    agentSB2: {
                        name: "Edit Multiple Sportsbook 2",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2(2, 1);
                        }
                    },
                    superSB2: {
                        name: "Super Edit Multiple Sportsbook2",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2(4, 1);
                        }
                    },
                    masterSB2: {
                        name: "Master Edit Multiple Sportsbook2",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2(3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideCockfighting) {
            menuItems.cockFighting = {
                name: "Cockfighting",
                className: "iconEnableEdit",
                items: {
                    agentCF: {
                        name: "Edit Multiple Cockfighting",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting(2, 1);
                        }
                    },
                    superCF: {
                        name: "Super Edit Multiple Cockfighting",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting(4, 1);
                        }
                    },
                    masterCF: {
                        name: "Master Edit Multiple Cockfighting",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting(3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideCricket) {
            menuItems.cricket = {
                name: "Cricket",
                className: "iconEnableEdit",
                items: {
                    agentCK: {
                        name: "Edit Multiple Cricket",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCricket(2, 1);
                        }
                    },
                    superCK: {
                        name: "Super Edit Multiple Cricket",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCricket(4, 1);
                        }
                    },
                    masterCK: {
                        name: "Master Edit Multiple Cricket",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleCricket(3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideAllbetMember) {
            menuItems.allbet = {
                name: "Allbet",
                className: "iconEnableEdit",
                items: {
                    agentAB: {
                        name: "Edit Multiple Allbet",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleAllbet(2, 1);
                        }
                    },
                    superAB: {
                        name: "Super Edit Multiple Allbet",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleAllbet(4, 1);
                        }
                    },
                    masterAB: {
                        name: "Master Edit Multiple Allbet",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleAllbet(3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideVoidbridgeMember) {
            menuItems.voidbridge = {
                name: "Macau Games",
                className: "iconEnableEdit",
                items: {
                    agentVB: {
                        name: "Edit Multiple Macau Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge(2, 1);
                        }
                    },
                    superVB: {
                        name: "Super Edit Multiple Macau Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge(4, 1);
                        }
                    },
                    masterVB: {
                        name: "Master Edit Multiple Macau Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge(3, 1);
                        }
                    }
                }
            };
        }

        if (!this.HideNG2Member) {
            menuItems.ng2 = {
                name: ng2Text,
                className: "iconEnableEdit",
                items: {
                    agentNG2: {
                        name: "Edit Multiple " + ng2Text,
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleNG2(2, 1);
                        }
                    },
                    superNG2: {
                        name: "Super Edit Multiple " + ng2Text,
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleNG2(4, 1);
                        }
                    },
                    masterNG2: {
                        name: "Master Edit Multiple " + ng2Text,
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleNG2(3, 1);
                        }
                    }
                }
            };
        }

         if (!this.HideFishingWorldMember) {
            menuItems.fishingWorld = {
                name: "Fishing World",
                className: "iconEnableEdit",
                items: {
                    agentFW: {
                        name: "Edit Fishing World Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld(2, 1);
                        }
                    },
                    superFW: {
                        name: "Super Edit Fishing World Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = true;
                            Fanex.ExtAcc.Customer.isMasterEditMember = false;
                            Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld(4, 1);
                        }
                    },
                    masterFW: {
                        name: "Master Edit Fishing World Games",
                        callback: function () {
                            Fanex.ExtAcc.Customer.isSuperEditMember = false;
                            Fanex.ExtAcc.Customer.isMasterEditMember = true;
                            Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld(3, 1);
                        }
                    }
                }
            };
        }
      
        if (this.AllowDoubleComm) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editMult',
        items: menuItems,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.UpdateMultipleDoubleComm = function (allow) {
    var ret = this.IdsAndNames();

    this.ConfigMultipleDoubleComm(ret.Ids, allow, 0);
};

Fanex.ExtAcc.Customer.ShowEditMultiplePT = function () {
    var ret = this.IdsAndNames();

    if (!this.IsMultiple)
        Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
    else
        Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Multiple Sportsbook Setting', 894, 700, false, false, 1);
};

Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino = function (uplineId, directUplineId, uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = '';

    if (this.CurrentLevel == this.Master) {
        actionName = 'EditMaster';
    } else if (this.CurrentLevel == this.Agent) {
        actionName = 'EditAgent';
    } else {
        if (this.isSuperEditMember) {
            actionName = 'SuperEditMember';
            levelText = 'Super ';
        } else if (this.isMasterEditMember) {
            actionName = 'MasterEditMember';
            levelText = 'Master ';
        } else {
            actionName = 'EditMember';
        }
    }

    if (this.IsMultiple) {
        document.getElementById('parentId').value = directUplineId;
        document.getElementById('uplineId').value = uplineId;
        document.getElementById('customerIds').value = Fanex.ExtAcc.Customer.CustIds;
        document.getElementById('syncStatuses').value = Fanex.ExtAcc.Customer.LiveCasinoStatuses;
        document.getElementById('usernames').value = Fanex.ExtAcc.Customer.Usernames.replaceAll(' ', '');
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('actorId').value = parent.userId;
        document.getElementById('actorIdUsername').value = parent.username;
        document.getElementById('subAccId').value = 0;
        document.getElementById('language').value = 'E';
        document.getElementById('groupName').value = Fanex.ExtAcc.Customer.GroupName().groupName;

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('LiveCasinoSetting/Setting/EditMultipleSetting'), levelText + 'Edit Multiple Live Casino Setting', 900, 700, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('LiveCasinoSetting/Setting/EditMultipleSetting')).submit();
    }
    else {
        var url = '';
        switch (uplineRoleId) {
            case 4: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.superId + "&parentId=" + Fanex.ExtAcc.Customer.superId + "&uplineLevel=4&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                break;
            case 3: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.masterId + "&parentId=" + Fanex.ExtAcc.Customer.masterId + "&uplineLevel=3&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                break;
            default: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.agentId + "&parentId=" + Fanex.ExtAcc.Customer.agentId + "&uplineLevel=2&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                break;
        }

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Live Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, false, 1);
    }
};

Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe = function (uplineId, uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = '';

    if (this.CurrentLevel == this.Master) {
        actionName = 'EditMaster';
    } else if (this.CurrentLevel == this.Agent) {
        actionName = 'EditAgent';
    } else {
        if (this.isSuperEditMember) {
            actionName = 'SuperEditMember';
            levelText = 'Super ';
        } else if (this.isMasterEditMember) {
            actionName = 'MasterEditMember';
            levelText = 'Master ';
        } else {
            actionName = 'EditMember';
        }
    }

    if (this.IsMultiple) {
        var goldDeluxeUsernames = '';
        var goldDeluxeCustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            goldDeluxeUsernames += userNamesArr[i] + linkChar;
            goldDeluxeCustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('uplineId').value = uplineId;
        document.getElementById('customerIds').value = goldDeluxeCustIds;
        document.getElementById('usernames').value = goldDeluxeUsernames;
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('actorId').value = parent.userId;
        document.getElementById('subAccId').value = 0;
        document.getElementById('language').value = 'E';
        document.getElementById('groupName').value = Fanex.ExtAcc.Customer.GroupName().groupName;

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSettingMultiple'), levelText + 'Edit Multiple Gold Deluxe Setting', 900, 400, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSettingMultiple')).submit();
    }
    else {
        var url = '';
        switch (uplineRoleId) {
            case 4: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.superId;
                break;
            case 3: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.masterId;
                break;
            default: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.agentId;
                break;
        }

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Gold Deluxe Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, false, 1);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleSportsbook2 = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 16;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Sportsbook2 Setting', 900, 600, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleSportsbook2 = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var goldDeluxeUsernames = '';
        var goldDeluxeCustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            goldDeluxeUsernames += userNamesArr[i] + linkChar;
            goldDeluxeCustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 16;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = goldDeluxeCustIds;
        document.getElementById('customerNames').value = goldDeluxeUsernames;
        document.getElementById('uplineLevel').value = "";


        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Sportsbook2 Setting', 900, 600, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleSportsbook2(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleCockfighting = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 19;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Cockfighting Setting', 900, 600, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleCockfighting = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var goldDeluxeUsernames = '';
        var goldDeluxeCustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            goldDeluxeUsernames += userNamesArr[i] + linkChar;
            goldDeluxeCustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 19;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = goldDeluxeCustIds;
        document.getElementById('customerNames').value = goldDeluxeUsernames;
        document.getElementById('uplineLevel').value = "";

        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Cockfighting Setting', 900, 600, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleCockfighting(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleCricket = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 20;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Cricket Setting', 900, 600, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleCricket = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var goldDeluxeUsernames = '';
        var goldDeluxeCustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            goldDeluxeUsernames += userNamesArr[i] + linkChar;
            goldDeluxeCustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 20;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = goldDeluxeCustIds;
        document.getElementById('customerNames').value = goldDeluxeUsernames;
        document.getElementById('uplineLevel').value = "";


        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Cricket Setting', 900, 600, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleCricket(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleAllbet = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 21;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    var isForcingMode = Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts("" + custid, 21);
    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(isForcingMode);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Allbet Setting', 900, 600, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleAllbet = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        var customerIdsElement = document.getElementById('customerIds');

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 21;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        customerIdsElement.value = (uplineRoleId - this.CurrentLevel != 1 ? this.AllbetSyncIdList.join(', ') : CustIds);
        document.getElementById('customerNames').value = (uplineRoleId - this.CurrentLevel != 1 ? this.AllbetSyncList.join(', ') : Usernames);
        document.getElementById('uplineLevel').value = "";

        if (customerIdsElement.value == "") {
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Views/_blank.html'), levelText + 'Edit Multiple Allbet Setting', 900, 600, false, false, 1);
            return;
        }

        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        var isForcingMode = Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts(customerIdsElement.value, 21);
        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(isForcingMode);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Allbet Setting', 900, 600, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleAllbet(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleVoidbridge = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 22;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    var isForcingMode = Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts("" + custid, 22);
    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(isForcingMode);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Macau Games Setting', 900, 750, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleVoidbridge = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        var customerIdsElement = document.getElementById('customerIds');
        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 22;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        customerIdsElement.value = (uplineRoleId - this.CurrentLevel != 1 ? this.VoidbridgeSyncIdList.join(', ') : CustIds);
        document.getElementById('customerNames').value = (uplineRoleId - this.CurrentLevel != 1 ? this.VoidbridgeSyncList.join(', ') : Usernames);
        document.getElementById('uplineLevel').value = "";

        if (customerIdsElement.value == "") {
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Views/_blank.html'), levelText + 'Edit Multiple Macau Games Setting', 900, 600, false, false, 1);
            return;
        }

        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        var isForcingMode = Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts(customerIdsElement.value, 22);
        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(isForcingMode);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Macau Games Setting', 900, 750, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleVoidbridge(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleFishingWorld = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 24;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";

    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit Fishing World Setting', 900, 750, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleFishingWorld = function (uplineRoleId, targetRoleId) {
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 24;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = (uplineRoleId - this.CurrentLevel != 1 ? this.FishingWorldSyncIdList.join(', ') : CustIds);
        document.getElementById('customerNames').value = (uplineRoleId - this.CurrentLevel != 1 ? this.FishingWorldSyncList.join(', ') : Usernames);
        document.getElementById('uplineLevel').value = "";

        if (document.getElementById('customerIds').value == "") {
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Views/_blank.html'), levelText + 'Edit Multiple Fishing World Setting', 900, 600, false, false, 1);
            return;
        }

        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple Fishing World Setting', 900, 750, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleFishingWorld(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowEditSingleNG2 = function (custid, uplineRoleId, targetRoleId) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = 23;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit ' + ng2Text + ' Setting', 900, 750, false, false, 1);
    $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
};

Fanex.ExtAcc.Customer.ShowEditMultipleNG2 = function (uplineRoleId, targetRoleId) {
    var ret = this.IdsAndNames();

    var actionName = '';
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = this.Names();
        var custIdsArr = this.Ids()

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = 23;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = (uplineRoleId - this.CurrentLevel != 1 ? this.NG2SyncIdList.join(', ') : CustIds);
        document.getElementById('customerNames').value = (uplineRoleId - this.CurrentLevel != 1 ? this.NG2SyncList.join(', ') : Usernames);
        document.getElementById('uplineLevel').value = "";

        if (document.getElementById('customerIds').value == "") {
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Views/_blank.html'), levelText + 'Edit Multiple ' + ng2Text + ' Setting', 900, 600, false, false, 1);
            return;
        }


        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField(false);

        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple ' + ng2Text + ' Setting', 900, 750, false, false, 1);
        $("#targetForm").attr("action", resolveClientUrl('DCS/ViewCustomerSetting')).submit();
    }
    else {
        this.ShowEditSingleNG2(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId);
    }
};

Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT = function () {
    var ret = this.IdsAndNames();
    var levelText = '';

    if (this.isSuperEditMember) {
        levelText = 'Super ';
    } else if (this.isMasterEditMember) {
        levelText = 'Master ';
    }

    if (!this.IsMultiple) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
    } else {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/SuperMasterEditMultiplePTMember?roleId=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Multiple Sportsbook Setting', 894, 700, false, false, 1);
    }
};

Fanex.ExtAcc.Customer.ShowEditMultipleCasino = function () {
    var ret = this.IdsAndNames();
    var data = this.CustomerInfo();
    var levelText = '';
    var actionName = 'CustomerSetting/Casino/';

    switch (this.CurrentLevel) {
        case this.Master:
            actionName += 'Master/EditMaster/' + this.Ids()[0] + '/' + this.superId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        case this.Agent:
            actionName += 'Agent/EditAgent/' + this.Ids()[0] + '/' + this.masterId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        case this.Member:
            if (this.isSuperEditMember)
                actionName += 'Member/EditMemberBySuper/' + this.Ids()[0] + '/' + this.superId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
            else if (this.isMasterEditMember)
                actionName += 'Member/EditMemberByMaster/' + this.Ids()[0] + '/' + this.masterId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
            else
                actionName += 'Member/EditMember/' + this.Ids()[0] + '/' + this.agentId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        default:
            break;
    }

    if (this.isSuperEditMember) {
        levelText = 'Super ';
    } else if (this.isMasterEditMember) {
        levelText = 'Master ';
    }

    if (!this.IsMultiple) {
        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(actionName), levelText + 'Edit Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1064, 700, false, false, 1, null);
    } else {
        window.IsMultiple = true;
        window.Ids = this.Ids();
        window.Names = this.Names();
        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(actionName), levelText + 'Edit Multiple Casino Setting', 1064, 700, false, false, 1, null);
    }
};

Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm = function (custids, allow, index) {
    Fanex.ExtAcc.BlockUI();

    if (index == 0) {
        this.MultipleDblCommError = '';
    }

    var len = custids.length - 1;
    var current = index;

    $.ajax({
        url: resolveClientUrl('Customer/ConfigDoubleComm'),
        dataType: 'json',
        type: 'post',
        data: {
            custid: parseInt(custids[current]),
            allow: allow,
            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
        }
    })
    .success(function (result) {
        if (result.errorCode && result.errorCode == '1') {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert('Error', result.errorMessage, 400, 150, 'alert');
            Fanex.ExtAcc.Form.Close();
        } else {
            if (!result.isSuccess) {
                Fanex.ExtAcc.Customer.MultipleDblCommError += (result.errorCode == '600' ? ('Account ' + Fanex.ExtAcc.Customer.Names()[current] + ' was disableed<br />') : (result.errorMessage + '<br/>'));
            }

            if (current == len) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.UnBlockUI();

                    if (Fanex.ExtAcc.Customer.MultipleDblCommError != '') {
                        Fanex.ExtAcc.Form.Alert('Error', Fanex.ExtAcc.Customer.MultipleDblCommError.formatErrorMessage(), 300, 150, 'alert');
                    }

                    Fanex.ExtAcc.Customer.CallBackCustomerList();

                }, 2000);
            }

            current++;

            if (current <= len) {
                Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
            }
        }
    })
    .error(function (xhr, status, err) {
        if (typeof console != 'undefined') {
            console.log(err);
        }

        Fanex.ExtAcc.Customer.MultipleDblCommError += err + '<br/>';

        current++;

        if (current <= len) {
            Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
        }

        Fanex.ExtAcc.UnBlockUI();
    });
};

Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts = function (customerIds, productId) {
    var customerIdRow, unsyncedProductsAttribute;
    var customerIdsArray = customerIds.split(",");

    for (var i = 0, max = customerIdsArray.length; i < max; i++) {
        customerIdRow = document.getElementById(customerIdsArray[i].replace(/[\s,]+/g, ""));

        if (customerIdRow) {
            unsyncedProductsAttribute = customerIdRow.attributes.getNamedItem("data-unsyncedproducts")

            if (unsyncedProductsAttribute && Fanex.ExtAcc.Customer.EditMultipleDCS_ItemInString(productId, unsyncedProductsAttribute.value)) {
                return true;
            }
        }
    }

    return false;
};

Fanex.ExtAcc.Customer.EditMultipleDCS_ItemInString = function (item, stringValue) {
    var stringArray = stringValue.split(",");

    for (var i = 0, max = stringArray.length; i < max; i++) {
        if (item === parseInt(stringArray[i])) {
            return true;
        }
    }

    return false;
};

Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField = function (value) {
    var isForcingModeElement = document.getElementById("isForcingMode");

    if (isForcingModeElement) {
        isForcingModeElement.value = value;
    }
};

window.successfulUpdatingCallback = function (isPartialUpdate, isMultipleUpdate) {
    var isForcingModeElement = document.getElementById("isForcingMode");

    if (isForcingModeElement && isForcingModeElement.value === "true") {
        parent.Fanex.ExtAcc.Customer.Search(false);

        isForcingModeElement.value = "false";
        $("#targetForm").attr("action", resolveClientUrl("DCS/ViewCustomerSetting")).submit();
    }
};