app.controller('vehicleCareCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0};

    $scope.saveStatus = 0;

    $scope.selectedVehicle = {};
    $scope.vehicleList = [];

    $scope.selectedVehicleCareType = {};
    $scope.vehicleCareTypeList = [];

    $scope.selectedOperationFirm = {};
    $scope.operationFirmList = [];

    $scope.selectedForexType = {};
    $scope.forexTypeList = [];


    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedVehicle = {};
        $scope.selectedOperationFirm = {};
        $scope.selectedForexType = {};
    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'VehicleCare/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleList = resp.data.Vehicles;
                        $scope.vehicleCareTypeList = resp.data.VehicleCareTypes;
                        $scope.operationFirmList = resp.data.Firms;
                        $scope.forexTypeList = resp.data.ForexTypes;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu araç bakım bilgisini silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'VehicleCare/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedVehicleCareType != 'undefined' && $scope.selectedVehicleCareType != null)
            $scope.modelObject.VehicleCareTypeId = $scope.selectedVehicleCareType.Id;
        else
            $scope.modelObject.VehicleCareTypeId = null;

        if (typeof $scope.selectedVehicle != 'undefined' && $scope.selectedVehicle != null)
            $scope.modelObject.VehicleId = $scope.selectedVehicle.Id;
        else
            $scope.modelObject.VehicleId = null;

        if (typeof $scope.selectedOperationFirm != 'undefined' && $scope.selectedOperationFirm != null)
            $scope.modelObject.OperationFirmId = $scope.selectedOperationFirm.Id;
        else
            $scope.modelObject.OperationFirmId = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        else
            $scope.modelObject.ForexTypeId = null;

        $http.post(HOST_URL + 'VehicleCare/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'VehicleCare/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.modelObject.ForexTypeId > 0)
                        $scope.selectedForexType = $scope.forexTypeList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else
                        $scope.selectedForexType = {};

                    if ($scope.modelObject.VehicleId > 0)
                        $scope.selectedVehicle = $scope.vehicleList.find(d => d.Id == $scope.modelObject.VehicleId);
                    else
                        $scope.selectedVehicle = {};

                    if ($scope.modelObject.VehicleCareTypeId > 0)
                        $scope.selectedVehicleCareType = $scope.vehicleCareTypeList.find(d => d.Id == $scope.modelObject.VehicleCareTypeId);
                    else
                        $scope.selectedVehicleCareType = {};

                    if ($scope.modelObject.OperationFirmId > 0)
                        $scope.selectedOperationFirm = $scope.operationFirmList.find(d => d.Id == $scope.modelObject.OperationFirmId);
                    else
                        $scope.selectedOperationFirm = {};

                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});