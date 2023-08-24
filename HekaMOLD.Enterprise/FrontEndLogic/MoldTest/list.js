app.controller('moldTestListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'MoldTest/GetMoldTestList', function (data) {
                            
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
                { dataField: 'TestDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'MoldCode', caption: 'Kalıp Kodu' },
                { dataField: 'MoldName', caption: 'Kalıp Adı' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                { dataField: 'ProductDescription', caption: 'Ürün Açıklama' },
                { dataField: 'RawMaterialName', caption: 'Hammadde' },
                { dataField: 'DyeCode', caption: 'Renk Kodu' },
                { dataField: 'RalCode', caption: 'Ral Kodu' },
                { dataField: 'InflationTimeSeconds', caption: 'Şişirme Zamanı' },
                { dataField: 'InPackageQuantity', caption: 'Kutu İçi' },
                { dataField: 'PackageDimension', caption: 'Koli Boyutu' },
                { dataField: 'RawMaterialGr', caption: 'Hammadde Gr' },
                { dataField: 'DyeCode', caption: 'Boya Kodu' },
                { dataField: 'HeadSize', caption: 'Kafa Ölçüsü' },
                { dataField: 'NutCaliber', caption: 'Somun Tipi' },
                { dataField: 'NutQuantity', caption: 'Somun Adedi' },
                { dataField: 'InPalletPackageQuantity', caption: 'Palet içi koli' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'MoldTest?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ]
            });
        }

    // ON LOAD EVENTS
    $scope.loadReport();
});
