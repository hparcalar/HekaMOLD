app.controller('recipeRevisionsCtrl', function ($scope, $http) {
    $scope.recipeRevisionList = [];
    $scope.recipeProductId = 0;

    $scope.loadRecipeRevisionList = function () {
        try {
            $http.get(HOST_URL + 'ProductRecipe/GetRevisionsOfProduct?productId='
                + $scope.recipeProductId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.recipeRevisionList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.goToRevision = function (revisionId) {
        window.location.href = HOST_URL + 'ProductRecipe?rid=' + revisionId;
    }

    // ON LOAD EVENTS
    $scope.$on('loadRecipeRevisionList', function (e, d) {
        $scope.recipeProductId = d;
        $scope.loadRecipeRevisionList();
    });
});