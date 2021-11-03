#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Pulse.Api/Pulse.Api.csproj", "Pulse.Api/"]
RUN dotnet restore "Pulse.Api/Pulse.Api.csproj"
COPY . .
WORKDIR "/src/Pulse.Api"
RUN dotnet build "Pulse.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pulse.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pulse.Api.dll"]