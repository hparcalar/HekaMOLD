app.controller('notificationDetailsCtrl', function ($scope, $http) {
    $scope.modelObject = { CurrentDate: moment().format('DD.MM.YYYY'), NotificationList: [] };

    $scope.loadData = function () {
        try {
            $http.get(HOST_URL + 'Common/GetAllNotifications', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.NotificationList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.openData = function (notifyItem) {
        if (notifyItem.NotifyType == 1 || notifyItem.NotifyType == 2) {
            window.location.href = HOST_URL + 'PIRequest?rid=' + notifyItem.RecordId;
        }
    }

    // ON LOAD EVENTS
    $scope.loadData();
});