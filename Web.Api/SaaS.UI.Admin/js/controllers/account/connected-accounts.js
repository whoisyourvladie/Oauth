(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountConnectedAccountsController', controller);

    controller.$inject = ['$rootScope', '$scope', '$api'];

    function controller($rootScope, $scope, $api) {

        $scope.model = {
            account: null,
            externalSessionTokens: null
        };
        $scope.isBusy = false;

        var viewSessionToken = function (json) {

            var self = this;

            Object.defineProperties(this, {
                id: { value: json.id, writable: false },
                email: { value: json.email, writable: false },
                isUnlinked: { value: !!json.isUnlinked, writable: false },
                createDate: { value: json.createDate, writable: false },
                modifyDate: { value: json.modifyDate, writable: false },
                externalClientName: { value: json.externalClientName, writable: false },
                externalAccountId: { value: json.externalAccountId, writable: false }
            });
        };

        var _sessionTokens = function () {

            $scope.isBusy = true;
            $api.account.external.sessionTokens($scope.model.account.id).then(function (json) {

                var sessionTokens = [];

                json && json.forEach(function callback(value, index, array) {
                    sessionTokens.push(new viewSessionToken(value));
                });

                $scope.model.sessionTokens = sessionTokens;

            }).finally(function () { $scope.isBusy = false; });
        };

        $scope.refresh = function () {

            if ($scope.isBusy) return;

            $scope.isBusy = true;

            _sessionTokens();
        };

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = json;
            $scope.refresh();
        });
    };
})();