app.controller('workOrderPlanningCtrl', function planningCtrl($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.saveStatus = 0;

    $scope.machineList = [];
    $scope.selectedMachineList = [];
    $scope.boardPlanList = [];
    $scope.selectedPlan = { Id: 0 };
    $scope.copiedPlan = { Id: 0 };
    $scope.selectedSource = { Id: 0 }; // Plan queue of a selected machine for highlighting

    $scope.getMachinePlans = function (machine, isActive = true) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id
            && ((isActive == true && d.WorkOrder.WorkOrderStatus != 6 && d.WorkOrder.WorkOrderStatus != 4)
                || (isActive == false && (d.WorkOrder.WorkOrderStatus == 6 || d.WorkOrder.WorkOrderStatus == 4))));
    }

    $scope.selectPlan = function (planItem) {
        $scope.selectedPlan = planItem;
        $scope.selectedSource = $scope.machineList.find(d => d.Id == planItem.MachineId);
    }

    $scope.selectSource = function (machineId) {
        $scope.selectedSource = $scope.machineList.find(d => d.Id == machineId);
    }

    $scope.showProperProductName = function (workOrder) {
        return workOrder.ProductName.replace(workOrder.FirmName.split(' ')[0], '');
    }

    // DATA GET METHODS
    $scope.loadMachineList = function () {
        $http.get(HOST_URL + 'Planning/GetMachineList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data;

                    if ($scope.selectedMachineList.length == 0) {
                        $scope.machineList.forEach(d => {
                            $scope.selectedMachineList.push(d);
                        });
                    }

                    $scope.loadRunningBoard();
                }
            }).catch(function (err) { });
    }

    $scope.loadRunningBoard = function () {
        $http.get(HOST_URL + 'Planning/GetProductionPlans', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.boardPlanList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Planning/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // TOASTR SETTINGS
    toastr.options.timeOut = 2000;
    toastr.options.progressBar = true;
    toastr.options.preventDuplicates = true;

    // ON LOAD EVENTS
    $scope.loadMachineList();
});