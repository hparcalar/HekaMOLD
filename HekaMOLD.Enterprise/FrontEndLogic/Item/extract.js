app.controller('itemExtractCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');
    $scope.modelObject = { Id: 0, ItemNo: '', ItemName: '' };
    $scope.extractList = [];

    $scope.bindModel = function (id) {
        // GET ITEM INFORMATION
        $http.get(HOST_URL + 'Item/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // GET EXTRACT DETAILS
                    $http.get(HOST_URL + 'Item/GetItemExtract?itemId=' + id, {}, 'json')
                        .then(function (respExtract) {
                            if (typeof respExtract.data != 'undefined' && respExtract.data != null) {
                                $scope.extractList = respExtract.data;

                                $('#dataList').dxDataGrid({
                                    dataSource: {
                                        load: function () {
                                            return $scope.extractList;
                                        },
                                        key: 'Id'
                                    },
                                    showColumnLines: false,
                                    showRowLines: true,
                                    rowAlternationEnabled: true,
                                    focusedRowEnabled: true,
                                    showBorders: true,
                                    export: {
                                        enabled: true,
                                        allowExportSelectedData: true,
                                    },
                                    allowColumnResizing: true,
                                    wordWrapEnabled: true,
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
                                        visible: true
                                    },
                                    scrolling: {
                                        mode: "virtual",
                                        columnRenderingMode: "virtual"
                                    },
                                    height: $('#tableContainer').height(),
                                    //columnWidth: 150,
                                    editing: {
                                        allowUpdating: false,
                                        allowDeleting: false
                                    },
                                    columns: [
                                        { dataField: 'ReceiptDateStr', caption: 'Tarih' },
                                        { dataField: 'ReceiptTypeStr', caption: 'Hareket Türü' },
                                        { dataField: 'WarehouseName', caption: 'Depo' },
                                        { dataField: 'FirmName', caption: 'Firma' },
                                        { dataField: 'InQuantity', caption: 'Giriş Miktar', format: { type: "fixedPoint", precision: 2 } },
                                        { dataField: 'OutQuantity', caption: 'Çıkış Miktar', format: { type: "fixedPoint", precision: 2 } },
                                        {
                                            type: "buttons",
                                            buttons: [
                                                {
                                                    name: 'preview', cssClass: '', text: 'Göster', onClick: function (e) {
                                                        var dataGrid = $("#dataList").dxDataGrid("instance");
                                                        dataGrid.deselectAll();
                                                        dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                                        window.location.href = HOST_URL + 'ItemReceipt?rid=' + e.row.data.ItemReceiptId;
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                });
                            }
                        }).catch(function (errExtract) { });
                }
            }).catch(function (err) { });
    }

    $scope.getItemLabel = function () {
        if ($scope.modelObject.Id > 0)
            return $scope.modelObject.ItemName;
        else
            return '';
    }

    $scope.showItemDialog = function () {
        $scope.$broadcast('loadItemList', { multipleSelection: false });

        $('#dial-items').dialog({
            width: window.innerWidth * 0.8,
            height: window.innerHeight * 0.8,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.getInQty = function () {
        try {
            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',
                
            });

            const numberData = $scope.extractList.reduce(function (acc, obj) { return acc + (obj.InQuantity ?? 0); }, 0);

            return formatter.format(numberData);
        } catch (e) {
            return 0;
        }
    }

    $scope.getOutQty = function () {
        try {
            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',

            });

            const numberData = $scope.extractList.reduce(function (acc, obj) { return acc + (obj.OutQuantity ?? 0); }, 0);

            return formatter.format(numberData);
        } catch (e) {
            return 0;
        }
    }

    $scope.getTotalQty = function () {
        try {
            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',

            });

            const numberData = $scope.extractList.reduce(function (acc, obj) { return acc + (obj.InQuantity ?? 0) - (obj.OutQuantity ?? 0); }, 0);

            return formatter.format(numberData);
        } catch (e) {
            return 0;
        }
    }

    $scope.$on('transferItems', function (e, data) {
        if (data != null && data.length > 0) {
            $scope.modelObject.Id = data[0].Id;
            $scope.modelObject.ItemNo = data[0].ItemNo;
            $scope.modelObject.ItemName = data[0].ItemName;

            $scope.bindModel($scope.modelObject.Id);
        }

        $('#dial-items').dialog('close');
    });

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
    else
        $scope.bindModel(0);
});
