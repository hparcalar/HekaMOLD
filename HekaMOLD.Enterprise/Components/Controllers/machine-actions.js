app.controller('machineActionsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.actionMachineId = 0;
    $scope.actionDate = '';

    $scope.actionList = [];

    $scope.loadActionList = function () {
        $scope.actionList = [];

        $('#machineActions').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.actionList.length == 0)
                        $scope.actionList = $.getJSON(HOST_URL + 'Machine/GetMachineActions?machineId=' + $scope.actionMachineId +
                            '&filterDate=' + $scope.actionDate, function (data) {
                            data.forEach(d => {
                               
                            }
                            );
                        });

                    return $scope.actionList;
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
                enabled: false,
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
                { dataField: 'StartDateStr', caption: 'Giriş Zamanı', dataType: 'date', format: 'dd.MM.yyyy HH:mm', allowEditing: false },
                { dataField: 'EndDateStr', caption: 'Çıkış Zamanı', dataType: 'date', format: 'dd.MM.yyyy HH:mm', allowEditing: false },
                { dataField: 'UserName', caption: 'Operatör', allowEditing: false },
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.$on('loadActionList', function (e, d) {
        $scope.actionMachineId = d.machineId;
        $scope.actionDate = d.actionDate;

        $scope.loadActionList();
    });
});