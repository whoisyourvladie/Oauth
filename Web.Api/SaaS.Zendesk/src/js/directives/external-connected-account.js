angular.module('app.directives')
    .directive('ngExternalConnectedAccount', [() => {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {

                element
                    .addClass('fab')
                    .addClass('fa-' + attrs.ngExternalConnectedAccount);

                switch (attrs.ngExternalConnectedAccount) {
                    case 'google': element.css({ color: '#d62d20' }); break;
                    case 'facebook': element.css({ color: '#3b5998' }); break;
                    case 'microsoft': element.css({ color: '#00a1f1' }); break;
                }
            }
        };
    }]);