app.controller('moldListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Mold/GetMoldList', function (data) {
                            
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
            height:400,
            paging: {
                enabled:false,
                pageSize: 10,
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
                { dataField: 'MoldCode', caption: 'Kalıp Kodu' },
                { dataField: 'MoldName', caption: 'Kalıp Adı' },
                { dataField: 'FirmName', caption: 'Sahibi' },
                { dataField: 'OwnedDateStr', caption: 'Giriş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LifeTimeTicks', caption: 'Kalıp Ömrü' },
                { dataField: 'CurrentTicks', caption: 'Mevcut Baskı' },
                { dataField: 'IsActive', caption:'Aktif' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Mold?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ]
            });
        }

    $scope.printingMoldList = [];

    $scope.showPrintDialog = function () {
        try {
            let gridInstance = $("#dataList").dxDataGrid("instance");
            const filterExpr = gridInstance.getCombinedFilter(true);
            gridInstance
                .getDataSource()
                .load({ filter: filterExpr })
                .then((result) => {
                    $scope.printingMoldList = result;

                    $('#dial-print-label').dialog({
                        width: 1100,
                        height: window.innerHeight * 0.6,
                        hide: true,
                        modal: true,
                        resizable: false,
                        show: true,
                        draggable: false,
                        closeText: "KAPAT"
                    });
                });
        } catch (e) {

        }
    }

    $scope.printLabel = function () {
        var printContents = document.getElementById('mold-label').innerHTML;
        var originalContents = document.body.innerHTML;
        document.body.innerHTML = printContents;
        window.print();
        document.body.innerHTML = originalContents;
        window.location.reload();
    }

    $scope.printingQueue = [];
    $scope.printEnd = false;

    $scope.afterPrintQueue = function () {
        if ($scope.printingQueue.length > 0)
            $scope.printingQueue.splice(0, 1);
        $scope.printEnd = true;
    }

    $scope.printLabelAsSingle = async function () {
        $scope.printingQueue = $scope.printingMoldList;

        window.onafterprint = $scope.afterPrintQueue;
        $scope.printEnd = true;

        while ($scope.printingQueue.length > 0 && $scope.printEnd == true) {
            let d = $scope.printingQueue[0];

            $scope.printingMoldList.splice(0, $scope.printingMoldList.length);
            $scope.printingMoldList.push(d);

            $scope.printEnd = false;

            var prms = new Promise(function (resolve, reject) {
                var printContents = document.getElementById('mold-label').innerHTML;
                var originalContents = document.body.innerHTML;
                document.body.innerHTML = printContents;
                window.print();
                document.body.innerHTML = originalContents;
            });


        }

        //window.location.reload();
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
