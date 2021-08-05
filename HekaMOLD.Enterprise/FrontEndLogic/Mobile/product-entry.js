app.controller('productEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
    };

    $scope.bindModel = function (id) {
        $scope.modelObject = {
            Id: 0,
            MachineId: 0
        };
    }

    $scope.activeWorkOrder = { Id:0 };
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
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // EMIT SELECTED MACHINE DATA
    $scope.$on('machineSelected', function (e, d) {
        $scope.selectedMachine = d;
        $scope.loadActiveWorkOrder();

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

                    $http.post(HOST_URL + 'Mobile/SaveProductEntry', { workOrderDetailId: $scope.activeWorkOrder.WorkOrder.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');
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