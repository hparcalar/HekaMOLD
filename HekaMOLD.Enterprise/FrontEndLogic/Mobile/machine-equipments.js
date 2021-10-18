app.controller('machineEquipmentsCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };

    $scope.machineList = [];
    $scope.eqCategoryList = [];

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
        return '';
    }

    // LOAD EVENTS
    $scope.bindModel();
});