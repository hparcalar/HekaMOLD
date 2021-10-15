app.controller('deliveryPlanListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.planList = [];
    $scope.cursor = 0;
    $scope.planDate = moment().format('DD.MM.YYYY');

    $scope.goPrev = function () {
        $scope.cursor--;
        $scope.planDate = moment().add('days', $scope.cursor).format('DD.MM.YYYY');
        $scope.loadPlanList();
    }

    $scope.goNext = function () {
        $scope.cursor++;
        $scope.planDate = moment().add('days', $scope.cursor).format('DD.MM.YYYY');
        $scope.loadPlanList();
    }

    $scope.loadPlanList = function () {
        $scope.planList = [];

        $('#deliveryPlanList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.planList.length == 0)
                        $scope.planList = $.getJSON(HOST_URL + 'Mobile/GetDeliveryPlanList?cursor=' +
                            $scope.cursor, function (data) {
                            if (data && data.length) {
                                data.forEach(d => {
                                    d.IsChecked = false;
                                }
                                );
                            }

                            $scope.planList = data;
                            return $scope.planList;
                        });
                    
                    return $scope.planList;
                },
                update: function (key, values) {
                    var obj = $scope.planList.find(d => d.Id == key);
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
                { dataField: 'OrderNo', caption: 'Sipariş No', allowEditing: false },
                { dataField: 'FirmName', caption: 'Firma', allowEditing: false },
                { dataField: 'ProductName', caption: 'Ürün Adı', allowEditing: false },
                { dataField: 'Quantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'IsChecked', caption: 'Seç' }
            ]
        });
    }

    $scope.planDateChanged = function () {
        $scope.cursor = moment().diff(moment($scope.planDate, 'DD.MM.YYYY'), 'days');
        $scope.loadPlanList();
    }

    $scope.transferSelections = function () {
        if ($scope.planList.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir veya daha plan seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedData = $scope.planList.filter(d => d.IsChecked == true);

        if (Object.keys(groupArrayOfObjects(selectedData, 'FirmId')).length > 1) {
            toastr.warning('Yalnızca tek bir firmanın sevkiyat planını seçebilirsiniz.', 'Uyarı');
            return;
        }

        var selectedDetails = selectedData;
        $scope.$emit('transferDeliveryPlans', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadDeliveryPlanList', function (e, d) {
        $scope.loadPlanList();

        $('.datepicker').datepicker({ dateFormat: 'dd.mm.yy' });
    });
});