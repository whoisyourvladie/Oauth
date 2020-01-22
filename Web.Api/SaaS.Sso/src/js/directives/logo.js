angular.module('app.directives')
    .directive('ngLogo', ['$brand', ($brand) => {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {

                let logo = $brand.logo();

                element.html(`<img width="${logo.width}" src="${logo.src}">`);
            }
        };
    }]);