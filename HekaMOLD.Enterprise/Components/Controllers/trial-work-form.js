app.controller('trialWorkCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        Quantity: 0,
        CompleteQuantity: 0,
        WastageQuantity: 0,
    };

    $scope.trialMachines = [];
    $scope.selectedTrialMachine = {};

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Common/GetMachineList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.trialMachines = resp.data;
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.saveTrial = function () {
        if ($scope.modelObject.Quantity <= 0) {
            toastr.error('Devam edebilmek için pozitif bir üretilecek miktar girmelisiniz.', 'Uyarı');
            return;
        }

        if (typeof $scope.selectedTrialMachine != 'undefined' && $scope.selectedTrialMachine != null)
            $scope.modelObject.MachineId = $scope.selectedTrialMachine.Id;
        else
            $scope.modelObject.MachineId = null;

        try {
            $http.post(HOST_URL + 'Planning/CreateTrialPlan', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('trialPlanCreated', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(() => {

    });
});