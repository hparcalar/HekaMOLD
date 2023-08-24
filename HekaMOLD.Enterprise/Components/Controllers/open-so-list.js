app.controller('openSoListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.orderDetailList = [];

    $scope.loadOpenSoList = function () {
        $scope.orderDetailList = [];

        $http.get(HOST_URL + 'SIOrder/GetOpenOrderDetails', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    resp.data.forEach(d => {
                        if (d.CreatedDate != null)
                            d.CreatedDate = moment(parseInt(d.CreatedDate.toString().substr(6, d.CreatedDate.toString().length - 8)));
                        if (d.OrderDate != null)
                            d.OrderDate = moment(parseInt(d.OrderDate.toString().substr(6, d.OrderDate.toString().length - 8)));
                        d.IsChecked = false;
                    }
                    );

                    $scope.orderDetailList = resp.data;

                    $('#openSoList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.orderDetailList;
                            },
                            update: function (key, values) {
                                var obj = $scope.orderDetailList.find(d => d.Id == key);
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
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        showBorders: true,
                        filterRow: {
                            visible: true
                        },
                        headerFilter: {
                            visible: true
                        },
                        paging: {
                            enabled: false,
                            pageSize: 13,
                            pageIndex: 0
                        },
                        scrolling: {
                            mode: "virtual",
                            columnRenderingMode: "virtual"
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
                            { dataField: 'OrderNo', caption: 'Sipariş No', allowEditing: false },
                            { dataField: 'DocumentNo', caption: 'Belge No', allowEditing: false },
                            { dataField: 'OrderDate', caption: 'Sipariş Tarihi', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                            { dataField: 'FirmName', caption: 'Firma', allowEditing: false },
                            { dataField: 'ItemName', caption: 'Stok Adı', allowEditing: false },
                            { dataField: 'Quantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                            { dataField: 'IsChecked', caption: 'Seç' }
                        ]
                    });
                }
            }).catch(function (err) { });

       
    }

    $scope.transferSelections = function () {
        if ($scope.orderDetailList.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir veya daha fazla talep seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedData = $scope.orderDetailList.filter(d => d.IsChecked == true);

        if (groupArrayOfObjects(selectedData, 'FirmId').length > 1) {
            toastr.warning('Yalnızca tek bir firmanın siparişlerini seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedDetails = selectedData;
        $scope.$emit('transferOrderDetails', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadOpenSoList', function (e, d) {
        $scope.loadOpenSoList();
    });
});