app.controller('faultEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        IsProdChief: false,
        IsMechanics: false,
    };
    $scope.machineQueue = [];
    $scope.selectedWorkOrder = { Id: 0 };

    $scope.bindModel = function (id) {

    }

    $scope.loadMachineQueue = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetIncidentList?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineQueue = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // INCIDENT CATEGORY SELECTION EVENTS
    $scope.selectedIncidentCategory = { Id: 0 };

    $scope.showIncidentCategoryList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadIncidentCategoryList');

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

    $scope.showPostureCategoryList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadPostureCategoryList');

        $('#dial-posture-categorylist').dialog({
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

    // EMIT SELECTED INCIDENT CATEGORY DATA
    $scope.$on('incidentCategorySelected', function (e, d) {
        $scope.selectedIncidentCategory = d;
        $('#dial-categorylist').dialog('close');
        $scope.createIncident();
    });

    // EMIT SELECTED POSTURE CATEGORY DATA
    $scope.$on('postureCategorySelected', function (e, d) {
        $scope.selectedPostureCategory = d;
        $('#dial-posture-categorylist').dialog('close');
        $scope.createPosture();
        $scope.applyIncidentEntry();
    });

    // CREATE POSTURE EVENT
    $scope.createPosture = function () {
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
                        toastr.success('Duruş başlatıldı.', 'Bilgilendirme');

                        $scope.modelObject = {
                            Id: 0,
                            MachineId: 0,
                            PostureCategoryId: 0,
                        };

                        $scope.loadMachineQueue();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

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

    // CREATE INCIDENT EVENT
    $scope.createIncident = function () {
        bootbox.confirm({
            message: "Bu arıza bildirimini onaylıyor musunuz?",
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
                    bootbox.confirm({
                        message: 'Duruş başlatmak istiyor musunuz?',
                        closeButton: false,
                        buttons: {
                            confirm: {
                                label: 'Evet',
                                className: 'btn-primary',
                            },
                            cancel: {
                                label: 'Hayır',
                                className: 'btn-light'
                            }
                        },
                        callback: function (resultPosture) {
                            if (resultPosture) {
                                $scope.showPostureCategoryList();
                            }
                            else {
                                $scope.applyIncidentEntry();
                            }
                        }
                    });

                    
                }
            }
        });
    }

    $scope.applyIncidentEntry = function () {
        $scope.saveStatus = 1;

        $scope.modelObject.CreatedDate = moment().format('DD.MM.YYYY');

        if (typeof $scope.selectedMachine != 'undefined'
            && $scope.selectedMachine != null)
            $scope.modelObject.MachineId = $scope.selectedMachine.Id;
        else
            $scope.modelObject.MachineId = null;

        if (typeof $scope.selectedIncidentCategory != 'undefined'
            && $scope.selectedIncidentCategory != null)
            $scope.modelObject.IncidentCategoryId = $scope.selectedIncidentCategory.Id;
        else
            $scope.modelObject.IncidentCategoryId = null;

        $http.post(HOST_URL + 'Mobile/SaveIncident', $scope.modelObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('Arıza kaydı girildi.', 'Bilgilendirme');

                        $scope.modelObject = {
                            Id: 0,
                            MachineId: 0,
                            IncidentCategoryId: 0,
                        };

                        $scope.loadMachineQueue();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    // LOAD EVENTS
    if (IS_PRODCHIEF || IS_MECHANICS)
        setTimeout($scope.showMachineList, 500);
    else {
        $scope.selectedMachine.Id = MACHINE_ID;
        $scope.selectedMachine.MachineName = MACHINE_NAME;
        $scope.modelObject.IsProdChief = IS_PRODCHIEF;
        $scope.modelObject.IsMechanics = IS_MECHANICS;

        $scope.loadMachineQueue();
    }
});