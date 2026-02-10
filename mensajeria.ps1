Write-Host "Buiding mensajeria"
docker build -t mm-comunicaciones:1.1.5 -f src\pods\comunicaciones\Dockerfile .

Write-Host "Pushing"
docker tag mm-comunicaciones:1.1. oswaldodgmx/mm-comunicaciones:1.1.0
docker tag mm-comunicaciones:1.1.5 oswaldodgmx/mm-comunicaciones:latest
docker push oswaldodgmx/mm-comunicaciones:1.5.0
docker push oswaldodgmx/mm-comunicaciones:latest

Write-Host "End"