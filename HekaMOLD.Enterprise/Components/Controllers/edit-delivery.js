app.controller('editDeliveryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id:0,
        Quantity: 0,
        CompleteQuantity: 0,
        WastageQuantity: 0,
        LabelConfigData: {
            ShowFirm:true,
        }
    };

    $scope.save = function () {
        if ($scope.modelObject.Quantity <= 0) {
            toastr.error('Devam edebilmek için pozitif bir sevkiyat miktarı girmelisiniz.', 'Uyarı');
            return;
        }

        try {
            $http.post(HOST_URL + 'Delivery/EditPlan', $scope.modelObject, 'json')
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

    $scope.completePlan = function () {
        bootbox.confirm({
            message: "Bu sevkiyat planını tamamlamak istediğinizden emin misiniz?",
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
                    try {
                        $http.post(HOST_URL + 'Deliery/CompletePlan', { rid: $scope.modelObject.Id }, 'json')
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
            }
        });
    }

    $scope.rollbackPlan = function () {
        bootbox.confirm({
            message: "Bu sevkiyat planını silmek istediğinizden emin misiniz?",
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
                    try {
                        $http.post(HOST_URL + 'Deliery/DeletePlan', { rid: $scope.modelObject.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    if (resp.data.Result == true) {
                                        toastr.success('Plan başarıyla silindi.', 'Bilgilendirme');
                                        $scope.$emit('editPlanEnd', $scope.modelObject);
                                    }
                                    else
                                        toastr.error(resp.data.ErrorMessage, 'Hata');
                                }
                            }).catch(function (err) { });
                    } catch (e) {

                    }
                }
            }
        });
    }

    $scope.bindPlanDetail = function () {
        try {
            $http.get(HOST_URL + 'Delivery/GetPlanDetail?rid=' + $scope.modelObject.Id, { }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadEditPlan', function (e, d) {
        $scope.modelObject.Id = d.id;

        $scope.bindPlanDetail();
    });
});