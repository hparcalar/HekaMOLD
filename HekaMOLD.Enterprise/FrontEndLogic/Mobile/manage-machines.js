app.controller('machineOnlineCtrl', function ($scope, $http) {
    $scope.machineList = [];
    $scope.filterModel = { startDate: moment().format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };
    $scope.selectedMachineId = 0;

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Machine/GetMachineStats?t1=' + $scope.filterModel.startDate + '&t2='
            + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data.Data;
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
                    $scope.selectedMachineId = item.Id;

                    if (item.MachineStatus == 1) { // DURUŞ SEBEBİ SOR
                        $scope.showPostureCategoryList();
                    }
                    else
                        $scope.postMacStatus(item.Id);
                }
            });
        } catch (e) {

        }
    }

    $scope.postMacStatus = function (machineId) {
        $http.post(HOST_URL + 'Mobile/ToggleMachineStatus', { machineId: machineId }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {

                    if (resp.data.Result == true) {
                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

                        $scope.bindModel(0);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');

                    $scope.selectedMachineId = 0;
                }
            }).catch(function (err) { });
    }

    // #region POSTURE SELECTION
    $scope.selectedPostureCategory = { Id: 0 };

    $scope.showPostureCategoryList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadPostureCategoryList');

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

    $scope.$on('postureCategorySelected', function (e, d) {
        $scope.selectedPostureCategory = d;
        $('#dial-categorylist').dialog('close');
        $scope.createPosture();
    });

    $scope.createPosture = function () {
        $scope.saveStatus = 1;

        $scope.postureObject = { Id: 0 };

        $scope.postureObject.CreatedDate = moment().format('DD.MM.YYYY');
        $scope.postureObject.MachineId = $scope.selectedMachineId;

        if (typeof $scope.selectedPostureCategory != 'undefined'
            && $scope.selectedPostureCategory != null)
            $scope.postureObject.PostureCategoryId = $scope.selectedPostureCategory.Id;
        else
            $scope.postureObject.PostureCategoryId = null;

        $http.post(HOST_URL + 'Mobile/SavePosture', $scope.postureObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('Duruş kaydı oluşturuldu.', 'Bilgilendirme');

                        $scope.postMacStatus($scope.selectedMachineId);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }
    // #endregion

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