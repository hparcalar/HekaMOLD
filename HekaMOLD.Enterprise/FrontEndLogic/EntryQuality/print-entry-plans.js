DevExpress.localization.locale('tr');

app.controller('printEntryPlansCtrl', function printTemplateCtrl($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.saveStatus = 0;

    $scope.planList = [];

    $scope.printPlans = function () {
        window.print();
    }

    // DATA GET METHODS
    $scope.loadPlans = function () {
        $http.get(HOST_URL + 'EntryQuality/GetEntryPlanView', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.planList = resp.data;
                }
            }).catch(function (err) { });
    }

    // TOASTR SETTINGS
    toastr.options.timeOut = 2000;
    toastr.options.progressBar = true;
    toastr.options.preventDuplicates = true;

    // ON LOAD EVENTS
    $scope.loadPlans();
});