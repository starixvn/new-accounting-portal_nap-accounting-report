$(document).ready(function () {
	reportManager.initPage();
});

Namespace.Register('Fanex.AccountingReports.Misc.TotalSMAMemberActive.ReportManager');
Fanex.AccountingReports.Misc.TotalSMAMemberActive.ReportManager = function () {
	var self = this;
	var detail = new Fanex.AccountingReports.Misc.TotalSMAMemberActive.StatusDetail();

	var _tables = {
		reportContainer: null,
		reportSummary: null,
		reportActiveContainer: null,
		reportActiveDetail: null,
		detailContainer: null,
		reportDetail: null,
		datepickerplugin: null,
		btnGenerate: null
	};
	var _params = {
		reportParams: {
			FromDate: null,
			ToDate: null,
			SiteList: ''
		}
	};
	var _status = {
		Active: 0,
		NewCreated: 1,
		Suspended: 2,
		Closed: 3
	};
	var _role = {
		All: 0,
		Agent: 2,
		Master: 3,
		Super: 4
	};

	self.initPage = function () {
		cachedDOMElements();
		renderDatePicker();
		renderFilterNote();
		bindingEvent();
		// Details
		detail.initPage();
		// get sites
		getSites();
	};
	var cachedDOMElements = function () {
		_tables.reportContainer = $('#reportContainer');
		_tables.reportSummary = $('#reportSummary');
		_tables.reportActiveContainer = $('#reportActiveContainer');
		_tables.reportActiveDetail = $('#reportActiveDetail');
		_tables.detailContainer = $('#detailContainer');
		_tables.reportDetail = $('#reportDetail');
		_tables.datepickerplugin = $('#datepickerplugin');
		_tables.btnGenerate = $('#btnGenerate');

		var screenWidth = $(window).width() - 20;
		$('#reportActiveDetail-grid-title').css({ width: screenWidth - 2 });
		_tables.reportActiveDetail.css({ width: screenWidth });
	};
	var renderDatePicker = function () {
		$('#datepickerplugin').nexDatePicker({
			nexDatePickerType: 'quickchoose',
			datePickerFormat: {
				changeMonth: true,
				changeYear: true,
				dateFormat: FormatConstant.M_DD_YY_STRING
			},
			dropDownList:
			{
				Id: "dropDownListId",
				Label: "Date ",
				LabelCss: "lbl-field col_2",
				OptionCss: "edit-field col_7"
			},
			firstDatePicker: {
				Id: 'firstDatePickerId1',
				Label: viewResources.From,
				InitDate: today,
				LabelCss: 'lbl-field col_2',
				TextboxCss: 'edit-field col_5'
			},
			secondDatePicker: {
				Id: 'secondDatePickerId1',
				Label: viewResources.To,
				InitDate: today,
				LabelCss: 'lbl-field col_2',
				TextboxCss: 'edit-field col_6'
			},
			selectedItem: 0,
			maxDate: new Date(today),
			minDate: new Date(minDate)
		});
	};
	var renderFilterNote = function () {
		var filterNote = $('#notemessage');
		filterNote.attr('title', viewResources.NoteMessage);
		filterNote.tipsy({
			html: true,
			opacity: 0.9,
			gravity: 'ne'
		});
	};
	var bindingEvent = function () {
		_tables.btnGenerate.unbind('click');
		_tables.btnGenerate.click(function () {
			detail.resetDefaultPageSize();
			getReport();
		});
	};
	var bindingHeaderEvent = function () {
		$('#newCreatedHeader').click(function () {
			self.ViewDetail(_status.NewCreated, _role.All);
		});
		$('#suspendedHeader').click(function () {
			self.ViewDetail(_status.Suspended, _role.All);
		});
		$('#closedHeader').click(function () {
			self.ViewDetail(_status.Closed, _role.All);
		});
	};
	var getReport = function () {
		if (getParams()) {
			AccReport.disableInput();
			$.ajax({
				type: 'POST',
				url: window.rootUrl + 'Misc/TotalSMAMemberActive/GetReport',
				contentType: 'application/json; charset=utf-8',
				dataType: 'json',
				data: JSON.stringify(_params.reportParams),
				beforeSend: function () { },
				success: function (rs) {
					renderReport(rs);
				},
				error: function (jqXhr, event, ajaxSettings) {
					if (jqXhr.status && jqXhr.status == 500) {
						var r = jQuery.parseJSON(jqXhr.responseText);
						AccReport.popUp.show(r.message);
					}
					renderReport(null);
				},
				complete: function () {
					AccReport.enableInput();
				}
			});
		}
	};
	var getParams = function () {
		var dllSite = $('#ddlSite');
		_params.reportParams.SiteList = dllSite.multiSelect('getCheckedItems').toString();
		var dateRange = _tables.datepickerplugin.nexDatePicker('getDateRange');
		_params.reportParams.FromDate = dateRange.FromDate;
		_params.reportParams.ToDate = dateRange.ToDate;

		return checkValidParams();
	};
	var checkValidParams = function () {
		AccReport.popUp.removeHighLight();
		if (_params.reportParams.SiteList.length === 0) {
			AccReport.popUp.show(viewResources.NoSiteSelected, 'ddlSite');
			return false;
		}

		return true;
	};
	var renderReport = function (report) {
		// hide detail
		_tables.reportActiveContainer.removeClass('display-none');
		_tables.reportActiveContainer.addClass('display-none');
		_tables.detailContainer.removeClass('display-none');
		_tables.detailContainer.addClass('display-none');

		var bodyData = [];
		var len = report ? report.length : 0;
		if (len) {
			var index = 0;
			// New Created
			var newCreated = report[index];
			var newCreatedValue = renderSMA(newCreated, _status.NewCreated);
			index++;
			// Active
			var active = report[index];
			index++;
			// Suspended
			var suspended = report[index];
			var suspendedValue = renderSMA(suspended, _status.Suspended);
			index++;
			// Closed
			var closed = report[index];
			var closedValue = renderSMA(closed, _status.Closed);
			index++;

			bodyData.push({ Columns: [
				{ Name: 'ColNewCreated', Value: newCreatedValue },
				{ Name: 'ColActive', Value: renderSMA(active) },
				{ Name: 'ColSuspended', Value: suspendedValue },
				{ Name: 'ColClosed', Value: closedValue}]
			});

			// active detail-data
			renderActiveReport(report, index);
		}
		// summary-grid
		var newCreatedHeader = '<div id="newCreatedHeader">' + viewResources.NewCreated + '</div>';
		var suspendedHeader = '<div id="suspendedHeader">' + viewResources.Suspended + '</div>';
		var closedHeader = '<div id="closedHeader">' + viewResources.Closed + '</div>';
		var gridOptions = {
			columns: [{ HeaderText: newCreatedHeader, Width: 150, Name: 'ColNewCreated', HeaderAlign: 'center', CellAlign: 'center', ThClass: 'smagrid-header link' },
                      { HeaderText: viewResources.Active, Width: 150, Name: 'ColActive', HeaderAlign: 'center', CellAlign: 'center' },
                      { HeaderText: suspendedHeader, Width: 150, Name: 'ColSuspended', HeaderAlign: 'center', CellAlign: 'center', ThClass: 'smagrid-header link' },
					  { HeaderText: closedHeader, Width: 150, Name: 'ColClosed', HeaderAlign: 'center', CellAlign: 'center', ThClass: 'smagrid-header link'}],
			showTitle: true,
			titleInfo:
			{
				text: viewResources.reportTitle.replace('{0}', _params.reportParams.FromDate).replace('{1}', _params.reportParams.ToDate),
				showExcelButton: true,
				exportExcelFunction: function () {
					exportReport();
				}
			},
			bodyRows: bodyData,
			showDistinctRow: false,
			showHighlightRow: false
		};
		_tables.reportContainer.removeClass('display-none');
		_tables.reportSummary.fanexGrid(gridOptions);
		bindingHeaderEvent();
	};
	var renderActiveReport = function (report, index) {
		var trSiteName = $('#trSiteName');
		trSiteName.html('');
		var trSMA = $('#trSMA');
		trSMA.html('');

		var sma;
		var len = report.length;
		for (var i = index; i < len; i++) {
			sma = report[i];
			trSiteName.append('<th>' + sma.Key.Name + '</th>');
			trSMA.append('<td>' + renderSMA(sma, _status.Active) + '</td>')
		}
		_tables.reportActiveContainer.removeClass('display-none');
	};
	var renderSMA = function (sma, status) {
		if (status && status != _status.Active) {
			var superLink = sma.Super === 0
				? sma.Super.toString()
				: '<span class="link" onclick="reportManager.ViewDetail(' + status + ',' + _role.Super + ');">' + sma.Super + '</span>';
			var masterLink = sma.Master === 0
				? sma.Master.toString()
				: '<span class="link" onclick="reportManager.ViewDetail(' + status + ',' + _role.Master + ');">' + sma.Master + '</span>';
			var agentLink = sma.Agent === 0
				? sma.Agent.toString()
				: '<span class="link" onclick="reportManager.ViewDetail(' + status + ',' + _role.Agent + ');">' + sma.Agent + '</span>';
			return superLink + '/' + masterLink + '/' + agentLink;
		}
		else {
			return sma.Super + '/' + sma.Master + '/' + sma.Agent;
		}
	};
	var getSites = function () {
		AccReport.disableInput();
		$.ajax({
			type: 'POST',
			url: window.rootUrl + 'Misc/TotalSMAMemberActive/GetSites',
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: null,
			success: function (rs) {
				var sites = [];
				if (rs && rs.length > 0) {
					$.each(rs, function (key, val) {
						sites.push({
							Id: val.Id,
							Value: val.Id,
							Text: val.Name,
							Checked: true,
							Enable: true,
							CssClass: '',
							Items: []
						});
					});
				}

				renderSiteDropdown(sites);
			},
			error: function (jqXHR, event, ajaxSettings) {
				renderSiteDropdown(null);
			},
			complete: function () {
				AccReport.enableInput();
			}
		});
	};
	var renderSiteDropdown = function (sites) {
		var noItemText = '';
		if (!sites || sites.length == 0) {
			noItemText = 'No sites found.';
		}

		$('#ddlSite').multiSelect(
		{
			minWidth: 160,
			maxHeight: 450,
			shadow: true,
			showHeader: true,
			isCheckAll: true,
			showFooter: false,
			checkAllText: 'All Sites',
			isUsedCustomCheckAllText: true,
			customCheckAllText: 'All Sites',
			noItemText: noItemText,
			listData: sites
		});
	};
	var exportReport = function () {
		AccReport.downloadFile(window.rootUrl + 'Misc/TotalSMAMemberActive/ExportReport', _params.reportParams);
	};
	self.ViewDetail = function (status, roleId) {
		var sitename = null;
		var siteArray = _params.reportParams.SiteList.split(',');
		if (siteArray != null && siteArray.length <= 3) { // will include sitename when sitelist <= 3
			sitename = $('#ddlSite').multiSelect('getCheckedItemsByText').toString();
		}
		detail.getReport({
			FromDate: _params.reportParams.FromDate,
			ToDate: _params.reportParams.ToDate,
			SiteList: _params.reportParams.SiteList,
			Status: status,
			RoleId: roleId,
			SiteName: sitename
		});
	};
};

var reportManager = new Fanex.AccountingReports.Misc.TotalSMAMemberActive.ReportManager();