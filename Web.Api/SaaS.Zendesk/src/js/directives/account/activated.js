angular.module('app.directives')
    .directive('ngAccountActivated', ['$api', ($api) => {

        return {
            restrict: 'A',
            scope: {
                status: '=ngAccountActivated'
            },
            link: (scope, element, attrs) => {

                element.addClass('badge');

                var watch = scope.$watch('status', () => {

                    element
                        .removeClass('badge-warning')
                        .removeClass('badge-success');

                    !$api.account.isActivated(scope) && element.addClass('badge-warning').text('not activated');
                    $api.account.isActivated(scope) && element.addClass('badge-success').text('activated');
                });

                scope.$on("$destroy", () => {

                    watch();
                });
            }
        };
    }]);