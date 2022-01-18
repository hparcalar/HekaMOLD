app.controller('vehicleTireCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, CareDateStr: moment().format('DD.MM.YYYY'), };

    $scope.saveStatus = 0;

    $scope.selectedVehicle = {};
    $scope.vehicleList = [];

    $scope.selectedVehicleTireType = {};
    $scope.vehicleTireTypeList = [];

    $scope.selectedOperationFirm = {};
    $scope.operationFirmList = [];

    $scope.selectedForexType = {};
    $scope.forexTypeList = [];

    $scope.selectedVehicleTireType = {};
    $scope.vehicleTireTypeList = [{ Id: 1, Text: 'Değişim' }, { Id: 2, Text: 'Onarım' },
        { Id: 3, Text: 'Bakım' }];

    $scope.selectedVehicleTireDirectionType = {};
    $scope.vehicleTireDirectionTypeList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedVehicle = {};
        $scope.selectedOperationFirm = {};
        $scope.selectedForexType = {};
        $scope.selectedVehicleTireDirectionType = {};

    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'VehicleTire/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleList = resp.data.Vehicles;
                        $scope.operationFirmList = resp.data.Firms;
                        $scope.forexTypeList = resp.data.ForexTypes;
                        $scope.vehicleTireDirectionTypeList = resp.data.VehicleTireDirectionTypes;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.performDelete = function ()

    {
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
                    $http.post(HOST_URL + 'VehicleTire/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedVehicleTireType != 'undefined' && $scope.selectedVehicleTireType != null)
            $scope.modelObject.VehicleTireDrectionTypeId = $scope.selectedVehicleTireType.Id;
        else
            $scope.modelObject.VehicleTireDrectionTypeId = null;

        if (typeof $scope.selectedVehicleTireType != 'undefined' && $scope.selectedVehicleTireType != null)
            $scope.modelObject.VehicleTireTypeId = $scope.selectedVehicleTireType.Id;
        else
            $scope.modelObject.VehicleTireTypeId = null;

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

        $http.post(HOST_URL + 'VehicleTire/SaveModel', {
            Id: $scope.modelObject.Id,
            VehicleId: $scope.selectedVehicle.Id,
            OperationFirmId: $scope.selectedOperationFirm.Id,
            VehicleTireDirectionTypeId: $scope.selectedVehicleTireDirectionType.Id,
            VehicleTireType: $scope.selectedVehicleTireType.Id,
            KmHour: $scope.modelObject.KmHour,
            Amount: $scope.modelObject.Amount,
            ForexTypeId: $scope.selectedForexType.Id,
            SeriNo: $scope.modelObject.SeriNo,
            DimensionsInfo: $scope.modelObject.DimensionsInfo,
            MontageDate: $scope.modelObject.MontageDateStr,
            Explanation: $scope.modelObject.Explanation

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
        $http.get(HOST_URL + 'VehicleTire/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    if ($scope.modelObject.VehicleTireDirectionTypeId > 0)
                        $scope.selectedVehicleTireDirectionType = $scope.vehicleTireDirectionTypeList.find(d => d.Id == $scope.modelObject.VehicleTireDirectionTypeId);
                    else
                        $scope.selectedVehicleTireDrectionType = {};

                   if ($scope.modelObject.ForexTypeId > 0)
                        $scope.selectedForexType = $scope.forexTypeList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else
                        $scope.selectedForexType = {};

                    if ($scope.modelObject.VehicleId > 0)
                        $scope.selectedVehicle = $scope.vehicleList.find(d => d.Id == $scope.modelObject.VehicleId);
                    else
                        $scope.selectedVehicle = {};

                    if ($scope.modelObject.VehicleTireType > 0)
                        $scope.selectedVehicleTireType = $scope.vehicleTireTypeList.find(d => d.Id == $scope.modelObject.VehicleTireType);
                    else
                        $scope.VehicleTireType = {};

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