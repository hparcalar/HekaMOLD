app.controller('notificationCtrl', function ($scope, $http, $interval) {
    $scope.notificationList = [];
    $scope.tempSeenNotifications = [];

    $scope.unSeenNotificationExists = function () {
        if (typeof $scope.notificationList != 'undefined'
            && $scope.notificationList != null) {
            const unseenCount = $scope.notificationList.filter(d => d.SeenStatus == 0).length > 0;
            if (unseenCount > 0)
                $scope.pushNotifications();

            return unseenCount;
        }
        return false;
    }
    $scope.lastSeenNotificationId = 0;

    $scope.pushNotifications = function () {
        try {
            var unseenData = $scope.notificationList.filter(d => d.SeenStatus == 0
                && (d.PushStatus == null || d.PushStatus == 0)
                && $scope.tempSeenNotifications.some(m => m == d.Id) == false);

            if (Push.Permission.has() == false) {
                Push.Permission.request(() => { }, () => { });
            }

            unseenData.forEach(d => {
                Push.create(d.Title, {
                    body: d.Message,
                    timeout: 4000,
                    onClick: function () {
                        $scope.setNotificationAsSeen(d.Id);
                        window.focus();
                        this.close();
                    }
                });

                $scope.tempSeenNotifications.push(d.Id);
                $scope.setNotificationAsPushed(d.Id);
            });
        } catch (e) {

        }
    }

    $scope.updateNotifyFlag = false;
    $scope.updateNotifications = function () {
        if ($scope.updateNotifyFlag == true)
            return;

        $scope.updateNotifyFlag = true;
        try {
            $http.get(HOST_URL + 'Common/GetNotifications', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.notificationList = resp.data;
                        setTimeout($scope.bindNotificationEvents, 300);
                        $scope.updateNotifyFlag = false;
                    }
                }).catch(function (err) {
                    $scope.updateNotifyFlag = false;
                });
        } catch (e) {
            $scope.updateNotifyFlag = false;
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

    $scope.setNotificationAsSeen = function (notificationId) {
        try {
            $http.post(HOST_URL + 'Common/SetNotifyAsSeen', { notificationId: notificationId }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        setTimeout($scope.bindNotificationEvents, 300);
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.setNotificationAsPushed = function (notificationId) {
        try {
            $http.post(HOST_URL + 'Common/SetNotifyAsPushed', { notificationId: notificationId }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // VISUAL EVENTS
    $scope.bindNotificationEvents = function () {
        $('.notification-item').hover(function (e) {
            if ($scope.lastSeenNotificationId != parseInt($(this).attr('data-id'))) {
                $scope.lastSeenNotificationId = parseInt($(this).attr('data-id'));
                $scope.setNotificationAsSeen($scope.lastSeenNotificationId);
            }
        });
    }
    
    // ON LOAD EVENTS
    $scope.updateNotifications();
    $interval($scope.updateNotifications, 5000);
});