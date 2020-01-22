angular.module('app.directives')
    .directive('ngExternalConnectedAccountStatus', [() => {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {

                element
                    .addClass('fas')

                if (attrs.ngExternalConnectedAccountStatus === 'false') {

                    element
                        .addClass('fa-user-slash text-danger')
                        .attr('title', 'Disconnected')
                }
                else {

                    element
                        .addClass('fa-user text-success')
                        .attr('title', 'Connected')
                }
            }
        };
    }]);