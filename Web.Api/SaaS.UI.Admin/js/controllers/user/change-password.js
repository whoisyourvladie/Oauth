(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('userChangePasswordController', controller);

    controller.$inject = ['$scope', '$notify', '$api', '$form'];

    function controller($scope, $notify, $api, $form) {

        $scope.model = {};

        $scope.status = null;
        $scope.isBusy = false;

        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                var model = {
                    oldPassword: $scope.model.currentPassword,
                    newPassword: $scope.model.password
                };

                $api.user.changePassword(model).then(function (json) {

                    $notify.info('Your password has been successfully changed.');

                }).finally(function () { $scope.isBusy = false; });
            });
        };
    };
})();