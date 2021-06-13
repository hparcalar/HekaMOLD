app.controller('itemEntryListCtrl', function ($scope, $http) {
    $scope.dataList = [];

    $scope.loadItemEntries = function () {
        $http.get(HOST_URL + 'Mobile/GetItemEntryList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.dataList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.showItemEntry = function (id) {
        window.location.href = HOST_URL + 'Mobile/ItemEntry?rid=' +
            id;
    }

    // LOAD EVENTS
    $scope.loadItemEntries();
});