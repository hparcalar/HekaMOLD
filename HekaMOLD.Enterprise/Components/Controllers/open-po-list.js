app.controller('openPoListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.orderDetailList = [];

    $scope.loadOpenPoList = function () {
        $scope.orderDetailList = [];

        $('#openPoList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.orderDetailList.length == 0)
                        $scope.orderDetailList = $.getJSON(HOST_URL + 'LOrder/GetApprovedItemOrderList', function (data) {
                            data.forEach(d => {
                                d.IsChecked = false;
                            }
                            );
                        });

                    return $scope.orderDetailList;
                },
                update: function (key, values) {
                    var obj = $scope.orderDetailList.responseJSON.find(d => d.Id == key);
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
                { dataField: 'Id', caption: 'Id', visible: false, sortOrder: "desc", },
                { dataField: 'OrderNo', caption: 'Sipariş No' },
                { dataField: 'OrderStatusStr', caption: 'Durum' },
                { dataField: 'OrderDateStr', caption: 'Sipariş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomerFirmName', caption: 'Müşteri Firma Adı' },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar' },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık' },
                { dataField: 'OveralVolume', caption: 'Toplam Hacim' },
                { dataField: 'OveralLadametre', caption: 'Toplam Ladametre' },
                { dataField: 'Explanation', caption: 'Açıklama' },
                { dataField: 'IsChecked', caption: 'Seç' }
            ]
        });
    }

    $scope.transferSelections = function () {
        if ($scope.orderDetailList.responseJSON.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir veya daha fazla talep seçmelisiniz.','Uyarı');
            return;
        }

        var selectedData = $scope.orderDetailList.responseJSON.filter(d => d.IsChecked == true);

        if (groupArrayOfObjects(selectedData, 'FirmId').length > 1) {
            toastr.warning('Yalnızca tek bir firmanın siparişlerini seçmelisiniz.','Uyarı');
            return;
        }

        var selectedDetails = selectedData;
        $scope.$emit('transferOrderDetails', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadOpenPoList', function (e, d) {
        $scope.loadOpenPoList();
    });
});