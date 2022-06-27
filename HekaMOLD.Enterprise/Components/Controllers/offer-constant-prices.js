app.controller('offerConstantsCtrl', function ($scope, $http) {
    $scope.rawSheetPrice = 0;
    $scope.wastagePrice = 0;
    $scope.manPrice = 0;
    $scope.profitRate = 0;
    $scope.expirationVal = '';

    $scope.saveConstants = function () {
        try {
            $http.post(HOST_URL + 'Common/SaveOfferConstants', {
                rawSheetPrice: $scope.rawSheetPrice.toString().replace(',', '.'),
                wastagePrice: $scope.wastagePrice.toString().replace(',', '.'),
                manPrice: $scope.manPrice.toString().replace(',', '.'),
                profitRate: $scope.profitRate.toString().replace(',', '.'),
                expirationVal: $scope.expirationVal,
            }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('Sabit değerler kaydedildi.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editOfferConstantsEnd', null);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.bindConstants = function () {
        try {
            $http.get(HOST_URL + 'Common/GetOfferConstants', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.rawSheetPrice = parseFloat(resp.data.RawSheetPrice);
                        $scope.wastagePrice = parseFloat(resp.data.WastagePrice);
                        $scope.manPrice = parseFloat(resp.data.ManPrice);
                        $scope.profitRate = parseFloat(resp.data.ProfitRate);
                        $scope.expirationVal = resp.data.ExpirationVal;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadOfferConstants', function (e, d) {
        $scope.bindConstants();
    });
});