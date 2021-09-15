app.controller('incidentCategorySelectionCtrl', function ($scope, $http) {
    $scope.categoryList = [];

    $scope.loadIncidentCategoryList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetIncidentCategoryList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.categoryList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.categoryClick = function (machine) {
        $scope.$emit('incidentCategorySelected', machine);
    }

    // ON LOAD EVENTS
    $scope.$on('loadIncidentCategoryList', function (e, d) {
        $scope.loadIncidentCategoryList();
    });
});