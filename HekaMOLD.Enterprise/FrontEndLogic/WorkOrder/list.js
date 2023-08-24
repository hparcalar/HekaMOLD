app.controller('workOrderListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.filterModel = {
        startDate: moment().add(-1, 'M').format('DD.MM.YYYY'),
        endDate: moment().format('DD.MM.YYYY'),
    };

    $scope.dataList = [];

    $scope.loadData = function () {

        $http.get(HOST_URL + 'WorkOrder/GetWorkOrderDetailList?dt1=' + $scope.filterModel.startDate +
            '&dt2=' + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.dataList = resp.data.map((d) => {
                        return {
                            ...d,
                            WorkOrderDate: new Date(parseInt(d.WorkOrderDate.substr(6, 13)))
                        };

                    });

                    $scope.loadReport();
                }
            }).catch(function (err) { });
    }

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.dataList;
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                enabled: true,
                pageSize: 13,
                pageIndex: 0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'WorkOrderDate', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'WorkOrderNo', caption: 'İş Emri No' },
                { dataField: 'FirmName', caption: 'Müşteri' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                /*{ dataField: 'DyeCode', caption: 'Renk Kodu' },*/
                { dataField: 'MachineName', caption: 'Makine' },
                { dataField: 'SaleOrderDeadline', caption: 'Sipariş Termin' },
                { dataField: 'Quantity', caption: 'Miktar' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'WorkOrder?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.loadData();
});
