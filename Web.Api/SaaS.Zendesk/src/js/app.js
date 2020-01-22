angular.module('app.auth', []);
angular.module('app.services', []);
angular.module('app.controllers', []);
angular.module('app.directives', []);
angular.module('app.filters', []);

angular.module('app', ['ui.router',
    'app.auth', 'app.services', 'app.controllers', 'app.directives', 'app.filters'])
    .run(function ($trace, $transitions, $auth, $brand) {

        $trace.enable('TRANSITION');

        $transitions.onBefore({ to: '**' }, (transitions) => {

            if (transitions.to().isProtected && !$auth.isAuthenticated()) {

                var query = {};

                if (!$brand.isEmpty())
                    query.brandId = $brand.get().id;

                return transitions.router.stateService.target('user/sign-in', query);
            }
        });
    })
    .config(($stateProvider, $urlRouterProvider) => {

        // $locationProvider.html5Mode({
        //     enabled: true,
        //     requireBase: false
        // });

        $urlRouterProvider.otherwise('/en/index');

        $stateProvider.state('app', {
            url: '/:locale',
            //templateUrl: 'index.html',
            restricted: false,
            abstract: true,
            views: {
                sidebar: {
                    controller: 'partialSidebarController',
                    templateUrl: 'partial/sidebar.html',
                },
                content: {
                    controller: 'appController',
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

        _state({
            url: 'account', controller: 'account', templateUrl: 'views/account/index.html',
            params: { accountId: null, brandId: null }, isProtected: true
        });
        _state({
            url: 'account/external-session-tokens', controller: 'accountExternalSessionTokens',
            params: { accountId: null }, isProtected: true
        });
        _state({
            url: 'account/product', controller: 'accountProduct',
            params: { accountId: null, accountProductId: null }, isProtected: true
        });
        _state({
            url: 'account/products', controller: 'accountProducts',
            params: { accountId: null }, isProtected: true
        });
        _state({
            url: 'account/sub-emails', controller: 'accountSubEmails',
            params: { accountId: null }, isProtected: true
        });

        _state({
            url: 'account/email-is-empty', controller: 'accountEmailIsEmpty',
            params: { email: null }, isProtected: true
        });
        _state({
            url: 'account/not-found', controller: 'accountNotFound',
            params: { email: null }, isProtected: true
        });

        _state({ url: 'brand/not-supported', controller: 'brandNotSupported', params: { brandId: null } });

        _state({ url: 'ticket/not-found', controller: 'ticketNotFound' });
        _state({ url: 'ticket/requester-email-is-empty', controller: 'ticketRequesterEmailIsEmpty' });

        _state({ url: 'user/sign-in', controller: 'userSignIn', params: { brandId: null } });

        _state({ url: 'zat/client-not-found', controller: 'zatClientNotFound' });
    });

//https://github.com/modularcode/modular-admin-angularjs/blob/master/src/_main.js