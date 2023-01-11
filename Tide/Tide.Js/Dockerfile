FROM node:14.21.2-alpine3.17 AS test
WORKDIR /src/Tide.Js
RUN apk add --no-cache curl bash \
    && curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /src/Tide.Js/wait-for-it.sh \
    && chmod +x /src/Tide.Js/wait-for-it.sh

COPY ["Tide.Js/package.json", "Tide.Js/package-lock.json", "./"]
RUN npm install

COPY ["Tide.Js/src/", "./src"]
COPY ["Tide.Js/test/", "./test"]
CMD ["npm", "run", "test"]