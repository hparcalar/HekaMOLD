app.controller('prodHistoryListCtrl', function ($scope, $http) {
    $scope.prodList = [];
    $scope.selectedHistoryId = 0;
    $scope.historyMachineId = 0;

    $scope.loadProdList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetHistoryWorkOrderListOnMachine?machineId='
                + $scope.historyMachineId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.prodList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectHistory = function (detailId) {
        $scope.selectedHistoryId = detailId;
        $scope.$emit('prodSelected', $scope.selectedHistoryId);
    }

    // ON LOAD EVENTS
    $scope.$on('loadProdHistory', function (e, d) {
        $scope.selectedHistoryId = 0;
        $scope.historyMachineId = d;
        $scope.loadProdList();
    });
});