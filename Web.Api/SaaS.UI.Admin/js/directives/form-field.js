/// <reference path="../bundle.js" />
(function () {

    'use strict';
    angular
        .module('app.directives')
        .directive('ngLoadingForm', ngLoadingForm)
        .directive('ngFormValidationGroup', ngFormValidationGroup)
        .directive('ngPasswordPattern', ngPasswordPattern)
        .directive('ngMatch', ngMatch);

    ngLoadingForm.$inject = [];
    ngFormValidationGroup.$inject = [];
    ngMatch.$inject = [];
    ngPasswordPattern.$inject = [];
    ngMatch.$inject = [];

    function ngLoadingForm() {

        var directive = {
            scope: {},
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attr) {

            var buttons = element.find('[type="submit"]');

            buttons.each(function () {

                var button = angular.element(this);

                button.html('<i class="fa fa-spinner fa-spin" style="display:none"></i> ' + button.val());
            });

            var icons = buttons.find('i');

            var isBusyWatcher = scope.$watch('$parent.isBusy', function (newValue, oldValue) {

                buttons.prop('disabled', newValue);
                newValue ? icons.show() : icons.hide();
            });

            scope.$on('$destroy', function () {
                isBusyWatcher();
            });
        }
    }
    function ngFormValidationGroup() {

        var directive = {

            scope: {

            },
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attr) {

            element.addClass('form-group');

            if (element.find('label').size())
                element.addClass('has-label');

            var input = element.find('input:first, select:first, textarea:first').addClass('form-control');
            element.find('span').addClass('label label-danger').hide();

            var errors = [];

            var _addError = function (validationKey) {

                var currentSpan = element.find('span[data-ng-' + validationKey + '-error]');
                if (!currentSpan.size())
                    return;

                errors.push({ span: currentSpan, validationKey: validationKey });
            };

            _addError('required');
            _addError('pattern');
            _addError('match');
            _addError('passwordPattern');
            _addError('passwordSpecialCharacters');
            _addError('passwordSpecialWords');

            var field = input.attr('name');
            var type = input.attr('type');
            var isPattern = input.is('[ng-password-pattern]');

            scope.form = scope.$parent.form;

            var _hasError = function () {

                return scope.form[field].$invalid && !scope.form[field].$pristine;
            };
            var _hasSuccess = function () {

                if (type === 'password' && !isPattern)
                    return false;

                return !scope.form[field].$invalid && !scope.form[field].$pristine;
            };
            var _hasValidationError = function (validationKey) {

                var _field = scope.form[field];
                var _error = _field.$error[validationKey];

                return (_error && !_field.$pristine) || (_error && !_field.$pristine);
            };

            var _validate = function () {

                var hasError = _hasError();
                var hasSuccess = _hasSuccess();

                hasError ? element.addClass('has-error') : element.removeClass('has-error');
                hasSuccess ? element.addClass('has-success') : element.removeClass('has-success');

                for (var index = 0; index < errors.length; index++)
                    errors[index].span.hide();

                for (var index = 0; index < errors.length; index++) {

                    if (_hasValidationError(errors[index].validationKey)) {

                        errors[index].span.show();
                        break;
                    }
                }
            };

            scope.$watch('form.' + field + '.$pristine', _validate);

            for (var index = 0; index < errors.length; index++)
                scope.$watch('form.' + field + '.$error.' + errors[index].validationKey, _validate);
        }
    };

    function ngMatch() {

        var directive = {
            require: 'ngModel',
            scope: {
                ngMatch: '='
            },
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attributes, ngModel) {

            ngModel.$validators.match = function (modelValue) {

                return modelValue == scope.ngMatch;
            };

            scope.$watch('ngMatch', function () {
                ngModel.$validate();
            });
        }
    };

    function ngPasswordPattern() {

        var regex1 = new RegExp(/^((?=.*\d)(?=.*[^ \t0-9!"\#$%&'()*+,\-./:;<=>?@\[\\\]^_`{|}~]).{6,})$/i);

        var regex2 = new RegExp(/^(?!.*(\d)\1{2})(?!.*(0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)|9(?=0)){2}).{6,100}$/i);
        var regex3 = new RegExp(/^(?!.*(abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz)).{6,100}$/i);
        var regex4 = new RegExp(/^(?!.*(.)\1{2}).*$/i);

        var regex5 = new RegExp(/^(?!.*(admin|administrator|password)).{6,100}$/i);

        var directive = {
            require: 'ngModel',
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attributes, ngModel) {

            ngModel.$validators.passwordPattern = function (modelValue) {

                return regex1.test(modelValue);
            };
            ngModel.$validators.passwordSpecialCharacters = function (modelValue) {

                return regex2.test(modelValue) && regex3.test(modelValue) && regex4.test(modelValue);
            };
            ngModel.$validators.passwordSpecialWords = function (modelValue) {

                return regex5.test(modelValue);
            };
        }
    };

})();