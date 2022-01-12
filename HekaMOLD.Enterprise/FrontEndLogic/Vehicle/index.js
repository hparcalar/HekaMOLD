app.controller('vehicleCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedVehicleType = {};
    $scope.vehicleTypeList = [];

    $scope.selectedTrailerType = {};
    $scope.trailerTypeList = [{ Id: 1, Text: 'Çadırlı' },
        { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı' }];

    $scope.selectedVehicleAllocationType = {};
    $scope.vehicleAllocationTypeList = [{ Id: 1, Text: 'Satın Alma' },
        { Id: 2, Text: 'Kiralık' }];

    $scope.selectedForexType = {};
    $scope.forexTypeList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedTrailerType = {};
        $scope.selectedVehicleType = {};
        $scope.selectedVehicleAllocationType = {};
        $scope.selectedForexType = {};
    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Vehicle/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleTypeList = resp.data.VehicleTypes;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu firma tanımını silmek istediğinizden emin misiniz?",
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
                    $scope.saveStatus = 1;
                    $http.post(HOST_URL + 'Vehicle/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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
        alert($scope.modelObject.Plate);
        alert($scope.modelObject.Model);
        $http.post(HOST_URL + 'Vehicle/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Vehicle/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});