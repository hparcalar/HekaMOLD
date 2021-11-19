app.controller('userProfileCtrl', function ($scope, $http, $interval) {
    $scope.loadProfileImage = function () {
        try {
            $http.get(HOST_URL + 'Common/GetProfileImage', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        var imageData = resp.data;
                        if (imageData != null && imageData.length > 0) {
                            $('#imgProfile1').attr('src', imageData);
                            $('#imgProfile2').attr('src', imageData);
                        }
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.loadProfileImage();
});