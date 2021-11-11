app.controller('productInformationCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
        MoldData: { Id: 0 },
        IsPopup: false,
    };

    $scope.activeDetails = [];

    $scope.historyWorkOrderDetailId = 0;
    $scope.selectedActiveDetailId = 0;

    $scope.bindModel = function (id) {
        $scope.modelObject = {
            Id: 0,
            MachineId: 0,
            Barcode: '',
            IsPopup: false,
        };
    }

    $scope.lastPackageQty = 0;

    $scope.activeWorkOrder = { Id: 0 };
    $scope.selectedMachine = { Id: 0, MachineName: '' };

    $scope.getHourPart = function (text) {
        try {
            return text.substr(11, 5);
        } catch (e) {

        }

        return text;
    }

    $scope.getDatePart = function (text) {
        try {
            return text.substr(0, 10);
        } catch (e) {

        }

        return text;
    }

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
            if ($scope.historyWorkOrderDetailId > 0 || $scope.selectedActiveDetailId > 0) {
                var searchedId = $scope.historyWorkOrderDetailId > 0 ? $scope.historyWorkOrderDetailId : $scope.selectedActiveDetailId;

                $http.get(HOST_URL + 'Common/GetHistoryWorkOrderOnMachine?workOrderDetailId=' + searchedId, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.activeWorkOrder = resp.data;
                            $scope.loadMoldTest();
                        }
                    }).catch(function (err) { });
            }
            else {
                $http.get(HOST_URL + 'Common/GetActiveWorkOrderOnMachine?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.activeWorkOrder = resp.data;
                            if ($scope.lastPackageQty > 0)
                                $scope.activeWorkOrder.WorkOrder.InPackageQuantity = $scope.lastPackageQty;
                            $scope.loadMoldTest();
                        }
                    }).catch(function (err) { });
            }

            // CHECK IF MULTIPLE WORK ORDERS ACTIVE ON CURRENT MACHINE
            $http.get(HOST_URL + 'Mobile/GetMachineWorkList?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (respMac) {
                    if (typeof respMac.data != 'undefined' && respMac.data != null) {
                        $scope.activeDetails = respMac.data;
                    }
                });
        } catch (e) {

        }
    }

    $scope.loadMoldTest = function () {
        try {
            $http.get(HOST_URL + 'Common/GetMoldTestOfProduct?productCode=' + $scope.activeWorkOrder.WorkOrder.ProductCode, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.MoldData = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // PRODUCTION HISTORY
    $scope.showProductionHistory = function () {
        if ($scope.historyWorkOrderDetailId > 0) {
            $scope.historyWorkOrderDetailId = 0;
            $scope.loadActiveWorkOrder();
        }
        else {
            // DO BROADCAST
            $scope.$broadcast('loadProdHistory', $scope.selectedMachine.Id);

            $('#dial-prodlist').dialog({
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
    }

    // EMIT SELECTED MACHINE DATA
    $scope.$on('machineSelected', function (e, d) {
        $scope.selectedMachine = d;
        $scope.selectedActiveDetailId = 0;
        $scope.historyWorkOrderDetailId = 0;
        $scope.loadActiveWorkOrder();

        $('#dial-machinelist').dialog('close');
    });

    $scope.$on('prodSelected', function (e, d) {
        $scope.historyWorkOrderDetailId = d;
        $scope.loadActiveWorkOrder();

        $('#dial-prodlist').dialog('close');
    });

    $scope.showMultipleDetails = function () {
        if ($scope.activeDetails.length > 1) {
            var buttonsHtml = '';

            for (var i = 0; i < $scope.activeDetails.length; i++) {
                var wOrder = $scope.activeDetails[i];
                buttonsHtml += '<button type="button" class="btn my-2 btn-sm btn-block btn-warning active-work" data-id="' +
                    wOrder.Id
                    + '">' + wOrder.ProductName + '</button>';
            }

            bootbox.alert({
                message: '<div class="d-flex flex-column">' + buttonsHtml + '</div>',
                closeButton: false,
                locale: 'tr',
                callback: function () {
                    bootbox.hideAll();
                }
            });

            setTimeout(() => {
                $('.active-work').on("click", function () {
                    var detailId = $(this).attr('data-id');
                    $scope.selectedActiveDetailId = parseInt(detailId);
                    $scope.loadActiveWorkOrder();
                    bootbox.hideAll();
                });
            }, 200);
        }
    }

    // LOAD EVENTS
    if (parseInt(PRM_ID) > 0) {
        $scope.modelObject.IsPopup = true;
        $scope.selectedActiveDetailId = PRM_ID;
        $scope.loadActiveWorkOrder();
    }
    else {
        if (IS_PRODCHIEF)
            setTimeout($scope.showMachineList, 500);
        else {
            $scope.selectedMachine.Id = MACHINE_ID;
            $scope.selectedMachine.MachineName = MACHINE_NAME;
            $scope.modelObject.IsProdChief = IS_PRODCHIEF;

            $scope.loadActiveWorkOrder();
        }
    }
});