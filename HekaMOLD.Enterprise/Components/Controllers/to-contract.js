app.controller('toContractCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Quantity: 0,
        DeliveryDate: moment().format('DD.MM.YYYY'),
        DocumentNo: '',
        ReceiptDetailId: null,
    };

    $scope.selectedDetail = null;
    $scope.receiptDetailList = [];

    $scope.saveDelivery = function () {
        if ($scope.selectedDetail == null) {
            toastr.error('Bir irsaliye kalemi seçmelisiniz.');
            return;
        }

        $scope.modelObject.ReceiptDetailId = $scope.selectedDetail.Id;
        $scope.$emit('endToContract', $scope.modelObject);
    }

    $scope.bindReceiptDetails = function () {
        $('#receiptDetails').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.receiptDetailList.length == 0)
                        $scope.receiptDetailList = $.getJSON(HOST_URL + 'ContractWorks/GetWarehouseEntries', function (data) {
                            data.forEach(d => {
                                if (d.CreatedDate != null)
                                    d.CreatedDate = moment(parseInt(d.CreatedDate.toString().substr(6, d.CreatedDate.toString().length - 8)));
                                d.IsChecked = false;
                            }
                            );
                        });

                    return $scope.receiptDetailList;
                },
                key: 'Id'
            },
            onFocusedRowChanged(e) {
                $scope.selectedDetail = e.row.data;
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
                allowUpdating: false,
                allowDeleting: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'ReceiptNo', caption: 'İrsaliye No', allowEditing: false },
                { dataField: 'ReceiptDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'FirmName', caption: 'Firma', allowEditing: false },
                { dataField: 'ItemName', caption: 'Stok Adı', allowEditing: false },
                { dataField: 'WarehouseName', caption: 'Depo', allowEditing: false },
                { dataField: 'Quantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.$on('loadToContract', function (e, d) {
        $scope.bindReceiptDetails();
    });
});