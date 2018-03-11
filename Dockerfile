FROM microsoft/dotnet:latest AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:latest
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "theCountBot.dll"]