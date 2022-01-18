app.controller('vehicleInsuranceCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedVehicle= {};
    $scope.vehicleList = [];

    $scope.selectedVehicleInsuranceType = {};
    $scope.vehicleInsuranceTypeList = [];

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
            $http.get(HOST_URL + 'VehicleInsurance/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleList = resp.data.Vehicles;
                        $scope.vehicleInsuranceTypeList = resp.data.VehicleInsuranceTypes;
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
            message: "Bu araç sigorta bilgisini silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'VehicleInsurance/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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
        $http.post(HOST_URL + 'VehicleInsurance/SaveModel', {
            Id: $scope.modelObject.Id,
            Amount: $scope.modelObject.Amount,
            StartDateStr: $scope.modelObject.StartDateStr,
            EndDateStr: $scope.modelObject.EndDateStr,
            Plate: $scope.selectedVehicle.Plate,
            VehicleId: $scope.selectedVehicle.Id,
            KmHour: $scope.selectedVehicle.KmHour,
            OperationFirmId: $scope.selectedOperationFirm.Id,
            VehicleInsuranceTypeId: $scope.selectedVehicleInsuranceType.Id,
            ForexTypeId: $scope.selectedForexType.Id,
            External: $scope.modelObject.External

        }, 'json')
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
        $http.get(HOST_URL + 'VehicleInsurance/BindModel?rid=' + id, {}, 'json')
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

                        if ($scope.modelObject.VehicleInsuranceTypeId > 0)
                            $scope.selectedVehicleInsuranceType = $scope.vehicleInsuranceTypeList.find(d => d.Id == $scope.modelObject.VehicleInsuranceTypeId);
                        else
                            $scope.selectedVehicleInsuranceType = {};

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