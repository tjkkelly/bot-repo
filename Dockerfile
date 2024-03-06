# Base image for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "TheCountBot.Application.dll"]
