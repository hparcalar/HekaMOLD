﻿app.controller('incidentListCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.machineQueue = [];

    $scope.selectedIncident = { Id: 0 };

    $scope.bindModel = function (id) {

    }

    $scope.loadMachineQueue = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetOngoingFaultList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineQueue = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectIncident = function (item) {
        $scope.selectedIncident = item;
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
        $scope.startIncident();
    });

    // INCIDENT STATUS EVENTS
    $scope.startIncident = function () {
        bootbox.confirm({
            message: "Bu arızaya müdaheleye başlanacaktır. Onaylıyor musunuz?",
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

                    $http.post(HOST_URL + 'Mobile/StartIncident', {
                        incidentId: $scope.selectedIncident.Id,
                        categoryId: $scope.selectedIncidentCategory.Id
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.selectedIncident = { Id: 0 };

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

    $scope.finishIncident = function () {
        bootbox.prompt({
            message: "Açıklama",
            closeButton: false,
            title: 'Bu arıza tamamlanacaktır. Onaylıyor musunuz?',
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

                    $http.post(HOST_URL + 'Mobile/FinishIncident', { incidentId: $scope.selectedIncident.Id, description: result }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.selectedIncident = { Id: 0 };

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
    $scope.isMechanics = IS_MECHANICS;
    setTimeout($scope.loadMachineQueue, 500);
});