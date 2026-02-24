Write-Host "Buiding mensajeria"
docker build -t mm-comunicaciones:1.1.6 -f src\pods\comunicaciones\Dockerfile .

Write-Host "Pushing"
docker tag mm-comunicaciones:1.1. oswaldodgmx/mm-comunicaciones:1.1.0
docker tag mm-comunicaciones:1.1.6 oswaldodgmx/mm-comunicaciones:latest
docker push oswaldodgmx/mm-comunicaciones:1.5.6
docker push oswaldodgmx/mm-comunicaciones:latest

Write-Host "End"