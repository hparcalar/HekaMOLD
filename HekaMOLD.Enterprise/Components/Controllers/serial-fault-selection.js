app.controller('serialFaultSelectionCtrl', function ($scope, $http) {
    $scope.faultTypeList = [];

    $scope.loadFaultTypeList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetSerialFaultTypeList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.faultTypeList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.faultClick = function (fault) {
        $scope.$emit('faultTypeSelected', fault);
    }

    // ON LOAD EVENTS
    $scope.$on('loadFaultTypeList', function (e, d) {
        $scope.loadFaultTypeList();
    });
});