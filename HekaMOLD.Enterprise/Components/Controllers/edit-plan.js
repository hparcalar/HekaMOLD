app.controller('editPlanCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id:0,
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
            $http.post(HOST_URL + 'Planning/EditPlan', $scope.modelObject, 'json')
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
            message: "Bu üretim planını kapatmak istediğinizden emin misiniz?",
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
                        $http.post(HOST_URL + 'Planning/CompletePlan', { id: $scope.modelObject.Id }, 'json')
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

    $scope.bindPlanDetail = function () {
        try {
            $http.get(HOST_URL + 'Planning/GetPlanDetail?workOrderDetailId=' + $scope.modelObject.Id, { }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.printLabel = function () {
        $scope.$broadcast('showPrintOptions');

        $('#dial-print-options').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: 300,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('printOptionsApproved', function (e, d) {
        $('#dial-print-options').dialog('close');

        try {
            $http.post(HOST_URL + 'Planning/AllocateAndPrintLabel', {
                model: { PrinterId: d.PrinterId, RecordType: 4 },
                labelCount: d.PrintCount,
                workOrderDetailId: $scope.modelObject.Id,
            }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İstek yazıcıya iletildi.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    });

    $scope.$on('cancelPrinting', function (e, d) {
        $('#dial-print-options').dialog('close');
    });

    // ON LOAD EVENTS
    $scope.$on('loadEditPlan', function (e, d) {
        $scope.modelObject.Id = d.id;

        $scope.bindPlanDetail();
    });
});