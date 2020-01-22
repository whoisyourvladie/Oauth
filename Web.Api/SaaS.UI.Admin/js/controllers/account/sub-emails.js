(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountSubEmailsController', controller);

    controller.$inject = ['$rootScope', '$scope', '$api'];

    function controller($rootScope, $scope, $api) {

        $scope.model = {
            account: null,
            subEmails: null
        };
        $scope.isBusy = false;

        var viewSubEmail = function (json) {

            var self = this;

            Object.defineProperties(this, {
                id: { value: json.id, writable: false },
                email: { value: json.email, writable: false },
                createDate: { value: json.createDate, writable: false }
            });
        };

        var _subEmails = function () {

            $scope.isBusy = true;
            $api.account.subEmails($scope.model.account.id).then(function (json) {

                var subEmails = [];

                json && json.forEach(function callback(value, index, array) {
                    subEmails.push(new viewSubEmail(value));
                });

                $scope.model.subEmails = subEmails;

            }).finally(function () { $scope.isBusy = false; });
        };

        $scope.refresh = function () {

            if ($scope.isBusy) return;

            $scope.isBusy = true;

            _subEmails();
        };

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = json;
            $scope.refresh();
        });
    };
})();