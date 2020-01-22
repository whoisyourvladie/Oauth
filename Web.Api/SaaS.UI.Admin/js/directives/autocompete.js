(function () {

    'use strict';
    angular
        .module('app.directives')
        .directive('ngAutocomplete', directive);

    directive.$inject = ['$timeout'];

    function directive($timeout) {

        var _autoCompleteInputs = angular.element('#autoCompleteForm input');

        var _getAutoCompleteJson = function () {

            var autoCompleteJson = {};

            _autoCompleteInputs.each(function () {

                var input = angular.element(this);
                var name = input.attr('name');

                autoCompleteJson[name] = input.val();
            });

            return autoCompleteJson;
        };

        var directive = {
            link: link,
            restrict: 'A'
        };

        return directive;

        function link($scope, element, attr) {

            element.attr('novalidate', 'novalidate');
            element.attr('autocomplete', 'off');
            element.find('input,select').eq(0).focus();

            $scope.model = $scope.model || {};

            $scope.form.$setPristine();

            if (attr.ngAutocomplete == 'off')
                element.find('input').attr('autocomplete', 'off').val(null);

            $timeout(function () {

                $scope.form.$setPristine();

                var autoCompleteJson = _getAutoCompleteJson();

                if (attr.ngAutocomplete == 'off') {

                    //element.find('input').attr('autocomplete', 'off').val(null);

                    //for (var key in autoCompleteJson)
                    //    $scope.model[key] = '';
                }
                else {

                    for (var key in autoCompleteJson) {

                        var value = autoCompleteJson[key];

                        if (element.find('[name="' + key + '"]').size())
                            $scope.model[key] = $scope.model[key] ? $scope.model[key] : value;
                    }
                }

            }, 100);
        }
    }

})();