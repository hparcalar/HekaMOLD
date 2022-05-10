app.controller('loadListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Load/GetItemLoadList', function (data) {

                    });
                },
                key: 'Id'
            },
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            }, onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Yük Listesi');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Yük Listesi.xlsx');
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
            //allowColumnReordering = true,
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
                if (item.LoadStatusType == 1)
                    deadlineClass = 'bg-danger';
                if (item.LoadStatusType == 2)
                    deadlineClass = 'bg-secondary';
                if (item.LoadStatusType == 13)
                    deadlineClass = 'bg-success';
                if (!item.LoadStatusType == 11 || !item.LoadStatusType == 1 || !item.LoadStatusType == 2)
                    deadlineClass = 'bg-white';
                if (item.LoadStatusType == 14)
                    deadlineClass = 'bg-primary';
                //else {
                //    if (item.DateOfNeedStr != null && item.DateOfNeedStr.length > 0) {
                //        var dtDeadline = moment(item.DateOfNeedStr, 'DD.MM.YYYY');
                //        if (moment().diff(dtDeadline, 'days') >= 0)
                //            deadlineClass = 'bg-secondary';
                //        else if (moment().diff(dtDeadline, 'days') > -5)
                //            deadlineClass = 'bg-warning';
                //    }
                //}
                //if (item.LoadStatusType == 5) {
                //    deadlineClass = 'bg-success';
                //}

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'Id', caption: 'Id', visible: false, sortOrder: "desc", },
                { dataField: 'UserAuthorName', caption: 'Temsilci/Yükü Veren' },
                { dataField: 'Billed', caption: 'Faturalaştırıldı', dataType: 'boolean' },
                { dataField: 'LoadCode', caption: 'Yük Kodu' },
                { dataField: 'VoyageConverted', caption: 'Sef. Dön.', dataType: 'boolean' },
                { dataField: 'LoadStatusTypeStr', caption: 'Durum' },
                { dataField: 'VoyageCode', caption: 'Sefer Kodu' },
                { dataField: 'OrderUploadPointTypeStr', caption: 'Yükleme Noktası Tipi' },
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'LoadWeek', caption: 'Yük Hafta' },
                { dataField: 'BringingToWarehouseDateStr', caption: 'Depoya Geliş Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DeliveryFromCustomerDateStr', caption: 'Müş. Teslim Alınış Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomerFirmName', caption: 'Müşteri' },
                { dataField: 'ManufacturerFirmName', caption: 'İmalatçı Firma' },
                { dataField: 'ReelOwnerFirmName', caption: 'Gerçek Mal Sahibi' },
                { dataField: 'AgentFirmName', caption: 'Komisyoncu Firma' },
                { dataField: 'OrderCalculationTypeStr', caption: 'Hesaplama Tipi' },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar' },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık (KG)' },
                { dataField: 'OveralVolume', caption: 'Toplam Hacim (M3)' },
                { dataField: 'OveralLadametre', caption: 'Toplam Ladametre' },
                { dataField: 'OverallTotal', caption: 'Toplam Tutar' },
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
                { dataField: 'Explanation', caption: 'Açıklama' },
                { dataField: 'ShipperFirmName', caption: 'Gönderici Firma' },
                { dataField: 'BuyerFirmName', caption: 'Alıcı Firma' },
                { dataField: 'ShipperCityName', caption: 'Gön. Şehir' },
                { dataField: 'ShipperCountryName', caption: 'Gön. Ülke' },
                { dataField: 'ShipperFirmExplanation', caption: 'Gön. Adres' },
                { dataField: 'BuyerCityName', caption: 'Al. Şehir' },
                { dataField: 'BuyerCountryName', caption: 'Al. Ülke' },
                { dataField: 'BuyerFirmExplanation', caption: 'Al. Adres' },
                { dataField: 'EntryCustomsName', caption: 'Çıkış Gümrüğü' },
                { dataField: 'ExitCustomsName', caption: 'Varış Gümrüğü' },
                { dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü' },
                { dataField: 'OrderUploadTypeStr', caption: 'Yükleme Tipi' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'OrderDateStr', caption: 'Sipariş Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'ScheduledUploadDateStr', caption: 'Pl. Yükleme Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DateOfNeedStr', caption: 'Teslim Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadingDateStr', caption: 'Yükleme Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadOutDateStr', caption: 'Boşaltma Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadExitDateStr', caption: 'Yük Çıkış Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DischargeDateStr', caption: 'Müşteriye Teslim Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'VehicleTraillerPlate', caption: 'Romörk Plaka', },
                { dataField: 'VehicleTraillerMarkAndVersiyon', caption: 'Romörk Marka/Model', },

                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Load?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ],
            summary: {
                totalItems: [{
                    column: 'LoadCode',
                    summaryType: 'count',
                }]
            },
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
