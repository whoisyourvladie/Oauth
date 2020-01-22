(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountController', controller);

    controller.$inject = ['$rootScope', '$scope', '$api', '$query'];

    function controller($rootScope, $scope, $api, $query) {

        $scope.model = { account: null };

        $scope.delete = function () {

            if (!confirm('Are you sure you want to delete this customer?'))
                return;

            $api.account._delete($scope.model.account.id).then(function () {

                $rootScope.$broadcast('event:accountDeleted', {});
            });
        };

        $scope.deleteGDPR = function (index) {

            if (!confirm('Are you sure you want to delete this customer?'))
                return;
            $api.account._deleteGDPR($scope.model.account.id).then(function () {

                //alert("Account " + $scope.model.accounts[index].email + " has been successfuly deleted");
                //$scope.model.accounts[index].isDeleted = true;
                $rootScope.$broadcast('event:accountDeleted', {});

            });
        };

        (function ($scope) {

            var query = $query.getJson();

            query.id && $api.account.get(query).then(function (json) {

                $rootScope.$broadcast('event:accountLoaded', json);
            });

        })($scope);

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.id = json.id;
            $scope.model.account.email = json.email;
            $scope.model.account.firstName = json.firstName;
            $scope.model.account.lastName = json.lastName;
            $scope.model.account.status = json.status;
        });
        $rootScope.$on('event:accountDeleted', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.isDeleted = true;
        });
    };
})();