var app = angular.module('moldApp', []);
app.controller('notificationCtrl', function ($scope, $http, $interval) {
    $scope.notificationList = [];
    $scope.unSeenNotificationExists = function () {
        return $scope.notificationList.filter(d => d.SeenStatus == 0).length > 0;
    }

    $scope.updateNotifications = function () {
        try {
            $http.get(HOST_URL + 'Common/GetNotifications', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.notificationList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.openNotify = function (notifyItem) {
        if (notifyItem.NotifyType == 1 || notifyItem.NotifyType == 2) {
            window.location.href = HOST_URL + 'PIRequest?rid=' + notifyItem.RecordId;
        }
        else if (notifyItem.NotifyType == 3 || notifyItem.NotifyType == 4) {
            window.location.href = HOST_URL + 'PIOrder?rid=' + notifyItem.RecordId;
        }
    }

    // ON LOAD EVENTS
    $scope.updateNotifications();
    $interval($scope.updateNotifications, 5000);
});