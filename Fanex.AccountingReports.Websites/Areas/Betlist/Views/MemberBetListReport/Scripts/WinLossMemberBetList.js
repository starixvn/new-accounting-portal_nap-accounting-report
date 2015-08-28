$(document).ready(function () {
	var reportParams = WinLossMemberBetList.GetQueryParams(document.location.search);
	WinLossMemberBetList.GetReport(reportParams);
	//scoreInfoManager.InitPage();
});
var WinLossMemberBetList = {
	gridOpts: {
		tblContentId: 'tblContent',
		manualCopyHeader: true,
		fullWidth: true,
		fullHeight: true,
		showFooter: true,
		gridWidth: 1190, //=totalColumnWidth + totalColumn*4 +totalColumn
		gridHeight: 40,
		numberFreezeCol: 0,
		freezeWidth: 0
	},
	GetReport: function (reportParams) {
		$.ajax({
			type: 'POST',
			url: window.rootUrl + 'BetList/BetList/GetWinLossMemberBetList?' + $.param(reportParams),
			success: function (result) {
				$('#reportContainer').html(result);

				var grid = $('#WinLossMemberBetList');
				grid.initGrid(WinLossMemberBetList.gridOpts);
				grid.removeClass('display-none').gridScroller(WinLossMemberBetList.gridOpts);
			},
			error: function () {
			},
			complete: function () {
			}
		});
	},
	trim: function (str) {
		return str.replace(/(^\s*)|(\s*$)/g, "");
	},
	GetQueryParams: function (qs) {
		qs = qs.split('+').join(' ');
		var params = {}, tokens, re = /[?&]?([^=]+)=([^&]*)/g;

		while (tokens = re.exec(qs)) {
			params[decodeURIComponent(tokens[1])] = decodeURIComponent(tokens[2]);
		}

		return params;
	},
    ExportBetlist: function () {
        var reportParams = WinLossMemberBetList.GetQueryParams(document.location.search);
        AccReport.downloadFile(window.rootUrl + "BetList/BetList/ExportBetlist", $.param(reportParams));
    }
};

function showMP(transid, refno) {
	var grid = $('#WinLossMemberBetList');
	var divMixParlay = $('#mixParlay_' + refno);
	if (divMixParlay) {
	    if (divMixParlay.hasClass('display-none')) {
	        divMixParlay.removeClass('display-none');
		    if (divMixParlay.html() == '') { // get data from server
		    	$.ajax({
		    		type: 'GET',
		    		url: window.rootUrl + 'BetList/BetList/GetMixParlayDetail?refno=' + refno,
		    		beforeSend: function () {
		    			AccReport.disableInput();
		    		},
		    		success: function (rs) {
		    			if (rs) {
		    				divMixParlay.html(rs);

		    				WinLossMemberBetList.gridOpts.isResetScroller = false;
		    				grid.gridScroller(WinLossMemberBetList.gridOpts);
		    			}
		    			AccReport.enableInput();
		    		},
		    		error: function (event, jqXHR, ajaxSettings) {
		    			AccReport.enableInput();
		    		},
		    		complete: function () { }
		    	});
		    }
		    else {
		        grid.gridScroller(WinLossMemberBetList.gridOpts);
            }
		}
		else {
			divMixParlay.addClass('display-none');
			grid.gridScroller(WinLossMemberBetList.gridOpts);
		}
		
	}
}

function showCombMP(transId, a, refNo) {
	showMP(transId, refNo);
}

function showCombMPDetail(refno, date, tranid, betid, matchid) {
	/*var username = $(".combinationLink span:last").html();
	showDetailsCMP(tranid, "System Parlay - " + username, parseFloat(refno), parseInt(betid), _custid)*/
	var divMixParlay = $('#mixParlay_' + refno);
	if (divMixParlay) { // get custId
		var custId = divMixParlay.attr('data-custid');
		$.ajax({
			type: 'GET',
			url: window.rootUrl + 'BetList/BetList/GetCombinationMP?refNo='
			+ refno + '&transId=' + tranid + '&betId=' + betid + '&custId=' + custId,
			beforeSend: function () {
				AccReport.disableInput();
			},
			success: function (rs) {
				if (rs) {
					$('#dialogContent').empty().append(rs);
					var dlgContainer = $('#dialogContainer');
					dlgContainer.dialog({
						title: "System Parlay",
						autoOpen: false,
						height: "auto",
						width: "auto",
						modal: false,
						resizable: false,
						close: function (event, ui) {

						}
					});
					dlgContainer.dialog("open");
				}
			},
			error: function (event, jqXHR, ajaxSettings) {
			},
			complete: function () {
				AccReport.enableInput();
			}
		});
	}
}

function ViewDNS(ip) {
	url = 'http://anonym.to/?http://legacytools.dnsstuff.com/tools/ipall.ch?tool_id=67&token=&toolhandler_redirect=0&ip=' + ip + '&j=1';
	window.open(url, 'DNS', ' scrollbars=yes, toolbar=yes, menubar=yes,location=yes,status=yes,resizable=yes');
}

function updateHeader() {
	var reportParams = scoreInfoManager.Params.reportParams;
	$('#scoreInformationPopup .reportHeader').html(reportParams.SportTypeName + ' - ' + reportParams.LeagueName);
}