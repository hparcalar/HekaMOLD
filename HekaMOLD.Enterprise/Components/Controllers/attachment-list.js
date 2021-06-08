app.controller('attachmentsCtrl', ['$scope', '$http', 'Upload',
    function ($scope, $http, Upload) {
        $scope.attachmentList = [];

        $scope.selectedFile = null;
        $scope.selectedDescription = '';

        $scope.recordId = 0;
        $scope.recordType = 0;

        $scope.uploadAttachment = function () {
            if ($scope.selectedFile) {
                if (typeof $scope.selectedDescription == 'undefined' ||
                    $scope.selectedDescription == null ||
                    $scope.selectedDescription.length == 0) {
                    toastr.error('Açıklama girmelisiniz.', 'Uyarı');
                    return;
                }

                Upload.upload({
                    url: HOST_URL + 'Common/AddAttachment',
                    data: {
                        file: $scope.selectedFile,
                        description: $scope.selectedDescription,
                        recordId: $scope.recordId,
                        recordType: $scope.recordType
                    }
                }).then(function (resp) {
                    $scope.loadAttachmentList();
                    //console.log('Success ' + resp.config.data.file.name
                    //    + 'uploaded. Response: ' + resp.data);
                }, function (resp) {
                    //console.log('Error status: ' + resp.status);
                }, function (evt) {
                    //var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    //console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
                });
            }
        }

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
    }
]);