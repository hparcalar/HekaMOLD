app.controller('machineOnlineCtrl', function ($scope, $http) {
    $scope.machineList = [];
    $scope.filterModel = { startDate: moment().format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Machine/GetMachineStats?t1=' + $scope.filterModel.startDate + '&t2='
            + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.toggleMachineStart = function (item) {
        try {
            bootbox.confirm({
                message: "Bu makineyi "+ (item.MachineStatus == 1 ? "durdurmak" : "başlatmak") +" istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Mobile/ToggleMachineStatus', { machineId: item.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {

                                if (resp.data.Result == true) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.bindModel(0);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            });
        } catch (e) {

        }
    }

    $scope.getFixed = function (arg, point) {
        try {
            return arg.toFixed(point);
        } catch (e) {

        }

        return arg;
    }

    $scope.showIncidents = function (machineId) {
        $scope.$broadcast('showIncidents',
            {
                MachineId: machineId,
                StartDate: $scope.filterModel.startDate,
                EndDate: $scope.filterModel.endDate
            });

        $('#dial-incidents').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.8,
            height: window.innerHeight * 0.8,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.showPostures = function (machineId) {
        $scope.$broadcast('showPostures',
            {
                MachineId: machineId,
                StartDate: $scope.filterModel.startDate,
                EndDate: $scope.filterModel.endDate
            });

        $('#dial-postures').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.8,
            height: window.innerHeight * 0.8,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // ON LOAD EVENTS
    $scope.bindModel(PRM_ID);
});