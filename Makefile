.PHONY: build run test test-basic unit-test clean ork

build:
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.test.yml build

build-ork:
	docker build -t tideorg/ork -f Tide/Tide.Ork/Dockerfile .

run:
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.storage.yml -f Tide/docker-compose.proxy.yml up ork1 ork2 ork3 ork4 ork5

simulator:
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.storage.yml -f Tide/docker-compose.proxy.yml up -d simulator

dauth: build
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.storage.yml up ork1 ork2 ork3 ork4 ork5

unit-test:
	dotnet test Tide/Tide.Ork.Test

test:  build test-basic clean
	
test-basic:
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.test.yml up --exit-code-from test test

clean:
	docker-compose -f Tide/docker-compose.yml -f Tide/docker-compose.proxy.yml -f Tide/docker-compose.test.yml down

ork:
	@read -p "Enter the ORK number: " n; \
	[ -z "$$n" ] && echo "n cannot be empty" && return 1 || \
	port=500$$n; export ASPNETCORE_ENVIRONMENT=ork$$n; \
	dotnet watch --project Tide/Tide.Ork run --no-build --no-launch-profile --urls=http://0.0.0.0:$$port;
