app.controller('postureCategorySelectionCtrl', function ($scope, $http) {
    $scope.categoryList = [];

    $scope.loadPostureCategoryList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetPostureCategoryList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.categoryList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }
    
    $scope.categoryClick = function (machine) {
        $scope.$emit('postureCategorySelected', machine);
    }

    // ON LOAD EVENTS
    $scope.$on('loadPostureCategoryList', function (e, d) {
        $scope.loadPostureCategoryList();
    });
});