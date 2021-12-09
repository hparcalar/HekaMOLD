app.controller('finishedProductsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Warehouse/GetFinishedProducts', function (data) {
                            
                        });
                },
                key: ['ItemId']
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
            height:500,
            //paging: {
            //    enabled:true,
            //    pageSize: 13,
            //    pageIndex:0
            //},
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                /*{ dataField: 'ItemNo', caption: 'Ürün Kodu' },*/
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                { dataField: 'ItemGroupName', caption: 'Grup' },
                //{ dataField: 'InQty', caption: 'Giriş Miktar' },
                //{ dataField: 'OutQty', caption: 'Çıkış Miktar' },
                { dataField: 'TotalQty', caption: 'Mevcut Miktar' },
                //{
                //    type: "buttons",
                //    buttons: [
                //        {
                //            name: 'preview', cssClass: 'btn btn-sm btn-light-primary', text: 'Seriler', onClick: function (e) {
                //                var dataGrid = $("#dataList").dxDataGrid("instance");
                                
                //            }
                //        }
                //    ]
                //}
            ],
            summary: {
                totalItems: [{
                    column: "InQty",
                    summaryType: "sum",
                }, {
                    column: "OutQty",
                    summaryType: "sum",
                },
                {
                    column: "TotalQty",
                    summaryType: "sum",
                }
                ]
            }
            });
        }

    // ON LOAD EVENTS
    $scope.loadReport();
});
