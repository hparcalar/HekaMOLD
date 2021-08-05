app.controller('machineSelectionCtrl', function ($scope, $http) {
    $scope.machineList = [];

    $scope.loadMachineList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetMachineList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.machineClick = function (machine) {
        $scope.$emit('machineSelected', machine);
    }

    // ON LOAD EVENTS
    $scope.$on('loadMachineList', function (e, d) {
        $scope.loadMachineList();
    });
});