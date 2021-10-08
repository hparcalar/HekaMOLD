app.controller('printOptionsCtrl', function ($scope, $http) {
    $scope.printerList = [];
    $scope.selectedPrinter = { Id:0 };
    $scope.printCount = 1;

    $scope.loadPrinterList = function () {
        try {
            $http.get(HOST_URL + 'Common/GetPrinterList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.printerList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.approvePrinterOptions = function () {
        if ($scope.selectedPrinter == null || $scope.selectedPrinter.Id <= 0) {
            toastr.error('Lütfen bir yazıcı seçin.', 'Uyarı');
            return;
        }

        if ($scope.printCount <= 0) {
            toastr.error('Etiket adedi giriniz.', 'Uyarı');
            return;
        }

        $scope.$emit('printOptionsApproved', { PrinterId: $scope.selectedPrinter.Id, PrintCount: $scope.printCount });
    }

    $scope.cancelPrinting = function () {
        $scope.$emit('cancelPrinting');
    }

    // ON LOAD EVENTS
    $scope.$on('showPrintOptions', function (e, d) {
        $scope.loadPrinterList();
    });
});