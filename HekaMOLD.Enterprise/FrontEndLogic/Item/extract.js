app.controller('itemExtractCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');
    $scope.modelObject = { Id: 0, ItemNo:'', ItemName:'' };

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Item/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });

        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Item/GetItemExtract?itemId=' + id, function (data) {

                    });
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
                enabled: true,
                pageSize: 13,
                pageIndex: 0
            },
            groupPanel: {
                visible: true
            },
            //scrolling: {
            //    mode: "virtual",
            //    columnRenderingMode: "virtual"
            //},
            //height: 550,
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
                //{
                //    type: "buttons",
                //    buttons: [
                //        {
                //            name: 'preview', cssClass: '', text: 'Göster', onClick: function (e) {
                //                var dataGrid = $("#dataList").dxDataGrid("instance");
                //                dataGrid.deselectAll();
                //                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                //                window.location.href = HOST_URL + 'ItemReceipt?rid=' + e.row.data.ItemReceiptId;
                //            }
                //        }
                //    ]
                //}
            ]
        });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
    else
        $scope.bindModel(0);
});
