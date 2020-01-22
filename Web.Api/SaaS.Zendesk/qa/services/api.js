describe('service:$api', () => {

    var $httpBackend, $api;

    beforeEach(module('app'));
    beforeEach(inject(($injector) => {

        $httpBackend = $injector.get('$httpBackend');
        $api = $injector.get('$api');
    }));

    afterEach(function () {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });

    it('http:toBeDefined', () => {

        expect($api.http).toBeDefined();
    });
    it('http:expectGET("/api/config/")', () => {

        $httpBackend.expectGET('/api/config/')
            .respond({ success: true });

        $api.http().then(function (response) {

            expect(response.data.success).toBe(true);
        });

        $httpBackend.flush();
    });
});