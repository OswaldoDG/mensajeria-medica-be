Write-Host "Buiding mensajeria"
docker build -t mm-comunicaciones:1.2.2 -f src\pods\comunicaciones\Dockerfile .

Write-Host "Pushing"
docker tag mm-comunicaciones:1.2.2 oswaldodgmx/mm-comunicaciones:1.2.2
docker tag mm-comunicaciones:1.2.2 oswaldodgmx/mm-comunicaciones:latest
docker push oswaldodgmx/mm-comunicaciones:1.2.2
docker push oswaldodgmx/mm-comunicaciones:latest

Write-Host "End"