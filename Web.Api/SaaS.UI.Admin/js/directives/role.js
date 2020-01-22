(function () {
    'use strict';

    angular
        .module('app.directives')
        .directive('ngRole', directive)
        .directive('ngAccountType', accountType)
        .directive('ngAccountActivated', accountActivated)
        .directive('ngSpId', localSubscription);

    directive.$inject = [];
    accountType.$inject = ['$api'];
    accountActivated.$inject = ['$api'];

    var _getRoleCssClass = function (role) {

        switch (role) {
            case 'admin': return 'label-danger';
            case 'agent': return 'label-info';
            case 'supervisior': return 'label-warning';

            default:
                return 'label-success';
        }
    };

    function directive() {

        return {
            link: link,
            scope: {
                role: '=ngRole'
            },
            restrict: 'A'
        };

        function link(scope, element, attrs) {

            element.addClass('label')
                   .addClass(_getRoleCssClass(scope.role))
                   .text(scope.role);
        };
    };
    function accountType($api) {

        return {
            link: link,
            scope: {
                status: '=ngAccountType'
            },
            restrict: 'A'
        };

        function link(scope, element, attrs) {

            element.addClass('label');

            var _apply = function () {

                element
                    .removeClass('label-primary')
                    .removeClass('label-info');

                !$api.account.isBusiness(scope) && element.addClass('label-info').text('b2c');
                $api.account.isBusiness(scope) && element.addClass('label-primary').text('b2b');
            };

            var watch = scope.$watch('status', _apply);

            scope.$on("$destroy", function () {

                watch();
            });
        };
    };
    function accountActivated($api) {

        return {
            link: link,
            scope: {
                status: '=ngAccountActivated'
            },
            restrict: 'A'
        };

        function link(scope, element, attrs) {

            element.addClass('label');

            var _apply = function () {

                element
                    .removeClass('label-warning')
                    .removeClass('label-success');

                !$api.account.isActivated(scope) && element.addClass('label-warning').text('not activated');
                $api.account.isActivated(scope) && element.addClass('label-success').text('activated');
            };

            var watch = scope.$watch('status', _apply);

            scope.$on("$destroy", function () {

                watch();
            });
        };
    }
    function localSubscription() {

        return {
            link: link,
            scope: {
                status: "=ngSpId"
            },
            restrict: 'A'
        };

        function link(scope, element, attrs) {

            element
                .addClass("label label-danger")
                .attr("title", "Subscription has been created on CP.")
                .text("!").tooltip();

            var _apply = function () {


                if (scope.status && scope.status.startsWith("D00000"))
                    element.show();
                else
                    element.hide();
            };

            var watch = scope.$watch("status", _apply);

            scope.$on("$destroy", function () {

                watch();
            });
        };
    };

})();