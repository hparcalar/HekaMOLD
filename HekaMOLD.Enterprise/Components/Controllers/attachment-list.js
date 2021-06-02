app.controller('attachmentsCtrl', function ($scope, $http) {
    $scope.attachmentList = [];

    $scope.recordId = 0;
    $scope.recordType = 0;

    $scope.loadAttachmentList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetAttachments?recordId=' + $scope.recordId
                + '&recordType=' + $scope.recordType, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.attachmentList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.showAttachment = function (attachmentId) {
        window.open(HOST_URL + 'Common/ShowAttachment?attachmentId='
            + attachmentId, '_blank');
    }

    // ON LOAD EVENTS
    $scope.$on('showAttachmentList', function (e, d) {
        $scope.recordId = d.RecordId;
        $scope.recordType = d.RecordType;

        $scope.loadAttachmentList();
    });
});