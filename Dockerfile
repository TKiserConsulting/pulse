## building dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.13 AS build-dotnet
LABEL \
	product="pulse" \
	application="pulse-dotnet"
WORKDIR /src

COPY ./source/backend/*.sln ./
COPY ./source/backend/projects/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p projects/${file%.*} && mv $file projects/${file%.*}; done

RUN dotnet restore -r alpine-x64

COPY ./source/backend/ ./

ARG version=1.0.0-d000000

RUN dotnet publish --no-restore -c Release -o /dist -p:InformationalVersion=$version


# building angular
FROM node:14-alpine AS build-angular
LABEL \
	product="pulse" \
	application="pulse-angular"
WORKDIR /src

COPY ./source/frontend/package*.json ./

RUN npm ci --no-optional --no-progress --loglevel=error --silent

COPY ./source/frontend/ ./

ARG version=1.0.0-d000000

RUN  \
    npm run version -- v${version} --unsafe-perm  && \ 
    npm run lint && \ 
    npm run build:prod && \
    rm ./dist/pulse/stats.json && \
    cp -r ./dist/pulse /dist


# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.13
LABEL \
	product="pulse" \
	application="portal"
ENV \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:8080 \
    PORT=8080 \
    SSH_PORT=2222 \
    SSH_PWD=Docker! \
    BUILD_DEPS="gettext"  \
    RUNTIME_DEPS="libintl"
WORKDIR /app

RUN \
    adduser \
        --disabled-password \
        --home /app \
        --gecos '' app \
    && chown -R app /app
#RUN \
#    adduser \
#        --disabled-password \
#        --home /app \
#        --gecos '' app & \
#    set -x && \
#    apk add --update $RUNTIME_DEPS && \
#    apk add --virtual build_deps $BUILD_DEPS &&  \
#    cp /usr/bin/envsubst /usr/local/bin/envsubst && \
#    apk add openssh && \
#    echo "root:Docker!" | chpasswd && \
#    ssh-keygen -A && \
#    apk del build_deps
#EXPOSE 8080 2222
EXPOSE 8080 

#COPY sshd_config /etc/ssh/
#COPY entrypoint.sh ./
#RUN chmod +x entrypoint.sh && chown -R app /etc/ssh
USER app

COPY --from=build-dotnet /dist .
COPY --from=build-angular --chown=app /dist ./wwwroot

#ENTRYPOINT ["entrypoint.sh"]
CMD ["dotnet", "Pulse.Api.dll"]
