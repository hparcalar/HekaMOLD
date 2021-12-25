app.controller('palletViewCtrl', function ($scope, $http) {
    $scope.boxesOfPallet = [];
    $scope.viewPallet = 0;

    $scope.loadPalletView = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetBoxesOfPallet?palletId='
                + $scope.viewPallet, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.boxesOfPallet = resp.data.Boxes;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.removeFromPallet = function (boxItem) {
        $http.post(HOST_URL + 'Mobile/RemoveFromPallet', { itemSerialId: boxItem.Id }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    if (resp.data.Status == 1) {
                        toastr.success('Başarıyla paletten çıkartıldı.', 'Bilgilendirme');

                        $scope.loadPalletView();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.$on('loadPalletView', function (e, d) {
        $scope.viewPallet = d;
        $scope.loadPalletView();
    });
});