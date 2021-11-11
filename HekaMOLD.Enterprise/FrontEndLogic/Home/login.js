var app = angular.module('loginApp', []);

app.controller('loginCtrl', function ($scope, $http) {
    $scope.modelObject = {
        password: '',
        errorMessage:'',
    };

    $scope.userList = [];
    $scope.machineList = [];

    $scope.selectedUser = { Id: 0 };
    $scope.selectedMachine = { Id: 0 };

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Home/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.userList = resp.data.Users;
                        $scope.machineList = resp.data.Machines;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.getIsProductionUser = function () {
        try {
            return $scope.selectedUser.IsProdTerminal == true && $scope.selectedUser.IsProdChief != true;
        } catch (e) {

        }

        return false;
    }



    // ON LOAD EVENTS
    $scope.loadSelectables().then(function () {
        $scope.$apply();
    })
});