angular
    .module('app.services')
    .factory('$api', ['$http', '$brand', ($http, $brand) => {

        let _uri = (method) => {

            return `${$brand.oauthLink()}/${method}`;
        };
        let _getConfig = () => {

            return {
                asJson: true,
                isOauth: true
            };
        };

        let service = {};

        service.account = {
            get: (params) => {

                let config = _getConfig();
                config.params = params;

                return $http.get(_uri('api/account/'), config);
            }
        };
        service.tokenJwt = (data) => {

            let config = _getConfig();

            return $http.post(_uri('api/token/jwt'), data, config);
        };

        return service;
    }]);