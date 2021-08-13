.PHONY: build run test test-basic clean

build:
	docker-compose -f docker-compose.yml -f docker-compose.test.yml build

run:
	docker-compose -f docker-compose.yml -f docker-compose.storage.yml -f docker-compose.proxy.yml up ork1 ork2 ork3

test:  test-basic clean
	
test-basic:
	docker-compose -f docker-compose.yml -f docker-compose.test.yml up --exit-code-from test test

clean:
	docker-compose -f docker-compose.yml -f docker-compose.proxy.yml -f docker-compose.test.yml down
