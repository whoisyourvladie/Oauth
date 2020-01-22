angular.module('app.services')
    .factory('$utils', () => {

        let service = {};

        service.query = (query) => {

            let json = {};

            try {

                if (typeof query === 'undefined')
                    query = document.location.search;

                var split = query.replace(/(^\?)/, '').split('&');

                for (let i = 0; i < split.length; ++i) {

                    let item = split[i];
                    let index = item.indexOf('=');
                    if (index === -1)
                        continue;

                    let key = item.substring(0, index);
                    let value = item.substr(index + 1).trim();
                    json[key.toLowerCase()] = decodeURIComponent(value);
                }

            } catch (e) { }

            return json;
        };
        service.params = (json) => {

            let queryString = [];

            for (let key in json)
                queryString.push(key.toLowerCase() + '=' + encodeURIComponent(json[key]));

            return queryString.join('&');
        };
        service.hash = (value) => {

            let hash = 0;

            if (!value)
                return hash;

            for (let index = 0; index < value.length; index++)
                hash = value.charCodeAt(index) + ((hash << 5) - hash);

            return hash;
        };
        service.intToRgb = (value) => {

            let str = (value & 0x00FFFFFF)
                .toString(16)
                .toUpperCase();

            return '00000'.substring(0, 6 - str.length) + str;
        };
        service.stringToColor = (value) => {

            let hash = service.hash(value);
            let rgb = service.intToRgb(hash);

            return `#${rgb}`;
        };

        return service;
    });