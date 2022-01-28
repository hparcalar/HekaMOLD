app.controller('lOrderListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'LOrder/GetItemOrderList', function (data) {

                    });
                },
                key: 'Id'
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
                if (item.OrderStatus == 3 || item.OrderStatus == 2 ) {
                    deadlineClass = 'bg-danger';
                }
                else {
                    if (item.DateOfNeedStr != null && item.DateOfNeedStr.length > 0) {
                        var dtDeadline = moment(item.DateOfNeedStr, 'DD.MM.YYYY');
                        if (moment().diff(dtDeadline, 'days') >= 0)
                            deadlineClass = 'bg-secondary';
                        else if (moment().diff(dtDeadline, 'days') > -5)
                            deadlineClass = 'bg-warning';
                    }
                }
                if (item.OrderStatus == 1) {
                    deadlineClass = 'bg-success';
                }

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'CreatedDateStr', caption: 'Sipariş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DateOfNeedStr', caption: 'Teslim Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomerFirmCode', caption: 'Müşteri Firma Kodu' },
                { dataField: 'CustomerFirmName', caption: 'Müşteri Firma Adı' },
                { dataField: 'OrderStatusStr', caption: 'Durum' },
                { dataField: 'LoadCityName', caption: 'Yükleme Şehri' },
                { dataField: 'LoadCountryName', caption: 'Yükleme Ülke' },
                { dataField: 'DischangeCityName', caption: 'Boşaltma Şehri' },
                { dataField: 'DischangeCountryName', caption: 'Boşaltma Ülke' },
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
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
