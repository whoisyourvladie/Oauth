angular.module('app.directives')
    .directive('ngFocus', ['$window', ($window) => {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {

                element.first().focus();
            }
        };
    }]);