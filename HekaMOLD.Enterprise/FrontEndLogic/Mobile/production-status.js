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

    $scope.showProductInfo = function () {
        try {
            if ($scope.selectedWorkOrder.Id > 0) {
                bootbox.alert({
                    size: 'xl',
                    centerVertical: true,
                    
                    title: $scope.selectedWorkOrder.ProductName + ' / Ürün Bilgileri',
                    message: '<iframe class="w-100 h-300px" src="' + HOST_URL + 'Mobile/ProductInformation?rid=' + $scope.selectedWorkOrder.WorkOrder.Id
                        + '&popup=1"></iframe>',
                    callback: function (result) {
                        
                    }
                });
            }
        } catch (e) {

        }
    }

    $scope.selectWorkOrder = function (workOrder) {
        $scope.selectedWorkOrder = workOrder;
    }

    $scope.holdWorkOrder = function () {
        try {
            bootbox.confirm({
                message: "Bu iş emrini beklemeye almak istediğinize emin misiniz?",
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

                        $http.post(HOST_URL + 'Mobile/HoldWorkOrder',
                            { workOrderDetailId: $scope.selectedWorkOrder.WorkOrderDetailId }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    $scope.saveStatus = 0;

                                    if (resp.data.Result == true) {
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    }
                                    else
                                        toastr.error(resp.data.ErrorMessage, 'Hata');

                                    $scope.loadMachineQueue();
                                    /*$scope.loadActiveWorkOrder();*/
                                }
                            }).catch(function (err) { });
                    }
                }
            });
        } catch (e) {

        }
    }

    $scope.toggleWorkOrderStatus = function () {
        try {
            bootbox.confirm({
                message: "Bu iş emrini " + ($scope.selectedWorkOrder.WorkOrderStatus == 3 ? 'bitirmek' : 'başlatmak')
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
                            { workOrderDetailId: $scope.selectedWorkOrder.WorkOrderDetailId }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    $scope.saveStatus = 0;

                                    if (resp.data.Result == true) {
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    }
                                    else
                                        toastr.error(resp.data.ErrorMessage, 'Hata');

                                    $scope.loadMachineQueue();
                                    /*$scope.loadActiveWorkOrder();*/
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
                            $scope.activeWorkOrder.InPackageQuantity = $scope.lastPackageQty;
                        $scope.selectedWorkOrder = $scope.activeWorkOrder;
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