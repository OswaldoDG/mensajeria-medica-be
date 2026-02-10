Write-Host "Building identity"
docker build -t pdfsplitter-identity:1.0.0 -f src\pods\identity\mensajeriamedica.api.identity\Dockerfile .
Write-Host "Buiding pdf revision"
docker build -t pdfsplitter-revision:1.5.0 -f pdf.revision.api\Dockerfile .

Write-Host "Pushing"
docker tag pdfsplitter-identity:1.0.0 oswaldodgmx/pdfsplitter-identity:latest
docker tag pdfsplitter-revision:1.5.0 oswaldodgmx/pdfsplitter-api:latest
docker push oswaldodgmx/pdfsplitter-identity:latest
docker push oswaldodgmx/pdfsplitter-api:latest

Write-Host "End"