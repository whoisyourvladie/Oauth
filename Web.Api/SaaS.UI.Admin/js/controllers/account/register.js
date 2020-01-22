(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountRegisterController', controller);

    controller.$inject = ['$rootScope', '$scope', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $notify, $api, $form) {

        $scope.model = { account: null };

        $scope.submit = function (form) {

            $form.submit($scope, form, function (form) {

                $api.account.register($scope.model.account).then(function (json) {

                    $scope.model.account = json;
                    $notify.info("Customer password has been created.");
                }, function () { $scope.isBusy = false; });
            });
        };
    };
})();