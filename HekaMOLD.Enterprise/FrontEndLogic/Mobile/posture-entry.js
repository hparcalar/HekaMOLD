app.controller('postureEntryCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.machineQueue = [];

    $scope.bindModel = function (id) {

    }

    $scope.loadMachineQueue = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetPostureList?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineQueue = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // POSTURE CATEGORY SELECTION EVENTS
    $scope.selectedPostureCategory = { Id: 0 };

    $scope.showPostureCategoryList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadPostureCategoryList');

        $('#dial-categorylist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // EMIT SELECTED POSTURE CATEGORY DATA
    $scope.$on('postureCategorySelected', function (e, d) {
        $scope.selectedPostureCategory = d;
        $('#dial-categorylist').dialog('close');
        $scope.createPosture();
    });

    // MACHINE SELECTION EVENTS
    $scope.selectedMachine = { Id: 0, MachineName: '' };

    $scope.showMachineList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadMachineList');

        $('#dial-machinelist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // EMIT SELECTED MACHINE DATA
    $scope.$on('machineSelected', function (e, d) {
        $scope.selectedMachine = d;
        $scope.loadMachineQueue();

        $('#dial-machinelist').dialog('close');
    });

    // CREATE POSTURE EVENT
    $scope.createPosture = function () {
        bootbox.confirm({
            message: "Bu duruş girişini onaylıyor musunuz?",
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

                    $scope.modelObject.CreatedDate = moment().format('DD.MM.YYYY');

                    if (typeof $scope.selectedMachine != 'undefined'
                        && $scope.selectedMachine != null)
                        $scope.modelObject.MachineId = $scope.selectedMachine.Id;
                    else
                        $scope.modelObject.MachineId = null;

                    if (typeof $scope.selectedPostureCategory != 'undefined'
                        && $scope.selectedPostureCategory != null)
                        $scope.modelObject.PostureCategoryId = $scope.selectedPostureCategory.Id;
                    else
                        $scope.modelObject.PostureCategoryId = null;

                    $http.post(HOST_URL + 'Mobile/SavePosture', $scope.modelObject, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        Id: 0,
                                        MachineId: 0,
                                        PostureCategoryId:0,
                                    };

                                    $scope.loadMachineQueue();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // STOP AN ONGOINT POSTURE
    $scope.stopPosture = function (item) {
        bootbox.prompt({
            message: "Açıklama",
            closeButton: false,
            title: 'Bu duruş sona erecektir. Onaylıyor musunuz?',
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
                if (result != null) {
                    $scope.saveStatus = 1;

                    $http.post(HOST_URL + 'Mobile/FinishPosture', { postureId: item.Id, description: result }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.loadMachineQueue();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // LOAD EVENTS
    setTimeout($scope.showMachineList, 500);
});