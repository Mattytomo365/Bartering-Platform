### Run Locally
1. cp deploy/.env.example deploy/.env and fill values
2. docker compose up -d
3. docker compose logs -f (optional)

# per service folder (e.g., ApiGatewayService/Web)
copy appsettings.Development.json.example appsettings.Development.json
# fill in local values — never commit the real file
