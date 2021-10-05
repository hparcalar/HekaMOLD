app.controller('moldTestCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.selectedMachine = { Id: 0, MachineName: '' };
    $scope.machineList = [];

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.showAttachmentList = function () {
        $scope.$broadcast('showAttachmentList',
            { RecordId: $scope.modelObject.Id, RecordType: 3 });

        $('#dial-attachments').dialog({
            width: 500,
            height: 400,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu denemeyi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'MoldTest/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedMachine != 'undefined' && $scope.selectedMachine != null)
            $scope.modelObject.MachineId = $scope.selectedMachine.Id;
        else
            $scope.modelObject.MachineId = null;

        $http.post(HOST_URL + 'MoldTest/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'MoldTest/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $scope.modelObject.TestDate = $scope.modelObject.TestDateStr;

                    if (typeof $scope.modelObject.MachineId != 'undefined' && $scope.modelObject.MachineId != null)
                        $scope.selectedMachine = $scope.machineList.find(d => d.Id == $scope.modelObject.MachineId);
                    else
                        $scope.selectedMachine = $scope.machineList[0];

                    refreshArray($scope.machineList);
                }
            }).catch(function (err) { });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'MoldTest/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineList = resp.data.Machines;

                        var emptyMcObj = { Id: 0, MachineName: '-- Seçiniz --' };
                        $scope.machineList.splice(0, 0, emptyMcObj);
                        $scope.selectedMachine = emptyMcObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
    });
});