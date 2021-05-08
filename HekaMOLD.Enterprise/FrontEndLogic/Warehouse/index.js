app.controller('warehouseCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedWarehouseType = {};
    $scope.warehouseTypeList = [{ Id: 1, Text: 'Malzeme Deposu' }, { Id: 2, Text: 'Ürün Deposu' }];

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedWarehouseType = {};
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu depoyu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Warehouse/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedWarehouseType != 'undefined' && $scope.selectedWarehouseType != null) {
            $scope.modelObject.WarehouseType = $scope.selectedWarehouseType.Id;
        }
        else
            $scope.modelObject.FirmType = null;

        $http.post(HOST_URL + 'Warehouse/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Warehouse/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE
                    if ($scope.modelObject.WarehouseType > 0) {
                        $scope.selectedWarehouseType = $scope.warehouseTypeList.find(d => d.Id == $scope.modelObject.WarehouseType);
                    }
                    else {
                        $scope.selectedWarehouseType = {};
                    }
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});