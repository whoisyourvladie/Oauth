angular.module('app.controllers')
    .controller('appController', ['$scope', '$state', '$zendesk', '$brand', '$auth',
        ($scope, $state, $zendesk, $brand, $auth) => {

            $zendesk.init();

            if ($zendesk.isEmpty())
                return $state.go('zat/client-not-found');

            var _validateBrand = (brand) => {

                $brand.set(brand);
                
                if (!$brand.isSupport())
                    return $state.go('brand/not-supported', { brandId: brand.id });

                $scope.$auth = $auth;
                $state.go('account', { brandId: brand.id });
            };

            $zendesk.get(['ticket.brand'])
                .then((response) => {
                    _validateBrand(response['ticket.brand']);
                },
                    () => { $state.go('ticket/not-found'); });

            $zendesk.on('ticket.brand.changed', _validateBrand);

        }]);