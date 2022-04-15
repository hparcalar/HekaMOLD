app.controller('offerSheetsList', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.sheetsList = [];

    $scope.selectedSheetRow = null;
    $scope.showItemList = function (row) {
        $scope.selectedSheetRow = row;

        $scope.$broadcast('loadItemList', null);

        $('#dial-items-list').dialog({
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

    $scope.$on('transferItems', function (e, d) {
        if (d != null && d.length > 0 && $scope.selectedSheetRow != null && $scope.selectedSheetRow.Id > 0) {
            $scope.selectedSheetRow.SheetItemId = d[0].Id;

            $http.post(HOST_URL + 'SIOffer/SaveSheetItem', $scope.selectedSheetRow, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            $scope.selectedSheetRow.SheetItemName = d[0].ItemName;
                            $scope.loadSheetList();
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });

            $('#dial-items-list').dialog('close');
        }
    });

    $scope.loadSheetList = function () {
        $('#sheetList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.sheetsList.sort((a,b) => a.SheetNo - b.SheetNo);
                },
                update: function (key, values) {
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
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
                visible: false
            },
            editing: {
                allowUpdating: true,
                allowDeleting: false,
                mode: 'cell'
            },
            onCellDblClick: function (args) {
                if (args.column.caption == 'Resim') {
                    var image = new Image();
                    image.src = "data:image/png;base64," + args.displayValue;
                    image.width = 800;

                    var w = window.open("");
                    w.document.write(image.outerHTML);
                }
            },
            columns: [
                { dataField: 'SheetNo', caption: 'Şablon No', allowEditing: false },
                {
                    dataField: 'SheetVisualStr', caption: 'Resim', allowEditing: false,
                    cellTemplate: function (element, info) {

                        if (info.displayValue != null && info.displayValue.length > 0)
                            element.append('<image src="data:image/x-wmf;base64,' + info.displayValue + '" />');
                    }
                },
                { dataField: 'SheetItemName', caption: 'Stok', allowEditing: false },
                { dataField: 'SheetHeight', caption: 'Boy', allowEditing: false },
                { dataField: 'SheetWidth', caption: 'En', allowEditing: false },
                { dataField: 'Thickness', caption: 'Kalınlık', allowEditing: false },
                { dataField: 'Quantity', caption: 'Adet', allowEditing: false },
                { dataField: 'Eff', caption: 'Verimlilik', allowEditing: false },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'selectSheetItem', text: 'Stok Seç', onClick: function (e) {
                                $scope.showItemList(e.row.data);
                            }
                        },
                    ]
                }
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.$on('loadSheetsList', function (e, d) {
        $scope.sheetsList = d;
        $scope.loadSheetList();
    });
});