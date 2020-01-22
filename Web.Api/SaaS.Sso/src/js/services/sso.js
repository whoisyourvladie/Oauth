angular
    .module('app.services')
    .factory('$sso', ['$location', '$utils', ($location, $utils) => {

        let service = {};

        service.signInJwt = (json) => {

            let query = $utils.query();

            query.return_to && (json.return_to = query.return_to);

            let location = `${query.redirect_uri}?${$utils.params(json)}`;

            window.location = location;

        };
        service.logout = () => {

            let query = $utils.query();

            if (query.return_to)
                return !!(window.location = query.return_to);
        };



        return service;
    }]);