angular.module('app.directives')
    .directive('ngSpinner', ['$rootScope', ($rootScope) => {

        return {
            restrict: 'E',
            replace: true,
            template: '<div class="spinner" style="display:none"></div>',
            link: (scope, element, attrs) => {

                $rootScope.$on('event:spinner-show', () => {

                    element.show();
                });
                $rootScope.$on('event:spinner-hide', () => {
                    
                    element.hide();
                });

                //scope.$on('$destroy', cleanup);
            }
        };
    }]);