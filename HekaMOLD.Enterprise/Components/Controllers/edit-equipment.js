app.controller('editEquipmentCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
        EquipmentCategoryId: 0,
        Manufacturer: '',
        ModelNo: '',
        SerialNo: '',
        Location: '',
        EquipmentCode: '',
        EquipmentName: '',
    };

    $scope.machineId = 0;
    $scope.equipmentCategoryId = 0;
    $scope.machineName = '';
    $scope.categoryName = '';

    $scope.saveEquipment = function () {
        try {
            $scope.modelObject.EquipmentCode = $scope.modelObject.EquipmentName;

            $http.post(HOST_URL + 'Mobile/UpdateEquipment', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editEquipmentEnd', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.deleteEquipment = function () {
        bootbox.confirm({
            message: "Bu ekipmanı silmek istediğinizden emin misiniz?",
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
                        $http.post(HOST_URL + 'Mobile/DeleteEquipment', { rid: $scope.modelObject.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    if (resp.data.Result == true)
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    else
                                        toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                                }

                                $scope.$emit('editEquipmentEnd', $scope.modelObject);
                            }).catch(function (err) { });
                    } catch (e) {

                    }
                }
            }
        });
    }

    $scope.bindEquipment = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetEquipment?rid=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;

                        $scope.modelObject.MachineId = $scope.machineId;
                        $scope.modelObject.EquipmentCategoryId = $scope.equipmentCategoryId;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }

        try {
            $http.get(HOST_URL + 'Mobile/GetEquipmentHeader?machineId=' + $scope.modelObject.MachineId
                + '&equipmentCategoryId=' + $scope.modelObject.EquipmentCategoryId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineName = resp.data.MachineName;
                        $scope.categoryName = resp.data.EquipmentCategoryName;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadEquipment', function (e, d) {
        $scope.modelObject.Id = d.id;
        $scope.machineId = d.machineId;
        $scope.equipmentCategoryId = d.equipmentCategoryId;

        $scope.modelObject.MachineId = $scope.machineId;
        $scope.modelObject.EquipmentCategoryId = $scope.equipmentCategoryId;

        $scope.bindEquipment();
    });
});