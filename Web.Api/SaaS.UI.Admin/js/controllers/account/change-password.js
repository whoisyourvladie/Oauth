(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountChangePasswordController', controller);

    controller.$inject = ['$rootScope', '$scope', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $notify, $api, $form) {

        $scope.model = { account: null };

        $scope.recoverPassword = function () {

            $api.account.recoverPassword($scope.model.account.id).then(function (json) {

                $notify.info("An email has been sent to customer to reset your password.");
            });
        };
        $scope.submit = function (form) {

            $form.submit($scope, form, function (form) {

                $api.account.changePassword($scope.model.account.id, { newPassword: $scope.model.account.password }).then(function (json) {

                    $notify.info("Customer's password has been changed.");
                }).finally(function () {

                    $scope.isBusy = false;
                });
            });
        };

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