angular
    .module('app.services')
    .factory('$api', ['$http', '$brand', function factory($http, $brand) {

        const _config = {
            asJson: true,
            isOauth: true
        };

        const _accountStatusEnum = {
            none: 0,
            isActivated: 1 << 0,
            isAnonymous: 1 << 1,
            isBusiness: 1 << 2
        };;
        const _productStatusEnum = {

            none: 0,
            isDisabled: 1 << 0,
            isExpired: 1 << 1,
            isTrial: 1 << 2,
            isFree: 1 << 3,
            isMinor: 1 << 4,
            isDefault: 1 << 5,
            isPPC: 1 << 6,
            isUpgradable: 1 << 7,
            isNew: 1 << 8,
            isPaymentFailed: 1 << 9,
            isRenewal: 1 << 10,
            isOwner: 1 << 11,
            IsNotAbleToRenewCreditCartExpired: 1 << 12
        };

        var service = {};

        service.account = {
            isActivated: (account) => {

                return !!(account.status & _accountStatusEnum.isActivated);
            },
            isBusiness: (account) => {

                return !!(account.status & _accountStatusEnum.isBusiness);
            },
            get: (params) => {

                var method = 'api/account/';

                if (params.accountId) {

                    method += params.accountId;
                    delete params.accountId;
                }

                let config = angular.copy(_config);
                config.params = params;

                return $http.get($brand.getApiUri(method), config);
            }
        };
        service.product = {
            isDisabled: (product) => { return !!(product.status & _productStatusEnum.isDisabled); },
            isTrial: (product) => { return !!(product.status & _productStatusEnum.isTrial); },
            isFree: (product) => { return !!(product.status & _productStatusEnum.isFree); },
            isPPC: (product) => { return !!(product.status & _productStatusEnum.isPPC); },
            isRenewal: (product) => { return !!(product.status & _productStatusEnum.isRenewal); },
            isOwner: (product) => { return !!(product.status & _productStatusEnum.isOwner); }
        };

        service.account.getSubEmails = (params) => {

            return $http.get($brand.getApiUri(`api/account/${params.accountId}/sub-email`), _config);
        };
        service.account.removeSubEmail = (params) => {

            return $http.delete($brand.getApiUri(`api/account/${params.accountId}/sub-email/${params.id}`), _config);
        };

        service.account.getExternalSessionTokens = (params) => {

            return $http.get($brand.getApiUri(`api/account/${params.accountId}/external/session-token`), _config);
        };
        service.account.getOwnerProducts = (params) => {

            return $http.get($brand.getApiUri(`api/account/${params.accountId}/owner-product`), _config);
        };
        service.account.getOwnerProductDetails = (params) => {

            return $http.get($brand.getApiUri(`api/account/${params.accountId}/owner-product/${params.accountProductId}`), _config);
        };

        return service;
    }]);