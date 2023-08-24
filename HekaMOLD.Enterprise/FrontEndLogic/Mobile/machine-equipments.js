app.controller('machineEquipmentsCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };

    $scope.machineList = [];
    $scope.eqCategoryList = [];
    $scope.eqList = [];

    $scope.bindModel = async function (id) {
        var prmsMachine = new Promise(function (resolve, reject) {
            try {
                $http.get(HOST_URL + 'Common/GetMachineList', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.machineList = resp.data;
                            resolve();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        });

        await prmsMachine.then();

        var prmsEqCats = new Promise(function (resolve, reject) {
            try {
                $http.get(HOST_URL + 'Common/GetEquipmentCategoryList', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.eqCategoryList = resp.data;
                            resolve();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        });

        await prmsEqCats.then();

        var prmsAllEq = new Promise(function (resolve, reject) {
            try {
                $http.get(HOST_URL + 'Mobile/GetAllEquipments', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.eqList = resp.data;
                            resolve();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        });

        await prmsAllEq.then();
    }

    $scope.showNewEqCatForm = function () {
        bootbox.prompt("Kategori adını giriniz", function (result) {
            if (result && result.length > 0) {
                $http.post(HOST_URL + 'Mobile/SaveEquipmentCategory', { name: result }, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            if (resp.data.Status == 1) {
                                toastr.success('Kayıt başarılı.', 'Bilgilendirme');
                                $scope.bindModel();
                            }
                            else
                                toastr.error(resp.data.ErrorMessage, 'Hata');
                        }
                    }).catch(function (err) { });
            }
        });
    }

    $scope.getMachineData = function (category, machine) {
        try {
            var properEq = $scope.eqList.find(d => d.EquipmentCategoryId == category.Id && d.MachineId == machine.Id);
            if (properEq && properEq != null) {
                return properEq.EquipmentName;
            }
        } catch (e) {

        }
        return '';
    }

    $scope.editEquipment = function (category, machine) {
        var properEq = $scope.eqList.find(d => d.EquipmentCategoryId == category.Id && d.MachineId == machine.Id);

        // DO BROADCAST
        $scope.$broadcast('loadEquipment', {
            id: properEq && properEq != null ? properEq.Id : 0,
            machineId: machine.Id,
            equipmentCategoryId: category.Id,
        });

        $('#dial-edit-equipment').dialog({
            width: window.innerWidth * 0.7,
            /*height: window.innerHeight * 0.7,*/
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('editEquipmentEnd', function (e, d) {
        $('#dial-edit-equipment').dialog('close');
        $scope.bindModel();
    })

    $scope.editCategory = function (item) {
        if (item != null && item.Id > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadEquipmentCategory', { id: item.Id });

            $('#dial-edit-category').dialog({
                width: window.innerWidth * 0.7,
                /*height: window.innerHeight * 0.7,*/
                hide: true,
                modal: true,
                resizable: false,
                show: true,
                draggable: false,
                closeText: "KAPAT"
            });
        }
    }

    $scope.$on('editEquipmentCategoryEnd', function (e, d) {
        $('#dial-edit-category').dialog('close');
        $scope.bindModel();
    })

    // LOAD EVENTS
    $scope.bindModel();
});