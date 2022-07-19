app.controller('editShiftUsersCtrl', function ($scope, $http) {
    $scope.shiftList = [];
    $scope.userList = [];

    $scope.selectedShift = { Id: 0 };

    $scope.saveShiftList = function () {
        try {
            $http.post(HOST_URL + 'Delivery/EditPlan', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editPlanEnd', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectUser = function (user) {
        if ($scope.selectedShift.Id == 0) {
            toastr.error('Önce bir vardiya seçmelisiniz.');
            return;
        }

        const shift = $scope.shiftList.find(d => d.Id == $scope.selectedShift.Id);
        if (shift) {
            user.ShiftId = shift.Id;
            shift.Users.push(user);

            const userIndex = $scope.userList.indexOf(user);
            $scope.userList.splice(userIndex, 1);

            try {
                $scope.$applyAsync();
            } catch (e) {

            }
        }
    }

    $scope.unSelectUser = function (user) {
        if (user.ShiftId > 0) {
            const shift = $scope.shiftList.find(d => d.Id == user.ShiftId);
            if (shift) {
                user.ShiftId = null;
                const userIndex = shift.Users.indexOf(user);
                shift.Users.splice(userIndex, 1);
                $scope.userList.push(user);

                try {
                    $scope.$applyAsync();
                } catch (e) {

                }
            }
        }
    }

    $scope.selectShift = function (sh) {
        $scope.selectedShift = sh;
    }

    $scope.bindShiftList = function () {
        try {
            $http.get(HOST_URL + 'User/GetUserList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.userList = resp.data;

                        $http.get(HOST_URL + 'Shift/GetShiftListWithUsers', {}, 'json')
                            .then(function (respShift) {
                                if (typeof respShift.data != 'undefined' && respShift.data != null) {
                                    $scope.shiftList = respShift.data;

                                    $scope.userList = $scope.userList.filter(d => $scope.shiftList.some(m => m.Users.some(c => c.Id == d.Id)) == false);
                                }
                            }).catch(function (err) { });
                    }
                }).catch(function (err) { });
        } catch (e) {

        }

        try {
           
        } catch (e) {

        }
    }

    $scope.bindShiftList();

    // ON LOAD EVENTS
    $scope.$on('loadShiftUsers', function (e, d) {
        $scope.bindShiftList();
    });
});