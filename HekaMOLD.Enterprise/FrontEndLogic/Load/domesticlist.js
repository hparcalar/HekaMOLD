app.controller('loadDomesticListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Load/GetItemLoadDomesticList', function (data) {

                    });
                },
                key: 'Id'
            },
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            columnChooser: {
                enabled: true,
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
                if (item.LoadStatusType == 2) {
                    deadlineClass = 'bg-danger';
                }
                if (item.LoadStatusType == 1) {
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
                if (item.LoadStatusType == 5) {
                    deadlineClass = 'bg-success';
                }

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'LoadCode', caption: 'Yük Kodu' },
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'OrderDateStr', caption: 'Sipariş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DateOfNeedStr', caption: 'Teslim Tarihi ', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadOutDateStr', caption: 'Boşaltma Tarihi ', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadingDateStr', caption: 'Yükleme Tarihi ', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomerFirmName', caption: 'Müşteri' },
                { dataField: 'LoadStatusTypeStr', caption: 'Durum' },
                { dataField: 'ShipperFirmName', caption: 'Gönderici Firma' },
                { dataField: 'BuyerFirmName', caption: 'Alıcı Firma' },
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
                { dataField: 'Explanation', caption: 'Açıklama' },
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
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
