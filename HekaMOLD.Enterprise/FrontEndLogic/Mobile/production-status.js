app.controller('productionStatusCtrl', function ($scope, $http) {
    $scope.machineQueue = [];
    $scope.selectedWorkOrder = {Id:0};

    $scope.bindModel = function (id) {
        
    }

    $scope.loadMachineQueue = function () {
        try {
            $http.get(HOST_URL + 'Common/GetMachineQueue?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineQueue = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectWorkOrder = function (workOrder) {
        $scope.selectedWorkOrder = workOrder;
    }

    $scope.toggleWorkOrderStatus = function () {
        try {
            bootbox.confirm({
                message: "Bu iş emrini " + ($scope.selectedWorkOrder.WorkOrder.WorkOrderStatus == 3 ? 'bitirmek' : 'başlatmak')
                    + " istediğinize emin misiniz?",
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

                        $http.post(HOST_URL + 'Mobile/ToggleWorkOrderStatus',
                            { workOrderDetailId: $scope.selectedWorkOrder.WorkOrder.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    $scope.saveStatus = 0;

                                    if (resp.data.Result == true) {
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    }
                                    else
                                        toastr.error(resp.data.ErrorMessage, 'Hata');

                                    $scope.loadMachineQueue();
                                }
                            }).catch(function (err) { });
                    }
                }
            });
        } catch (e) {
            
        }
    }

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
        $scope.selectedWorkOrder = { Id: 0 };
        $scope.loadMachineQueue();

        $('#dial-machinelist').dialog('close');
    });

    $scope.approveProductEntry = function () {
        bootbox.confirm({
            message: "Bu ürün girişini onaylıyor musunuz?",
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

                    $http.post(HOST_URL + 'Mobile/SaveProductEntry', $scope.modelObject, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        MachineId: 0,
                                    };
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