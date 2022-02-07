DevExpress.localization.locale('tr');

app.controller('printTemplateCtrl', function printTemplateCtrl($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.saveStatus = 0;
    $scope.cursor = 0;
    $scope.selectedDate = moment().format('DD.MM.YYYY');

    $scope.machineList = [];
    $scope.boardPlanList = [];

    $scope.onDateChanged = function () {
        $scope.cursor = moment().diff(moment($scope.selectedDate, 'DD.MM.YYYY'), 'days');
        $scope.loadRunningBoard();
    }

    $scope.goPrev = function () {
        $scope.cursor--;
        $scope.selectedDate = moment().add('days', $scope.cursor).format('DD.MM.YYYY');
        $scope.loadRunningBoard();
    }

    $scope.goNext = function () {
        $scope.cursor++;
        $scope.selectedDate = moment().add('days', $scope.cursor).format('DD.MM.YYYY');
        $scope.loadRunningBoard();
    }

    $scope.getMachinePlans = function (machine) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus >= 3);
    }

    $scope.getFuturePlans = function (machine) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus < 3);
    }

    //$scope.getMachineTotalPlanned = function (machine) {
    //    return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus >= 3)
    //        .reduce((p, a) => p + a, 0);
    //}

    $scope.getMachineTotalPlanned = function (machine) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus >= 3)
            .map(d => d.WorkOrder.Quantity)
            .reduce((p, a) => p + a, 0);
    }

    $scope.getFutureTotalPlanned = function (machine) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus < 3)
            .map(d => d.WorkOrder.Quantity)
            .reduce((p, a) => p + a, 0);
    }

    $scope.getMachineTotalComplete = function (machine) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id && d.WorkOrder.WorkOrderStatus >= 3)
            .map(d => d.WorkOrder.CompleteQuantity)
            .reduce((p, a) => p + a, 0);
    }

    $scope.printPlanning = function () {
        window.print();
    }

    // DATA GET METHODS
    $scope.loadMachineList = function () {
        $http.get(HOST_URL + 'Planning/GetMachineList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data;

                    $scope.loadRunningBoard();
                }
            }).catch(function (err) { });
    }

    $scope.loadRunningBoard = function () {
        $http.get(HOST_URL + 'Planning/GetProductionPlanViews?date=' + $scope.selectedDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.boardPlanList = resp.data;
                }
            }).catch(function (err) { });
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