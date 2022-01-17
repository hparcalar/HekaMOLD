app.controller('productQualityPlanCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0 };

    $scope.selectedCheckType = { };
    $scope.checkTypeList = [{ Id: 1, Text: 'Check' }, { Id:2, Text:'Sayısal' }];
    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.bindDetails();
    }


    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu proses kalite planını silmek istediğinizden emin misiniz?",
            closeButton:false,
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
                    $scope.saveStatus = 1;
                    $http.post(HOST_URL + 'ProductQuality/DeletePlanModel', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarıyla silindi.', 'Bilgilendirme');

                                    $scope.openNewRecord();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        if (typeof $scope.selectedCheckType != 'undefined' && $scope.selectedCheckType != null) {
            $scope.modelObject.CheckType = $scope.selectedCheckType.Id;
        }
        else
            $scope.modelObject.CheckType = 1;

        $http.post(HOST_URL + 'ProductQuality/SavePlanModel', $scope.modelObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                        $scope.bindModel(resp.data.RecordId);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'ProductQuality/BindPlanModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL CHECK TYPE
                    if ($scope.modelObject.CheckType > 0) {
                        $scope.selectedCheckType = $scope.checkTypeList.find(d => d.Id == $scope.modelObject.CheckType);
                    }
                    else {
                        $scope.selectedCheckType = {};
                    }
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});