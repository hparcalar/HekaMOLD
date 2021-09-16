app.controller('machineOnlineCtrl', function ($scope, $http) {
    $scope.machineList = [];
    $scope.filterModel = { startDate: moment().subtract('months', 1).format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };

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

    // ON LOAD EVENTS
    
    $scope.bindModel(PRM_ID);
});