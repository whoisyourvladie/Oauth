(function () {
    'use strict';

    angular
        .module('app.directives')
        .directive('ngExternalConnectedAccount', directive)

    directive.$inject = ['$filter'];

    var _init = function (element, provider) {

        element
            .addClass('fab')
            .addClass('fa-' + provider)
            .css({ 'font-size': 18 });

        switch (provider)
        {
            case 'google': element.css({ color: '#d62d20' }); break;
            case 'facebook': element.css({ color: '#3b5998' }); break;
            case 'microsoft': element.css({ color: '#00a1f1' }); break;
        }
    };

    function directive() {

        return {
            link: link,
            restrict: 'A'
        };

        function link(scope, element, attrs) {

            var text = element.text();
            _init(element, attrs.ngExternalConnectedAccount);
        }
    };

})();