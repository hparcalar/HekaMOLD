app.controller('weavingDraftCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedMachineBreed = {};
    $scope.machineBreedList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedMachineBreed = {};
    }
    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'WeavingDraft/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.machineBreedList = resp.data.MachineBreeds;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.getNextWeavingDraftCode = function (strCode) {
        $http.get(HOST_URL + 'WeavingDraft/GetNextWeavingDraftCode?Code=' + $scope.selectedMachineBreed.MachineBreedCode, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {

                    if (resp.data.Result) {
                        $scope.modelObject.WeavingDraftCode = resp.data.ReceiptNo;
                    }
                    else {
                        toastr.error('Sıradaki tahar Kodu üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');

                    }

                }
            }).catch(function (err) { });
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu tahar tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'WeavingDraft/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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
        if (typeof $scope.selectedMachineBreed != 'undefined' && $scope.selectedMachineBreed != null) {
            $scope.modelObject.MachineBreedId = $scope.selectedMachineBreed.Id;
        }
        else
            $scope.modelObject.YarnColourGroupId = null;
        $http.post(HOST_URL + 'WeavingDraft/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'WeavingDraft/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL 

                    if ($scope.modelObject.MachineBreedId > 0) {
                        $scope.selectedMachineBreed = $scope.machineBreedList.find(d => d.Id == $scope.modelObject.MachineBreedId);
                    }
                    else {
                        $scope.selectedMachineBreed = {};
                    }

                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.bindModel(0);
        }
    });
});