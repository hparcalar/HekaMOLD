app.controller('transferBuyerFirmAddressCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');
    $scope.waitingBuyerFirmAddressList = [];
    $scope.selectedRow = {};

    $scope.modelObject = { Id: 0 };
    $scope.FirmId = 0;


    $scope.bindWaitingBuyerFirmAddressList = function (Id) {
        $('#waitingBuyerFirmAddressList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.waitingBuyerFirmAddressList.length == 0)
                        $scope.waitingBuyerFirmAddressList = $.getJSON(HOST_URL + 'Firm/GetFirmAddressList?Id=' + $scope.FirmId, function (data) {
                            data.forEach(d => {
                                d.IsChecked = false;
                            }
                            );
                        });
                    return $scope.waitingBuyerFirmAddressList;
                },
                update: function (key, values) {
                    var obj = $scope.waitingBuyerFirmAddressList.responseJSON.find(d => d.Id == key);
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
                { dataField: 'AddressName', caption: 'Adres İsmi' },
                //{dataField: 'CityId', caption: 'Şehir'},
                { dataField: 'CityName', caption: 'Şehir' },
                { dataField: 'CountryName', caption: 'Ülke' },
                { dataField: 'Address1', caption: 'Adres', width: 150 },
                //{ dataField: 'MobilePhone', caption: 'Mobil telefon', dataType: 'number' },
                //{ dataField: 'Fax', caption: 'Fax', dataType: 'number' },
                { dataField: 'OfficePhone', caption: 'İş Telefon', dataType: 'number' },
                { dataField: 'AuthorizedInfo', caption: 'Yetkili Kişi', width: 150 },
                { dataField: 'AddressTypeStr', caption: 'Adres Tipi' },
                { dataField: 'IsChecked', caption: 'Seç' }

            ]
        });
    }

    $scope.transferSelections = function () {
        if ($scope.waitingBuyerFirmAddressList.responseJSON.filter(d => d.IsChecked == true).length != 1) {
            toastr.warning('Aktarmak için bir adres seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedDetails = $scope.waitingBuyerFirmAddressList.responseJSON.filter(d => d.IsChecked == true);
        $scope.$emit('loadBuyerFirmAddressEnd', selectedDetails);
    }
    // ON LOAD EVENTS
    $scope.$on('loadBuyerFirmAddress', function (e, d) {
        $scope.FirmId = d.Id;
        $scope.waitingBuyerFirmAddressList = [];
        $scope.bindWaitingBuyerFirmAddressList();
    });
});