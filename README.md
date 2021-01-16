# Goblineer Next Updater ![.NET](https://github.com/Goblineer-AH/goblineer-next-api/workflows/.NET/badge.svg)

Backend Web API for Goblineer. Fetches the data from the database and serves it to the frontend.

## Docker deployment
### Building with Docker
```sh
docker build . -t <your image tag>
```

### Running with Docker
```sh
docker run -it --rm \
    -e ALLOWED_FRONTEND_URLS=<comma separated list, eg.: http://localhost:80,http://localhost:3001,http://localhost:3000>
    -e DB_HOST=<the host of the database> \
    -e DB_USERNAME=<db user name> \
    -e DB_PASSWORD=<db user's password> \
    -e DB_DATABASE=<database to use> \
    -p 5000:80 \
    <your image tag>
```

## Running
Clone the project and rename `appsettings.EXAMPLE.json` to `appsettings.json` and fill it in with the appropriate DB connection info.
The frontend urls are a comma separated list, if you don't include it the browser will block the request because of CORS.
Run the project with
```sh
dotnet run
```

## Testing
```
dotnet test
```
