(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountInfoController', controller);

    controller.$inject = ['$rootScope', '$scope', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $notify, $api, $form) {

        $scope.model = { account: null };

        $scope.isActivated = function () {

            if ($scope.model.account)
                return $api.account.isActivated($scope.model.account);
        };
        $scope.isBusiness = function () {

            if ($scope.model.account)
                return $api.account.isBusiness($scope.model.account);
        };

        $scope.activate = function (build) {

            $api.account.confirmEmail($scope.model.account.id, { build: build }).then(function (json) {

                if (build == "b2c")
                    $notify.info("Customer has been activated.");

                if (build == "b2b")
                    $notify.info("Customer has been marked as business.");

                $rootScope.$broadcast('event:accountLoaded', json);
            });
        };
        $scope.submit = function (form) {

            $form.submit($scope, form, function (form) {

                $api.account.set($scope.model.account.id, $scope.model.account).then(function (json) {

                    $notify.info("Customer's information has been updated.");
                    $rootScope.$broadcast('event:accountLoaded', json);
                }).finally(function () {

                    $scope.isBusy = false;
                });
            });
        };

        $rootScope.$on('event:accountLoaded', function (event, json) {
            
            $scope.model.account = json;
        });
        $rootScope.$on('event:accountDeleted', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.isDeleted = true;
        });
    };
})();