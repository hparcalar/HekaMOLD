app.controller('documentPreviewCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Document/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $('#preview-panel').append($scope.modelObject.DocumentData);
                }

                //ClassicEditor
                //    .create(document.querySelector('#editor'))
                //    .then(nEditor => {
                //        $scope.docEditor = nEditor;
                //    })
                //    .catch(error => {
                //        console.error(error);
                //    });
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.docEditor = null;

    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});