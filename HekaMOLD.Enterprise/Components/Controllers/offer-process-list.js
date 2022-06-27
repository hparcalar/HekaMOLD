app.controller('offerProcessCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.offerProcessList = [];

    $scope.loadProcessList = function () {
        $('#processList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.offerProcessList;
                },
                update: function (key, values) {
                    var obj = $scope.offerProcessList.find(d => d.RouteItemId == key);
                    if (obj != null) {

                        if (typeof values.UnitPrice != 'undefined' && values.UnitPrice != null)
                            obj.UnitPrice = values.UnitPrice;
                    }
                },
                key: 'RouteItemId'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
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
                { dataField: 'ProcessCode', caption: 'Proses Kodu', allowEditing: false },
                { dataField: 'ProcessName', caption: 'Proses Adı', allowEditing: false },
                {
                    dataField: 'UnitPrice', caption: 'Fiyat', allowEditing: true,
                    dataType: 'number', format: { type: "fixedPoint", precision: 2 }
                },
            ]
        });
    }

    $scope.transferSelections = function () {
        $scope.$emit('transferProcessList', $scope.offerProcessList);
    }

    // ON LOAD EVENTS
    $scope.$on('loadProcessList', function (e, d) {
        $scope.offerProcessList = d;
        $scope.loadProcessList();
    });
});