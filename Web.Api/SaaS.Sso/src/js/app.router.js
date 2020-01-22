angular.module('app')

    .config(($stateProvider, $urlRouterProvider, localStorageServiceProvider) => {

        localStorageServiceProvider
            .setPrefix('sso')
            .setDefaultToCookie(false);

        // $locationProvider.html5Mode({
        //     enabled: true,
        //     requireBase: false
        // });

        $urlRouterProvider.otherwise('/en/index');

        $stateProvider.state('app', {
            url: '/:locale', //  url: '/:locale/{brand:sodapdf}',
            //templateUrl: 'index.html',
            restricted: false,
            abstract: true,
            views: {
                content: {
                    controller: 'appController'
                },
                footer: {
                    templateUrl: 'partial/footer.html',
                    controller: 'partialFooterController'
                }
            }
        });

        var _state = (json) => {

            json.name = json.name || json.url;
            json.params = json.params || {};
            json.templateUrl = json.templateUrl || `views/${json.url}.html`;
            json.isProtected = !!json.isProtected;

            var state = {
                parent: 'app',
                url: `/${json.url}`,
                params: json.params,
                templateUrl: json.templateUrl,
                controller: `${json.controller}Controller`,
                isProtected: json.isProtected
            };

            $stateProvider.state(json.name, state);
        };

        _state({ url: 'index', controller: 'index' });

        _state({ url: 'account/error', controller: 'accountError' });
        _state({ url: 'account/logout', controller: 'accountLogout' });
        _state({ url: 'account/identifier', controller: 'accountIdentifier', params: { email: null } });
        _state({ url: 'account/password', controller: 'accountPassword', params: { firstName: null, lastName: null, email: null } });
        _state({ url: 'account/remove', controller: 'accountRemove' });
        _state({ url: 'account/select', controller: 'accountSelect' });
    });