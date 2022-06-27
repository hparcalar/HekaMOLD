app.controller('receiptListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.receiptCategory = 0;
    $scope.receiptType = 0;

    $scope.selectedReceiptType = {Id:0,Text:'Tümü'};
    $scope.receiptTypeList = [];

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL
                        + 'ItemReceipt/GetReceiptList?receiptCategory=' + $scope.receiptCategory
                        + '&receiptType=' + $scope.receiptType, function (data) {
                            
                        });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                enabled:true,
                pageSize: 13,
                pageIndex:0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'ReceiptNo', caption: 'İrsaliye No' },
                { dataField: 'ReceiptTypeStr', caption: 'Hareket Türü' },
                { dataField: 'DocumentNo', caption: 'Belge No' },
                { dataField: 'ReceiptDateStr', caption: 'Tarih' },
                { dataField: 'FirmCode', caption: 'Firma Kodu' },
                { dataField: 'FirmName', caption: 'Firma Adı' },
                { dataField: 'ReceiverPlantName', caption: 'İşletme Adı' },
                { dataField: 'ReceiptStatusStr', caption: 'Durum' },
                { dataField: 'Explanation', caption: 'Açıklama' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'ItemReceipt?rid=' + e.row.data.Id
                                    + '&receiptCategory=' + $scope.receiptCategory;
                            }
                        }
                    ]
                }
            ]
            });
    }

    $scope.bindParameters = function () {
        $scope.receiptCategory = getParameterByName('receiptCategory');
        $scope.receiptType = getParameterByName('receiptType');
        if ($scope.receiptType == null)
            $scope.receiptType = 0;
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ItemReceipt/GetReceiptTypes?receiptCategory='
                + $scope.receiptCategory, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.receiptTypeList
                            .splice(0, $scope.receiptTypeList.length - 1);

                        $scope.receiptTypeList.push({ Id: 0, Text: 'Tümü' });

                        $scope.selectedReceiptType = $scope
                            .receiptTypeList.find(d => d.Id == 0);

                        resp.data.ReceiptTypes.forEach(d => {
                            $scope.receiptTypeList.push(d);
                        });

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.onReceiptTypeChanged = function (e) {
        if (typeof $scope.selectedReceiptType != 'undefined'
            && $scope.selectedReceiptType != null
            && typeof $scope.selectedReceiptType.Id != 'undefined')
            $scope.receiptType = $scope.selectedReceiptType.Id;
        else
            $scope.receiptType = 0;

        $scope.loadReport();
    }

    // ON LOAD EVENTS
    $scope.bindParameters();
    $scope.loadSelectables().then(function () {
        $scope.loadReport();
    });
});
