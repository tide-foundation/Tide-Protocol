.PHONY: build run test test-basic unit-test clean ork

build:
	docker-compose -f docker-compose.yml -f docker-compose.test.yml build

build-ork:
	docker build -t tideorg/ork -f Tide.Ork\Dockerfile .

run:
	docker-compose -f docker-compose.yml -f docker-compose.storage.yml -f docker-compose.proxy.yml up ork1 ork2 ork3

simulator:
	docker-compose -f docker-compose.yml -f docker-compose.storage.yml -f docker-compose.proxy.yml up -d simulator

dauth: build
	docker-compose -f docker-compose.yml -f docker-compose.storage.yml up ork1 ork2 ork3

unit-test:
	dotnet test Tide.Ork.Test

test:  build test-basic clean
	
test-basic:
	docker-compose -f docker-compose.yml -f docker-compose.test.yml up --exit-code-from test test

clean:
	docker-compose -f docker-compose.yml -f docker-compose.proxy.yml -f docker-compose.test.yml down

n=1
ork:
	n=$n; port=500$n; sw=true; \
	while [ "$$sw" = true ]; \
	do nc -z localhost $$port && n=$$((n+1)) && port=$$((port+1)) || sw=false; done; \
	export ASPNETCORE_ENVIRONMENT=ork$$n; \
	dotnet watch --project Tide.Ork run --no-build --no-launch-profile --urls=http://0.0.0.0:$$port;
