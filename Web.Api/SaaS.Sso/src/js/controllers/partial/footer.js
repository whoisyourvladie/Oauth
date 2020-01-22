angular.module('app.controllers')
    .controller('partialFooterController', ['$scope', '$state', '$brand', ($scope, $state, $brand) => {

        let _languages = {

            en: 'English',
            fr: 'Français',
            de: 'Deutsch‬'
        };

        $scope.$brand = $brand;
        $scope.model = {
            locale: $state.params.locale.toLowerCase()
        };

        $scope.currentLanguageTitle = () => {

            return _languages[$scope.model.locale];
        };
        $scope.changeLanguage = (locale) => {

            $scope.model.locale = locale;

            var url = $state.current.url.replace(/^\//gi, '');
            var params = angular.copy($state.params);
            params.locale = locale;

            $state.go(url, params);
        };
    }]);