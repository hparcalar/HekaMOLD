app.controller('dashboardCtrl', function ($scope, $http, $interval) {
    $scope.machineList = [];
    $scope.profileList = [];

    $scope.filterModel = { startDate: moment().format('DD.MM.YYYY'), endDate: moment().format('DD.MM.YYYY') };

    $scope.isBindingModel = false;
    $scope.bindModel = function () {
        if ($scope.isBindingModel)
            return;

        $scope.isBindingModel = true;

        $http.get(HOST_URL + 'Machine/GetMachineStatsForDashboard', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {

                    try {
                        if ($scope.profileList.length > 0) {
                            for (var i = 0; i < resp.data.length; i++) {
                                var cMac = resp.data[i];
                                var relatedProfile = $scope.profileList.find(d => d.Id == cMac.WorkingUserId);
                                if (relatedProfile != null) {
                                    cMac.ProfileImageBase64 = relatedProfile.ProfileImageBase64;
                                }
                                else
                                    cMac.ProfileImageBase64 = HOST_URL + 'assets/media/avatars/blank.png';
                            }
                        }
                        else {
                            for (var i = 0; i < resp.data.length; i++) {
                                resp.data[i].ProfileImageBase64 = HOST_URL + 'assets/media/avatars/blank.png';
                            }
                        }
                    } catch (e) {

                    }

                    $scope.machineList = resp.data;
                    for (var i = 0; i < $scope.machineList.length; i++) {
                        var cMac = $scope.machineList[i];
                        cMac.MachineStats.ShiftStats = cMac.MachineStats.ShiftStats.filter(d => d.IsCurrentShift == true);
                    }

                    $scope.getUserProfiles();
                    $scope.isBindingModel = false;
                }
            }).catch(function (err) { });
    }

    $scope.getUserProfiles = function () {
        var userPrm = '';
        for (var i = 0; i < $scope.machineList.length; i++) {
            if ($scope.machineList[i].WorkingUserId != null)
                userPrm += $scope.machineList[i].WorkingUserId + ',';
        }

        if (userPrm.length > 0)
            userPrm = userPrm.substr(0, userPrm.length - 1);

        $http.get(HOST_URL + 'Machine/GetUserProfiles?userIdPrm=' + userPrm, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.profileList = resp.data;
                    $scope.drawProfiles();
                }
            }).catch(function (err) { });
    }

    $scope.drawProfiles = function () {
        $.each($('.user-img'), function (ix, val) {
            var userId = parseInt($(this).attr('data-user-id'));
            var relatedProfile = $scope.profileList.find(d => d.Id == userId);
            if (relatedProfile != null && relatedProfile.ProfileImageBase64.length > 0) {
                var currentUserId = $(this).attr('data-user-id');
                $(this).attr('src', relatedProfile.ProfileImageBase64);
            }
        });
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
    $scope.bindModel();

    $interval($scope.bindModel, 5000);

    setTimeout(function () { window.location.reload(); }, 1000 * 60 * 5);
});