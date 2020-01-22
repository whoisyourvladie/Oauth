(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('menuController', controller);

    controller.$inject = ['$scope', '$auth'];

    function controller($scope, $auth) {

        $scope.$auth = $auth;
    };
})();