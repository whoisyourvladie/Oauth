angular.module('app.directives')
    .directive('ngSidebar', ['$rootScope', '$api', ($rootScope, $api) => {

        return {
            restrict: 'A',
            link: (scope, element, attrs) => {

                var sidebar = element.find('.sidebar:first, .sidebar-content');
                var buttons = element.find('button.navbar-toggler');

                buttons.eq(0).bind('click', () => {
                    sidebar.css({ height: '100%' });
                });

                // element.find('.btn').bind('click', function () {
                //     sidebar.css({ height: '0%' });
                // })
                buttons.eq(1).bind('click', () => {
                    sidebar.css({ height: '0%' });
                });

                var cleanup = $rootScope.$on('event:closeSidebar', () => {

                    sidebar.css({ height: '0%' });
                });

                scope.$on('$destroy', cleanup);
            }
        };
    }]);