app.controller('receiptSerialsCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = {
        Quantity: 0,
        PrintLabel: true,
        PrinterId: 0,
    };

    $scope.itemReceiptDetailId = 0;
    $scope.serialList = [];

    $scope.addNewSerial = function () {
        if ($scope.modelObject.Quantity > 0) {
            $http.post(HOST_URL + 'ItemReceipt/SaveItemEntry', {
                itemReceiptDetailId: $scope.itemReceiptDetailId,
                inPackageQuantity: $scope.modelObject.Quantity,
                printLabel: $scope.modelObject.PrintLabel,
                printerId: $scope.modelObject.PrinterId,
            }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        if (resp.data.Result == true) {
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');

                            $scope.bindSerials();
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });
        }
    }

    $scope.getDefaultSerialPrinter = function () {
        try {
            $http.get(HOST_URL + 'Common/GetDefaultSerialPrinter', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.PrinterId = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.bindSerials = function () {
        $timeout(function () {
            $http.get(HOST_URL + 'ItemReceipt/GetSerialsOfDetail?rid=' + $scope.itemReceiptDetailId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.serialList = resp.data;

                        $('#serialList').dxDataGrid({
                            dataSource: {
                                load: function () {
                                    return $scope.serialList;
                                },
                                key: 'Id'
                            },
                            showColumnLines: false,
                            showRowLines: true,
                            rowAlternationEnabled: true,
                            focusedRowEnabled: true,
                            showBorders: true,
                            filterRow: {
                                visible: true
                            },
                            headerFilter: {
                                visible: true
                            },
                            paging: {
                                enabled: false,
                                pageSize: 13,
                                pageIndex: 0
                            },
                            groupPanel: {
                                visible: false
                            },
                            editing: {
                                allowUpdating: false,
                                allowDeleting: false,
                                mode: 'cell'
                            },
                            columns: [
                                { dataField: 'CreatedDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                                { dataField: 'SerialNo', caption: 'Barkod', allowEditing: false },
                                { dataField: 'FirstQuantity', caption: 'Koli İçi Adet' },
                            ]
                        });
                    }
                }).catch(function (err) { });
        });

        $scope.$applyAsync();
    }

    // ON LOAD EVENTS
    $scope.$on('loadReceiptSerials', function (e, d) {
        $scope.getDefaultSerialPrinter();
        $scope.itemReceiptDetailId = d;
        $scope.bindSerials();
    });
});