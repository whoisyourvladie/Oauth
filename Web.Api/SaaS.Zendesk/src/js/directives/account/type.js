angular.module('app.directives')
    .directive('ngAccountType', ['$api', ($api) => {

        return {
            restrict: 'A',
            scope: {
                status: '=ngAccountType'
            },
            link: (scope, element, attrs) => {

                element.addClass('badge');

                var watch = scope.$watch('status', () => {

                    element
                        .removeClass('badge-primary')
                        .removeClass('badge-info');

                    !$api.account.isBusiness(scope) && element.addClass('badge-info').text('b2c');
                    $api.account.isBusiness(scope) && element.addClass('badge-primary').text('b2b');
                });

                scope.$on("$destroy", () => {

                    watch();
                });
            }
        };
    }]);