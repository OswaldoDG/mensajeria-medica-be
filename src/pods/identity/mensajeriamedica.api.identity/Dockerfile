FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore "src/pods/identity/mensajeriamedica.api.identity/mensajeriamedica.api.identity.csproj"

# Build and publish a release
RUN dotnet publish "src/pods/identity/mensajeriamedica.api.identity/mensajeriamedica.api.identity.csproj" -c Release -o /App/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build /App/out .

COPY build-assets/ca.crt /usr/local/share/ca-certificates/contabee-ca.crt
RUN update-ca-certificates
RUN apt-get update && apt-get install -y curl
ENTRYPOINT ["dotnet", "mensajeriamedica.api.identity.dll"]
