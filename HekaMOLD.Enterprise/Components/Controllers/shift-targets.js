app.controller('shiftTargetsCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        Quantity: 0,
        CompleteQuantity: 0,
        WastageQuantity: 0,
    };

    $scope.save = function () {
        if ($scope.modelObject.Quantity <= 0) {
            toastr.error('Devam edebilmek için pozitif bir hedef miktar girmelisiniz.', 'Uyarı');
            return;
        }

        try {
            $http.post(HOST_URL + 'Shift/SaveTargets', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editPlanEnd', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.bindPlanDetail = function () {
        try {
            $http.get(HOST_URL + 'Planning/GetPlanDetail?workOrderDetailId=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadShiftTargets', function (e, d) {
        $scope.modelObject.Id = d.id;

        $scope.bindPlanDetail();
    });
});