app.controller('itemListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.itemList = [];
    $scope.multipleSelection = true;

    $scope.loadItemList = function () {
        $scope.itemList = [];

        $('#itemList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.itemList.length == 0)
                        $scope.itemList = $.getJSON(HOST_URL + 'Item/GetItemList', function (data) {
                                data.forEach(d => {
                                    d.IsChecked = false;
                                }
                            );
                        });

                    return $scope.itemList;
                },
                update: function (key, values) {
                    var obj = $scope.itemList.responseJSON.find(d => d.Id == key);
                    if (obj != null) {
                        obj.IsChecked = values.IsChecked;

                        if ($scope.multipleSelection == false && obj.IsChecked == true) {
                            $scope.itemList.responseJSON.forEach(d => {
                                if (d.Id != obj.Id)
                                    d.IsChecked = false;
                            })
                        }
                    }
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            allowColumnResizing: true,
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
                visible: false
            },
            editing: {
                allowUpdating: true,
                allowDeleting: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'ItemNo', caption: 'Stok Kodu', allowEditing: false },
                { dataField: 'ItemName', caption: 'Stok Adı', allowEditing: false },
                { dataField: 'ItemTypeStr', caption: 'Stok Türü' },
                { dataField: 'CategoryName', caption: 'Kategori' },
                { dataField: 'GroupName', caption: 'Grup' },
                { dataField: 'IsChecked', caption: 'Seç' }
            ]
        });
    }

    $scope.transferSelections = function () {
        if ($scope.itemList.responseJSON.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir stok seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedData = $scope.itemList.responseJSON.filter(d => d.IsChecked == true);

        var selectedDetails = selectedData;
        $scope.$emit('transferItems', selectedDetails);
    }

    // ON LOAD EVENTS
    $scope.$on('loadItemList', function (e, d) {
        if (typeof d != 'undefined' && d != null) {
            $scope.multipleSelection = d.multipleSelection;
        }

        $scope.loadItemList();
    });
});