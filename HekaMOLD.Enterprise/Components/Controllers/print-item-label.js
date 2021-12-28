app.controller('printItemLabelCtrl', function ($scope, $http) {
    $scope.printingItemId = 0;
    $scope.itemInfo = {};

    $scope.printingModel = { quantity:0, itemId:0, labelCount:1 };

    $scope.loadItemInfo = function () {
        $http.get(HOST_URL + 'Item/BindModel?rid=' + $scope.printingItemId, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.itemInfo = resp.data;
                    $scope.printingModel.itemId = resp.data.Id;
                }
            }).catch(function (err) { });
    }

    $scope.confirmPrinting = function () {
        $http.post(HOST_URL + 'Planning/PrintItemLabel', $scope.printingModel, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result) {
                        toastr.success('Etiket yazıcıya gönderildi.', 'Bilgilendirme');

                        $scope.bindModel(resp.data.RecordId);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.$on('showItemPrinting', function (e, d) {
        $scope.printingItemId = d;
        $scope.loadItemInfo();
    });
});