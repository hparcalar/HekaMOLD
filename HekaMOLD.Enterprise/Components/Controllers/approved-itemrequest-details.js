app.controller('requestListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.approvedRequestDetailList = [];

    $scope.loadRequestDetails = function () {
        $scope.approvedRequestDetailList = [];

        $('#requestList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.approvedRequestDetailList.length == 0)
                        $scope.approvedRequestDetailList = $.getJSON(HOST_URL + 'PIOrder/GetApprovedRequestDetails', function (data) {
                            data.forEach(d => {
                                d.CreatedDate = moment(parseInt(d.CreatedDate.toString().substr(6, d.CreatedDate.toString().length - 8)));
                                d.RequestDate = moment(parseInt(d.RequestDate.toString().substr(6, d.RequestDate.toString().length - 8)));
                                d.IsChecked = false;
                            }
                            );
                        });

                    return $scope.approvedRequestDetailList;
                },
                update: function (key, values) {
                    var obj = $scope.approvedRequestDetailList.responseJSON.find(d => d.Id == key);
                    if (obj != null) {
                        obj.IsChecked = values.IsChecked;
                    }
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
                visible: false
            },
            editing: {
                allowUpdating: true,
                allowDeleting: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'RequestNo', caption: 'Talep No', allowEditing: false },
                { dataField: 'CreatedDate', caption: 'Talep Tarihi', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'RequestDate', caption: 'İhtiyaç Tarihi', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'CreatedUserStr', caption: 'Talep Eden', allowEditing: false },
                { dataField: 'ItemNo', caption: 'Stok No', allowEditing: false },
                { dataField: 'ItemName', caption: 'Stok Adı', allowEditing: false },
                { dataField: 'UnitCode', caption: 'Birim', allowEditing: false },
                { dataField: 'Quantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'RequestExplanation', caption: 'Açıklama', allowEditing: false },
                { dataField: 'IsChecked', caption: 'Seç' }
            ]
        });
    }

    $scope.transferSelections = function () {
        if ($scope.approvedRequestDetailList.responseJSON.filter(d => d.IsChecked == true).length == 0) {
            alert('Aktarmak için bir veya daha fazla kayıt seçmelisiniz.');
            return;
        }

        var selectedDetails = $scope.approvedRequestDetailList.responseJSON.filter(d => d.IsChecked == true);
        $scope.$emit('transferRequestDetails', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadApprovedRequestDetails', function (e, d) {
        $scope.loadRequestDetails();
    });
});