app.controller('machineIncidentsCtrl', function ($scope, $http) {
    $scope.modelObject = {
        MachineId: 0,
        StartDate: moment().format('DD.MM.YYYY'),
        EndDate: moment().format('DD.MM.YYYY'),
        DataList: [],
    };

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.groupArray = function (list, key) {
        if (list != null && list.length > 0)
            return groupArrayOfObjects(list, key);

        return [];
    }

    $scope.bindModel = function () {
        try {
            $http.get(HOST_URL + 'Maintenance/GetIncidentsOfMachine?machineId=' + $scope.modelObject.MachineId
                + '&startDate=' + $scope.modelObject.StartDate + '&endDate=' + $scope.modelObject.EndDate, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.DataList = resp.data;

                        $scope.bindTable();
                        $scope.bindChart();
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.bindTable = function () {
        $('#incidentsTable').dxDataGrid({
            dataSource: $scope.modelObject.DataList,
            keyExpr: 'Id',
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            cacheEnabled: false,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 250,
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                {
                    dataField: 'CreatedDateStr',
                    caption: 'Bildirim Tar.', dataType: 'date', format: 'dd.MM.yyyy HH:mm'
                },
                {
                    dataField: 'StartDateStr',
                    caption: 'Başlama Tar.', dataType: 'date', format: 'dd.MM.yyyy HH:mm'
                },
                {
                    dataField: 'EndDateStr',
                    caption: 'Bitiş Tar.', dataType: 'date', format: 'dd.MM.yyyy HH:mm'
                },
                { dataField: 'IncidentCategoryName', caption: 'Arıza Kategorisi' },
                { dataField: 'Description', caption: 'Açıklama' },
                { dataField: 'IncidentStatusStr', caption: 'Durum' },
            ]
        });
    }

    $scope.chartConfig = {
        type: 'line',
        data: {
            labels: [],
            datasets: [
                {
                    label: 'Arıza Grafiği',
                    data: [],
                    borderColor: 'yellow',
                    backgroundColor: 'yellow',
                },
            ]
        },
        options: {
            maintainAspectRatio: false,
            responsive: false,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: false,
                    text: 'Arıza Dağılımı'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    max:10,
                }
            }
        },
    };

    $scope.bindChart = function () {
        const ctx = document.getElementById('incidentsChart').getContext('2d');

        $scope.chartConfig.data.datasets[0].data.splice($scope.chartConfig.data.datasets[0].data.length);

        var loopDate = moment($scope.modelObject.StartDate, 'DD.MM.YYYY');
        var dtEnd = moment($scope.modelObject.EndDate, 'DD.MM.YYYY');

        while (loopDate <= dtEnd) {
            var loopDateStr = loopDate.format('DD.MM.YYYY');
            if (!$scope.chartConfig.data.labels.some(m => m == loopDateStr)) {
                $scope.chartConfig.data.labels.push(loopDateStr)

                if ($scope.modelObject.DataList.some(m => m.CreatedOnlyDateStr == loopDateStr))
                    $scope.chartConfig.data.datasets[0].data.push(
                        $scope.groupArray($scope.modelObject.DataList, 'CreatedOnlyDateStr')[loopDateStr].length);
                else
                    $scope.chartConfig.data.datasets[0].data.push(0);
            }

            loopDate = loopDate.add(1, 'days');
        }

        const lineChart = new Chart(ctx, $scope.chartConfig);
    }

    // ON LOAD EVENTS
    $scope.$on('showIncidents', function (e, d) {
        $scope.modelObject.MachineId = d.MachineId;
        $scope.modelObject.StartDate = d.StartDate;
        $scope.modelObject.EndDate = d.EndDate;

        $scope.bindModel();
    });
});