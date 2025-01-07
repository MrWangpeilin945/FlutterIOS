# syntax=docker/dockerfile:1

# build stage
FROM public.ecr.aws/docker/library/node:22 AS build
WORKDIR /source
COPY Frontend/ResultCollector ./ResultCollector
WORKDIR /source/ResultCollector
RUN npm install && npm run build

# final stage
FROM public.ecr.aws/docker/library/nginx:1.27 AS nginx
ENV TZ=Asia/Tokyo
VOLUME /etc/nginx
VOLUME /var/run
VOLUME /var/cache/nginx/
COPY Frontend/nginx/nginx.conf /etc/nginx/nginx.conf
COPY Frontend/ResultCollector.Equipments/scripts/ /usr/share/nginx/app/result-collector-equipment-scripts/
COPY --from=build /source/ResultCollector/build/client/ /usr/share/nginx/app/result-collector/
