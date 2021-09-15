app.controller('faultEntryCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };
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

    // EMIT SELECTED INCIDENT CATEGORY DATA
    $scope.$on('incidentCategorySelected', function (e, d) {
        $scope.selectedIncidentCategory = d;
        $('#dial-categorylist').dialog('close');
        $scope.createIncident();
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
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        Id: 0,
                                        MachineId: 0,
                                        IncidentCategoryId:0,
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

    // LOAD EVENTS
    setTimeout($scope.showMachineList, 500);
});