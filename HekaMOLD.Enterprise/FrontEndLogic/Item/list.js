﻿app.controller('itemListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
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
            showBorders: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            }, onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Mal Listesi');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Mal Listesi.xlsx');
                    });
                });
                e.cancel = true;
            },
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
                { dataField: 'ItemNo', caption: 'Mal No' },
                { dataField: 'ItemName', caption: 'Mal Adı' },
                { dataField: 'ItemTypeStr', caption: 'Mal Türü' },
                { dataField: 'CategoryName', caption: 'Kategori' },
                { dataField: 'GroupName', caption: 'Grup' },
                { dataField: 'TotalInQuantity', caption: 'Giriş', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalOutQuantity', caption: 'Çıkış', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalOverallQuantity', caption: 'Toplam', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Item?rid=' + e.row.data.Id;
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
