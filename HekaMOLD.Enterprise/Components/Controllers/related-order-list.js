app.controller('relatedOrderListCtrl', function ($scope, $http) {
    $scope.relatedOrderList = [];
    $scope.selectedReceiptId = 0;

    $scope.loadRelatedOrderList = function () {
        try {
            $http.get(HOST_URL + 'ItemReceipt/GetRelatedOrderList?receiptId='
                + $scope.selectedReceiptId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.relatedOrderList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.goToOrder = function (orderId) {
        window.location.href = HOST_URL + 'PIOrder?rid=' + orderId;
    }

    // ON LOAD EVENTS
    $scope.$on('loadRelatedOrderList', function (e, d) {
        $scope.selectedReceiptId = d;
        $scope.loadRelatedOrderList();
    });
});