describe('service:$ws', () => {

    var $ws;

    beforeEach(module('app'));
    beforeEach(inject(($injector) => {

        $ws = $injector.get('$ws');
    }));

    it('wifi:toBeDefined', () => {

        expect($ws.wifi).toBeDefined();
    });
    it('wifi.scan:toBeDefined', () => {

        expect($ws.wifi.scan).toBeDefined();
    });
});