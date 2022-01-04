app.controller('openWoListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.workOrderDetailList = [];

    $scope.loadOpenWoList = function () {
        $scope.workOrderDetailList = [];

        $('#openWoList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.workOrderDetailList.length == 0)
                        $scope.workOrderDetailList = $.getJSON(HOST_URL + 'SerialWinding/GetOpenWorkOrderList', function (data) {
                            data.forEach(d => {
                                if (d.CreatedDate != null)
                                    d.CreatedDate = moment(parseInt(d.CreatedDate.toString().substr(6, d.CreatedDate.toString().length - 8)));
                                if (d.WorkOrderDate != null)
                                    d.WorkOrderDate = moment(parseInt(d.WorkOrderDate.toString().substr(6, d.WorkOrderDate.toString().length - 8)));
                                d.IsChecked = false;
                            }
                            );
                        });

                    return $scope.workOrderDetailList;
                },
                update: function (key, values) {
                    var obj = $scope.workOrderDetailList.responseJSON.find(d => d.Id == key);
                    if (obj != null) {
                        obj.IsChecked = values.IsChecked;
                    }
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
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
                visible: false
            },
            editing: {
                allowUpdating: true,
                allowDeleting: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'WorkOrderNo', caption: 'İş Emri No', allowEditing: false },
                { dataField: 'WorkOrderDate', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'FirmName', caption: 'Firma', allowEditing: false },
                { dataField: 'ProductName', caption: 'Stok Adı', allowEditing: false },
                /*{ dataField: 'UnitCode', caption: 'Birim', allowEditing: false },*/
                { dataField: 'Quantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'IsChecked', caption: 'Seç' }
            ]
        });
    }

    $scope.transferSelections = function () {
        if ($scope.workOrderDetailList.responseJSON.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir iş emri seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedData = $scope.workOrderDetailList.responseJSON.filter(d => d.IsChecked == true);

        var selectedDetails = selectedData;
        $scope.$emit('transferWorkOrderDetails', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadOpenWoList', function (e, d) {
        $scope.loadOpenWoList();
    });
});