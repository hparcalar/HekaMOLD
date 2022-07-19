app.controller('machineOnlineCtrl', function ($scope, $http, $interval) {
    $scope.machineList = [];
    $scope.currentShift = {Id:0};
    $scope.filterModel = { startDate: moment().format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };
    $scope.isModelBinding = false;

    $scope.bindModel = function () {
        if ($scope.isModelBinding)
            return;

        $scope.isModelBinding = true;

        $http.get(HOST_URL + 'Machine/GetMachineStats?t1=' + $scope.filterModel.startDate + '&t2='
            + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data.Data;
                    $scope.currentShift = resp.data.Shift;

                    $scope.isModelBinding = false;
                }
            }).catch(function (err) {
                $scope.isModelBinding = false;
            });
    }

    $scope.showMachineActions = function (item) {
        if (item && item.Id > 0) {
            $scope.$broadcast('loadActionList',
                {
                    machineId: item.Id,
                    actionDate: $scope.filterModel.startDate,
                });

            $('#dial-machine-actions').dialog({
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
    $scope.bindModel();

    //$interval($scope.bindModel, 120000);

    //setTimeout(function () { window.location.reload(); }, 1000 * 60 * 10);
});