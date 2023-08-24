app.controller('printItemsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#itemList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Item/GetItemList', function (data) {

                    });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            allowColumnResizing: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            wordWrapEnabled: true,
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
                { dataField: 'ItemNo', caption: 'Stok No' },
                { dataField: 'ItemName', caption: 'Stok Adı' },
                { dataField: 'ItemTypeStr', caption: 'Stok Türü' },
                { dataField: 'CategoryName', caption: 'Kategori' },
                { dataField: 'GroupName', caption: 'Grup' },
                { dataField: 'TotalInQuantity', caption: 'Giriş', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalOutQuantity', caption: 'Çıkış', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalOverallQuantity', caption: 'Toplam', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Yazdır', onClick: function (e) {
                                var dataGrid = $("#itemList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                $scope.showPrintLabelDialog(e.row.data.Id);
                            }
                        }
                    ]
                }
            ]
        });
    }

    $scope.showPrintLabelDialog = function (itemId) {
        $scope.$broadcast('showItemPrinting',
            itemId);

        $('#dial-printing').dialog({
            width: 500,
            height: 400,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
    
});