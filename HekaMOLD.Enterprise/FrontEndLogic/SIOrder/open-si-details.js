app.controller('openSIListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'SIOrder/GetOpenOrderDetailList', function (data) {

                    });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: false,
            focusedRowEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            showBorders: true,
            filterRow: {
                visible: true,
            },
            headerFilter: {
                visible: true
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
                if (item.OrderStatus == 3 || item.OrderStatus == 2) {
                    deadlineClass = 'bg-secondary';
                }
                else {
                    if (item.DateOfNeedStr != null && item.DateOfNeedStr.length > 0) {
                        var dtDeadline = moment(item.DateOfNeedStr, 'DD.MM.YYYY');
                        if (moment().diff(dtDeadline, 'days') >= 0)
                            deadlineClass = 'bg-danger';
                        else if (moment().diff(dtDeadline, 'days') > -5)
                            deadlineClass = 'bg-warning';
                    }
                }

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'CreatedDateStr', caption: 'Sipariş Tarihi' },
                { dataField: 'DateOfNeedStr', caption: 'Termin Tarihi' },
                { dataField: 'FirmCode', caption: 'Firma Kodu' },
                { dataField: 'FirmName', caption: 'Firma Adı' },
                { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                { dataField: 'Quantity', caption: 'Miktar' },
                { dataField: 'OrderStatusStr', caption: 'Durum' },
                { dataField: 'Explanation', caption: 'Açıklama' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'SIOrder?rid=' + e.row.data.ItemOrderId;
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
