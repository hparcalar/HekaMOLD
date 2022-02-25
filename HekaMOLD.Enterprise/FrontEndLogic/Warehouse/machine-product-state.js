app.controller('machineProductStateCtrl', function ($scope, $http, $timeout) {
    DevExpress.localization.locale('tr');

    $scope.filterModel = {
        startDate: moment().format('DD.MM.YYYY'),
        endDate: moment().format('DD.MM.YYYY'),
    };

    $scope.mainData = [];
    $scope.currentFilter = null;

    $scope.updateConsumeData = function () {
        var prms = new Promise((resolve, reject) => {
            try {
                var filterExpr = $("#dataList").dxDataGrid("instance").getCombinedFilter(true);

                var consumePrm = $scope.mainData;

                if (typeof filterExpr != 'undefined' && filterExpr != null)
                    consumePrm = DevExpress.data.query($scope.mainData)
                        .filter(filterExpr).toArray();

                if (consumePrm == null || consumePrm.length == 0) {
                    resolve([]);
                    return;
                }

                $http.post(HOST_URL + 'Warehouse/GetConsumedRecipeData', consumePrm, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            resolve(resp.data);
                        }
                        else
                            resolve([]);
                    }).catch(function (err) { resolve([]); });
                
            } catch (e) {

            }
            
        });

        return prms;
    }

    $scope.consumeData = [];
    $scope.onChangeReport = function () {
        $scope.updateConsumeData().then(function (data) {
            
            $timeout(function () {
                $scope.consumeData = data;

                $('#rawItemList').dxDataGrid({
                    dataSource: {
                        load: function () {
                            return $scope.consumeData;
                        },
                        key: ['ItemId']
                    },
                    showColumnLines: false,
                    showRowLines: true,
                    rowAlternationEnabled: true,
                    export: {
                        enabled: true,
                        allowExportSelectedData: true,
                    },
                    allowColumnResizing: true,
                    wordWrapEnabled: true,
                    focusedRowEnabled: true,
                    showBorders: true,
                    filterRow: {
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
                    headerFilter: {
                        visible: true
                    },
                    height: 500,
                    groupPanel: {
                        visible: true
                    },
                    editing: {
                        allowUpdating: false,
                        allowDeleting: false
                    },
                    columns: [
                        /*{ dataField: 'ItemNo', caption: 'Ürün Kodu' },*/
                        { dataField: 'ItemName', caption: 'Stok Adı' },
                        //{ dataField: 'InQty', caption: 'Giriş Miktar' },
                        //{ dataField: 'OutQty', caption: 'Çıkış Miktar' },
                        {
                            dataField: 'TotalQty', caption: 'Miktar', dataType: 'number',
                            format: { type: "fixedPoint", precision: 2 }, },
                    ],
                    
                });
            });
            

            try {
                $scope.$applyAsync();
            } catch (e) {

            }
        });
    }

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $http.get(HOST_URL + 'Warehouse/GetMachineProductState?dt1=' + $scope.filterModel.startDate +
            '&dt2=' + $scope.filterModel.endDate, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.mainData = resp.data;

                    $('#dataList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.mainData;
                            },
                            key: ['ItemId']
                        },
                        showColumnLines: false,
                        showRowLines: true,
                        rowAlternationEnabled: true,
                        export: {
                            enabled: true,
                            allowExportSelectedData: true,
                        },
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        focusedRowEnabled: true,
                        showBorders: true,
                        paging: {
                            enabled: false,
                            pageSize: 13,
                            pageIndex: 0
                        },
                        scrolling: {
                            mode: "virtual",
                            columnRenderingMode: "virtual"
                        },
                        filterRow: {
                            visible: true
                        },
                        headerFilter: {
                            visible: true
                        },
                        height: 500,
                        groupPanel: {
                            visible: true
                        },
                        editing: {
                            allowUpdating: false,
                            allowDeleting: false
                        },
                        onContentReady: function () {
                            $scope.onChangeReport();
                        },
                        columns: [
                            /*{ dataField: 'ItemNo', caption: 'Ürün Kodu' },*/
                            { dataField: 'ItemName', caption: 'Ürün Adı' },
                            { dataField: 'ItemGroupName', caption: 'Grup' },
                            { dataField: 'MachineCode', caption: 'Makine' },
                            //{ dataField: 'InQty', caption: 'Giriş Miktar' },
                            //{ dataField: 'OutQty', caption: 'Çıkış Miktar' },
                            { dataField: 'TotalQty', caption: 'Miktar' },
                        ],
                        summary: {
                            totalItems: [
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
    $scope.loadReport();
});
