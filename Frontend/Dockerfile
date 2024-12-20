# syntax=docker/dockerfile:1

# build stage
FROM node:22 AS build
WORKDIR /source
COPY Frontend/ResultCollector ./ResultCollector
WORKDIR /source/ResultCollector
RUN npm install && npm run build

# final stage
FROM nginx:1.27 AS nginx
ENV TZ=Asia/Tokyo
COPY Frontend/nginx/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /source/ResultCollector/build/client/ /usr/share/nginx/app/result-collector/
