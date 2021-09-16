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

    $scope.loadActiveWorkOrder = function () {
        try {
            $http.get(HOST_URL + 'Common/GetActiveWorkOrderOnMachine?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.activeWorkOrder = resp.data;
                        if ($scope.lastPackageQty > 0)
                            $scope.activeWorkOrder.WorkOrder.InPackageQuantity = $scope.lastPackageQty;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // EMIT SELECTED MACHINE DATA
    $scope.$on('machineSelected', function (e, d) {
        $scope.selectedMachine = d;
        $scope.selectedWorkOrder = { Id: 0 };
        $scope.loadMachineQueue();
        $scope.loadActiveWorkOrder();

        $('#dial-machinelist').dialog('close');
    });

    // LOAD EVENTS
    setTimeout($scope.showMachineList, 500);
});