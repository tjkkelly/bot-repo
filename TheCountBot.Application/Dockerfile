# Base image for build
FROM microsoft/dotnet:latest AS build-env
WORKDIR /app

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# build runtime image
FROM microsoft/dotnet:latest
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "TheCountBot.Application.dll"]
