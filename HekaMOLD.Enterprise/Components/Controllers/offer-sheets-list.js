app.controller('offerSheetsList', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.sheetsList = [];

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
                { dataField: 'Thickness', caption: 'Kalınlık', allowEditing: false },
                { dataField: 'Quantity', caption: 'Adet', allowEditing: false },
                { dataField: 'Eff', caption: 'Verimlilik', allowEditing: false },
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.$on('loadSheetsList', function (e, d) {
        $scope.sheetsList = d;
        $scope.loadSheetList();
    });
});