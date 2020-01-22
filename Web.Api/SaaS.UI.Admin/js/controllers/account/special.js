(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountSpecialController', controller);

    controller.$inject = ['$rootScope', '$scope', '$api', 'special', 'appSettings'];

    function controller($rootScope, $scope, $api, special, appSettings) {

        $scope.model = {
            account: null,
            tracking: angular.copy(special.tracking),
            products: angular.copy(special.products)
        };

        Object.defineProperty($scope.model, 'link', {
            get: function () {

                $scope.model.tracking.ujId = $scope.model.discount.ujId;

                var tracking = special.url;
                tracking += '?' + $.param($scope.model.tracking);

                return tracking;
            }
        });


        $scope.model.product = $scope.model.products[0];

        $scope.isShowDiscount = function () {

            var discounts = $scope.model.product.discounts;

            return !(discounts.length == 1 && discounts[0].name == '0%');
        };
        $scope.productChange = function () {

            $scope.model.discount = $scope.model.product.discounts[0];
        };
        $scope.productChange();

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.id = json.id;
            $scope.model.tracking.email = json.email;

            json.firstName && ($scope.model.tracking.fName = json.firstName);
            json.lastName && ($scope.model.tracking.lName = json.lastName);

            json.id && $api.account.uid(json.id).then(function (json) {

                json.uid && ($scope.model.tracking.uid = json.uid);
            });
        });
        $rootScope.$on('event:accountDeleted', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.isDeleted = true;
        });
    };
})();