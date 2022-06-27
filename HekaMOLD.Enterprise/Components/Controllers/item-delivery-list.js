app.controller('itemDeliveryListCtrl', function ($scope, $http) {
    $scope.itemDeliveryList = [];
    $scope.selectedDetailId = 0;

    $scope.loadItemDeliveries = function () {
        try {
            $http.get(HOST_URL + 'WorkOrder/GetItemDeliveryList?workOrderDetailId='
                + $scope.selectedDetailId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemDeliveryList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.saveDetail = function (detailId) {
        var detailObj = $scope.itemDeliveryList.find(d => d.Id == detailId);
        if (detailObj != null) {
            $http.post(HOST_URL + 'WorkOrder/UpdateItemDelivery', detailObj, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Status == 1) {
                            toastr.success('Kayıt başarılı.', 'Bilgilendirme');
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });
        }
    }

    $scope.deleteDetail = function (detailId) {
        bootbox.confirm({
            message: "Bu teslimatı geri almak istediğinizden emin misiniz?",
            closeButton: false,
            buttons: {
                confirm: {
                    label: 'Evet',
                    className: 'btn-primary'
                },
                cancel: {
                    label: 'Hayır',
                    className: 'btn-light'
                }
            },
            callback: function (result) {
                if (result) {
                    $http.post(HOST_URL + 'WorkOrder/DeleteItemDelivery', { receiptDetailId: detailId }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                if (resp.data.Status == 1) {
                                    toastr.success('Teslimat kaydı silindi.', 'Bilgilendirme');
                                    $scope.loadItemDeliveries();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // ON LOAD EVENTS
    $scope.$on('loadItemDeliveries', function (e, d) {
        $scope.selectedDetailId = d;
        $scope.loadItemDeliveries();
    });
});