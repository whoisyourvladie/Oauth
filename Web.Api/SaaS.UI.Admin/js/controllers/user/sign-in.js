(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('signInController', controller);

    controller.$inject = ['$scope', '$auth', '$form'];

    function controller($scope, $auth, $form) {

        $scope.model = {};

        $scope.status = null;
        $scope.isBusy = false;

        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                return $auth.signIn($scope.model.login, $scope.model.password)
                    .then(function (response) { window.location = '/'; }, function () { $scope.isBusy = false; });
            });
        };
    };
})();