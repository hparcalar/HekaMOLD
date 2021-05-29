app.controller('relatedRequestListCtrl', function ($scope, $http) {
    $scope.relatedRequestList = [];
    $scope.selectedOrderId = 0;

    $scope.loadRelatedRequestList = function () {
        try {
            $http.get(HOST_URL + 'PIOrder/GetRelatedRequestList?orderId=' + $scope.selectedOrderId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.relatedRequestList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.goToRequest = function (requestId) {
        window.location.href = HOST_URL + 'PIRequest?rid=' + requestId;
    }

    // ON LOAD EVENTS
    $scope.$on('loadRelatedRequestList', function (e, d) {
        $scope.selectedOrderId = d;
        $scope.loadRelatedRequestList();
    });
});