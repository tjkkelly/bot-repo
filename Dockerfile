# Base image for build
FROM mcr.microsoft.com/dotnet/sdk:3.1-focal-arm64v8 AS build-env
WORKDIR /app

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# build runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1-focal-arm64v8
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "TheCountBot.Application.dll"]
