app.controller('productPartSelection', function ($scope, $http, $timeout) {
    DevExpress.localization.locale('tr');

    $scope.workOrderDetailId = null;
    $scope.partVariants = [];
    $scope.selectedPart = null;

    $scope.loadParts = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetPartVariants?workOrderDetailId=' + $scope.workOrderDetailId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.partVariants = resp.data;
                    }
                });
        } catch (e) {

        }
    }

    $scope.selectPart = function (item) {
        $scope.selectedPart = item;
        $scope.$emit('productPartSelected', $scope.selectedPart);
    }

    // ON LOAD EVENTS
    $scope.$on('loadParts', function (e, d) {
        $scope.workOrderDetailId = d;
        $scope.loadParts();
    });
});