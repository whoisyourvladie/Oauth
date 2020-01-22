angular.module('app.directives')
    .directive('ngAccountMenu', ['$state', ($state) => {

        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'directives/account/menu.html'
        };
    }]);