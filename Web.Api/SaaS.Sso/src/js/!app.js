angular.module('app.components', ['ui.router', 'pascalprecht.translate']);
angular.module('app.services', []);
angular.module('app.controllers', []);
angular.module('app.directives', []);
angular.module('app.filters', []);
angular.module('app.external', ['LocalStorageModule']); //angular-local-storage

angular.module('app', ['ui.router',
    'app.components', 'app.services', 'app.controllers', 'app.directives', 'app.filters', 'app.external'])
    .run(function ($trace, $transitions, $translate, $brand, $sso) {

        if (!$brand.validate())
            return $brand.redirect();

        //$trace.enable('TRANSITION');
        $transitions.onBefore({ to: '**' }, (transition) => {

            let to = transition.to();

            if (to.name === 'account/logout' && $sso.logout())
                document.body.style.display = 'none';
        });

        //$transitions.onSuccess({ to: '**' }, (transition) => {

        //    let stateService = transition.router.stateService;
        //    let locale = stateService.params.locale || 'en';

        //    $translate.use(locale);
        //});
    });