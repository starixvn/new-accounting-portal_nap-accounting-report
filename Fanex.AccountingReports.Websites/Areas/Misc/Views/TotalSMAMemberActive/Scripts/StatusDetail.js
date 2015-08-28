Namespace.Register('Fanex.AccountingReports.Misc.TotalSMAMemberActive.StatusDetail');
Fanex.AccountingReports.Misc.TotalSMAMemberActive.StatusDetail = function () {
	var self = this;

	var _tables = {
		detailContainer: null,
		status: null
	};
	var _params = {
		reportParams: {
			FromDate: null,
			ToDate: null,
			SiteList: '',
			SiteName: null,
			Status: null,
			RoleId: null,
			PageSize: 50,
			PageIndex: 0
		}
	};
	var _status = {
		NewCreated: 1,
		Suspended: 2,
		Closed: 3
	};

	self.initPage = function () {
		cachedDOMElements();
	};
	self.resetDefaultPageSize = function () {
		_params.reportParams.PageSize = 50;
	};
	var cachedDOMElements = function () {
		_tables.detailContainer = $('#detailContainer');
	};
	self.getReport = function (params) {
		AccReport.disableInput();
		if (params) {
			getReportParams(params);
		}
		$.ajax({
			type: 'POST',
			url: window.rootUrl + 'Misc/TotalSMAMemberActive/GetStatusDetails',
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
	};
	var getReportParams = function (params) {
		_params.reportParams.FromDate = params.FromDate;
		_params.reportParams.ToDate = params.ToDate;
		_params.reportParams.SiteList = params.SiteList;
		_params.reportParams.SiteName = params.SiteName;
		_params.reportParams.Status = params.Status;
		_params.reportParams.RoleId = params.RoleId;
		// reset paging
		_params.reportParams.PageIndex = 0;
		// update status
		_tables.status = params.Status;
	};
	var renderReport = function (rs) {
		clearDetails();

		var totalRow = 0;
		var bodyData = [];
		if (rs && (totalRow = rs.TotalRow) > 0) {
			var report = rs.Report;
			var len = report.length;
			for (var i = 0; i < len; i++) {
				var evenClass = i % 2 === 0 ? 'freegrid-even' : '';
				var group = report[i];
				var groupName = group.GroupHeadline.Name;
				var childsInfo = group.ChildsInfo;

				var childsLen = childsInfo.length;
				for (var j = 0; j < childsLen; j++) {
					var detail = childsInfo[j];
					var row = [{ Name: 'ColDate', Value: detail.DateStr}];
					if (j === 0) {
						row.push({ Name: 'ColGroupHeadline', Value: groupName, MergeRowNumber: childsLen });
					}
					row.push({ Name: 'ColUsername', Value: detail.Username },
							 { Name: 'ColStatus', Value: detail.Status },
							 { Name: 'ColSite', Value: detail.Site },
							 { Name: 'ColCurrency', Value: detail.Currency },
							 { Name: 'ColPosition', Value: detail.Position });
					if (_tables.status === _status.NewCreated) {
						row.push({ Name: 'ColCreatedTime', Value: detail.CreatedTimeStr },
								 { Name: 'ColCreator', Value: detail.Creator });
					}
					bodyData.push({ Columns: row, CssClass: evenClass });
				}
			}
		}

		var reportTitle;
		if (_tables.status === _status.NewCreated) {
			reportTitle = viewResources.NewCreated;
		}
		else if (_tables.status === _status.Suspended) {
			reportTitle = viewResources.Suspended;
		}
		else {
			reportTitle = viewResources.Closed;
		}
		reportTitle += ' ' + viewResources.Details;
		if (_params.reportParams.SiteName) { // will include sitename when sitelist <= 3
			reportTitle += ' - Site(s): ' + _params.reportParams.SiteName;
		}

		var gridColumns = initializeGridColumns()
		var gridOptions = {
			columns: gridColumns,
			showTitle: true,
			titleInfo:
					{
						text: reportTitle,
						showExcelButton: true,
						exportExcelFunction: function () {
							exportReport();
						}
					},
			bodyRows: bodyData,
			showDistinctRow: false,
			gridExpandHeight: 385,
			showPager: true,
			customPager: true,
			pagerOption: {
				itemsPerPage: _params.reportParams.PageSize,
				currentPage: _params.reportParams.PageIndex,
				totalItem: totalRow,
				itemsPerPageArray: [50, 100, 200, 500],
				pagerSelectCallBack: repaging
			}
		};
		_tables.detailContainer.removeClass('display-none');
		$('#reportDetail').fanexGrid(gridOptions);
	};
	var clearDetails = function () {
		_tables.detailContainer.html('');
		_tables.detailContainer.html('<div class="freegrid" id="reportDetail"></div>');
	};
	var initializeGridColumns = function () {
		var gridColumns = [{ HeaderText: viewResources.Date, Width: 120, Name: 'ColDate', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.GroupHeadline, Width: 150, Name: 'ColGroupHeadline', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.Username, Width: 130, Name: 'ColUsername', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.Status, Width: 100, Name: 'ColStatus', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.Site, Width: 118, Name: 'ColSite', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.Currency, Width: 140, Name: 'ColCurrency', HeaderAlign: 'center', CellAlign: 'center' },
						   { HeaderText: viewResources.Position, Width: 120, Name: 'ColPosition', HeaderAlign: 'center', CellAlign: 'center'}];
		if (_tables.status === _status.NewCreated) {
			gridColumns.push({ HeaderText: viewResources.CreatedTime, Width: 200, Name: 'ColCreatedTime', HeaderAlign: 'center', CellAlign: 'center' });
			gridColumns.push({ HeaderText: viewResources.Creator, Width: 120, Name: 'ColCreator', HeaderAlign: 'center', CellAlign: 'center' });
		}

		return gridColumns;
	};
	var repaging = function (page_index, itemsPerPage, jq) {
		_params.reportParams.PageSize = itemsPerPage;
		_params.reportParams.PageIndex = page_index;
		self.getReport();
	};
	var exportReport = function () {
		AccReport.downloadFile(window.rootUrl + 'Misc/TotalSMAMemberActive/ExportStatusDetails', _params.reportParams);
	};
}