angular.module('app')
    .config(['$translateProvider', '$translatePartialLoaderProvider',
        ($translateProvider, $translatePartialLoaderProvider) => {

            // // $translateProvider.useLoader('$i18nLoader', {});
            // // $translateProvider.useLocalStorage();

            $translateProvider.useLoader('$translatePartialLoader', {
                urlTemplate: 'i18n/{lang}.json'
            });

            $translateProvider.preferredLanguage('en');
            $translateProvider.fallbackLanguage('en');
            
            $translatePartialLoaderProvider.addPart('index');
        }]);