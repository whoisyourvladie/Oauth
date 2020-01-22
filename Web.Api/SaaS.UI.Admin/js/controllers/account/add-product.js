(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountAddProductController', controller);

    controller.$inject = ['$rootScope', '$scope', '$api', '$form', '$dict'];

    function controller($rootScope, $scope, $api, $form, $dict) {

        $scope.model = {
            products: null,
            price: 0,
            priceUsd: 0,
            quantity: 1,
            account: null
        };
        $scope.currencies = $dict.currencies;
        $scope.model.currency = 'USD';

        var _productsUrl = null;

        $scope.init = function (productsUrl) {

            _productsUrl = productsUrl;
        };
        $scope.submit = function (form) {

            $form.submit($scope, form, function (form) {
                
                var json = {
                    productUid: $scope.model.product.productUid,
                    price: $scope.model.price,
                    priceUsd: $scope.model.priceUsd,
                    quantity: $scope.model.quantity,
                    currency: $scope.model.currency,
                };
                return $api.account.addProduct($scope.model.account.id, json).then(function (json) {

                    window.location = _productsUrl;

                }, function () {

                    $scope.isBusy = false;
                });
            });
        };

        (function () {

            $api.upclick.products().then(function (json) {

                var products = [];
                for (var index = 0; index < json.length; index++) {

                    var item = json[index];
                    (!item.productUid.indexOf('P0') || !item.productUid.indexOf('T0')) && item.productName && products.push(item);
                }

                if (products.length)
                    $scope.model.product = products[0];

                $scope.model.products = products;
            });
        }());

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.id = json.id;
        });
        $rootScope.$on('event:accountDeleted', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.isDeleted = true;
        });
    };
})();