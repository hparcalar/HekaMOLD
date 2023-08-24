app.controller('finishedProductsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');
    $scope.dataList = [];

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $http.get(HOST_URL + 'Warehouse/GetFinishedProducts', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.dataList = resp.data;

                    $('#dataList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.dataList;
                            },
                            key: ['ItemId']
                        },
                        showColumnLines: false,
                        showRowLines: true,
                        rowAlternationEnabled: true,
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        focusedRowEnabled: true,
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
                            enabled: false,
                            pageSize: 13,
                            pageIndex: 0
                        },
                        scrolling: {
                            mode: "virtual",
                            columnRenderingMode: "virtual"
                        },
                        height: parseInt($('body').height() * 0.6),
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
            }).catch(function (err) { });

       
    }

    // ON LOAD EVENTS
    angular.element(document).ready(function () {
        $scope.loadReport();

    });
});
