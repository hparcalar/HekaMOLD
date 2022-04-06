app.controller('offerSheetUsages', function ($scope, $http, $timeout) {
    DevExpress.localization.locale('tr');

    $scope.usageList = [];
    $scope.partName = '';

    $scope.loadUsages = function () {
        try {
            $scope.partName = $scope.usageList[0].PartName;
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadUsages', function (e, d) {
        $timeout(function () {
            $scope.usageList = d;
            $scope.loadUsages();
        });

        try {
            $scope.$applyAsync();
        } catch (e) {

        }
    });
});