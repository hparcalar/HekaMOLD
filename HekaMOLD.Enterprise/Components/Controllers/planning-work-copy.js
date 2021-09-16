app.controller('planningWorkCopyCtrl', function ($scope, $http) {
    $scope.modelObject = {
        quantity: 0,
        firmId: 0,
    };

    $scope.selectedFirm = {};
    $scope.firmList = [];

    $scope.loadFirmList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetFirmList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.firmList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.confirmCopy = function () {
        if ($scope.selectedFirm.Id <= 0) {
            toastr.error('Devam edebilmek için firma seçmelisiniz.', 'Uyarı');
            return;
        }

        if ($scope.modelObject.quantity <= 0) {
            toastr.error('Devam edebilmek için pozitif bir hedef miktar girmelisiniz.', 'Uyarı');
            return;
        }

        $scope.modelObject.firmId = $scope.selectedFirm.Id;
        $scope.$emit('confirmCopy', $scope.modelObject);
    }

    // ON LOAD EVENTS
    $scope.$on('loadPlanCopy', function (e, d) {
        $scope.modelObject.quantity = 0;
        $scope.modelObject.firmId = 0;

        $scope.loadFirmList();
    });
});