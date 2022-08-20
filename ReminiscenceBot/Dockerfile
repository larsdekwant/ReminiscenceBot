FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy and publish app and libraries
COPY . .
RUN dotnet publish -c Release -o /app

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ReminiscenceBot.dll"]