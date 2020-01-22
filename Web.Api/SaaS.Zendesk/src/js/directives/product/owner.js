angular.module('app.directives')
    .directive('ngProductOwner', ['$api', ($api) => {

        return {
            restrict: 'A',
            scope: {
                status: '=ngProductOwner'
            },
            link: (scope, element, attrs) => {

                element.addClass('badge');

                var watch = scope.$watch('status', (status) => {

                    if (!status) return;

                    element
                        .removeClass('badge-warning')
                        .removeClass('badge-success');

                    !$api.product.isOwner(scope) && element.addClass('badge-danger').text('not owner');
                    $api.product.isOwner(scope) && element.addClass('badge-success').text('owner');
                });

                scope.$on("$destroy", () => {

                    watch();
                });
            }
        };
    }]);