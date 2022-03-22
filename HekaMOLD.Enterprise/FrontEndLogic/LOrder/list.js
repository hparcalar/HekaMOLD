﻿app.controller('lOrderListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'LOrder/GetItemOrderList', function (data) {

                    });
                },
                key: 'Id',
            },
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            }, onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Sipariş Listesi');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Sipariş Listesi.xlsx');
                    });
                });
                e.cancel = true;
            },
            columnFixing: {
                enabled: true,
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: false,
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true,
            },
            headerFilter: {
                visible: true
            },
            pager: {
                allowedPageSizes: [5, 8, 15, 30],
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true,
            },
            paging: {
                enabled: true,
                pageSize: 8,
                pageIndex: 0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            onRowPrepared: function (e) {
                if (e.rowType != "data")
                    return;

                var deadlineClass = '', item = e.data;
                if (item.OrderStatus == 2) {
                    deadlineClass = 'bg-danger';
                }
                if (item.OrderStatus == 1) {
                    deadlineClass = 'bg-primary';
                }
                //else {
                //    if (item.DateOfNeedStr != null && item.DateOfNeedStr.length > 0) {
                //        var dtDeadline = moment(item.DateOfNeedStr, 'DD.MM.YYYY');
                //        if (moment().diff(dtDeadline, 'days') >= 0)
                //            deadlineClass = 'bg-secondary';
                //        else if (moment().diff(dtDeadline, 'days') > -5)
                //            deadlineClass = 'bg-warning';
                //    }
                //}
                if (item.OrderStatus == 5) {
                    deadlineClass = 'bg-secondary';
                }

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'Id', caption: 'Id', visible: false, sortOrder: "desc", },
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'OrderStatusStr', caption: 'Durum' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'OrderDateWeek', caption: 'Sipariş Hafta', },
                { dataField: 'OrderDateStr', caption: 'Sipariş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DateOfNeedStr', caption: 'Teslim Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'ScheduledUploadDateStr', caption: 'Pl. Yükleme Tarihi ', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomerFirmName', caption: 'Müşteri Firma Adı' },
                { dataField: 'LoadPostCode', caption: 'Yükleme Şehri Posta Kodu' },
                { dataField: 'LoadCityName', caption: 'Yükleme Şehri' },
                { dataField: 'LoadCountryName', caption: 'Yükleme Ülke' },
                { dataField: 'DischangePostCode', caption: 'Boşaltma Şehri Posta Kodu' },
                { dataField: 'DischangeCityName', caption: 'Boşaltma Şehri' },
                { dataField: 'DischangeCountryName', caption: 'Boşaltma Ülke' },
                { dataField: 'EntryCustomsName', caption: 'Çıkış Gümrüğü' },
                { dataField: 'ExitCustomsName', caption: 'Varış Gümrüğü' },
                { dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü' },
                { dataField: 'OrderUploadTypeStr', caption: 'Yükleme Tipi' },
                { dataField: 'OrderUploadPointTypeStr', caption: 'Yükleme Noktası Tipi' },
                { dataField: 'OrderCalculationTypeStr', caption: 'Hesaplama Tipi' },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar' },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık' },
                { dataField: 'OveralVolume', caption: 'Toplam Hacim' },
                { dataField: 'OveralLadametre', caption: 'Toplam Ladametre' },
                { dataField: 'OverallTotal', caption: 'Toplam Tutar' },
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
                { dataField: 'CreatedUserName', caption: 'Siparişi Oluşturan' },
                { dataField: 'Explanation', caption: 'Açıklama' },

                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'LOrder?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ],
            summary: {
                totalItems: [{
                    column: 'OrderNo',
                    summaryType: 'count',
                }]
            },

        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
