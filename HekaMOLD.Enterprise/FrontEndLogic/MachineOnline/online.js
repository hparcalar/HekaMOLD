app.controller('machineOnlineCtrl', function ($scope, $http) {
    $scope.machineList = [];
    $scope.filterModel = { startDate: moment().format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Machine/GetMachineStats?t1=' + $scope.filterModel.startDate + '&t2='
            + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.getFixed = function (arg, point) {
        try {
            return arg.toFixed(point);
        } catch (e) {

        }

        return arg;
    }

    $scope.showIncidents = function (machineId) {
        $scope.$broadcast('showIncidents',
            {
                MachineId: machineId,
                StartDate: $scope.filterModel.startDate,
                EndDate: $scope.filterModel.endDate
            });

        $('#dial-incidents').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.8,
            height: window.innerHeight * 0.8,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.showPostures = function (machineId) {
        $scope.$broadcast('showPostures',
            {
                MachineId: machineId,
                StartDate: $scope.filterModel.startDate,
                EndDate: $scope.filterModel.endDate
            });

        $('#dial-postures').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.8,
            height: window.innerHeight * 0.8,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // ON LOAD EVENTS
    $scope.bindModel(PRM_ID);
});