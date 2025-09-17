# Bartering Platform

---

## Overview

Bartering Platform is a full-stack **.NET 8** & **Angular** microservices application designed to facilitate the exchange of goods between users.

Users have the ability to create/edit/delete listings, search & discover listings for a specified location, and manage their profiles after authentication.

---

## Technologies Used
- **Backend**: .NET 8, ASP.NET Core, EF Core, SQL Server 2022, RabbitMQ
- **Gateway**: Ocelot
- **Frontend**: Angular
- **Container/dev**: Docker, Docker Compose


---

## Architecture

Services are loosely coupled and independently deployable, all following **Clean Architecture** implemented through several NET class libraries:

- **ApiGatewayService**: Central entry point for client requests, routing to appropriate backend services.
- **DiscoveryService**: Consumes listing events and indexes/searches listings.
- **ListingService**: Manages item/service listings.
- **ProfileService**: Handles user profiles and related data.

**Command Query Responsibility Segragation** (CQRS) is adhered to within the microservices architecture, further improving the modularity & maintainability of the application's backend. The pattern is implemented using MediatR handlers to encapsulate logic fullfilling change of/reading state requests expressed through Command/Query classes.

This architecture was simplified to a **service + repository** approach within the `main` branch. Controllers accept request DTOs and call application service methods to enforce domain rules via the Listing aggregate. A clear command/query separation remains without a mediator or a separate read store.

The unconverted CQRS-heavy approach can be viewed within the `cqrs` branch

### Asynchronous integration (RabbitMQ)
Both versions persist via repositories and use **RabbitMQ** to decouple services:
- **Exchange:** `listing.events` (topic) with keys `listing.created`, `listing.updated`, `listing.deleted`
- **Consumer:** **DiscoveryService** subscribes (`listing.*`) and updates its search index
This enables resilience (messages queue if a service is offline) and loose coupling between write and read concerns.

---

## Setup Instructions

### Prerequisites
- [Docker](https://www.docker.com/) (to run full-stack) or [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (to run services locally)
- [Node.js](https://nodejs.org/) + [Angular CLI](https://angular.dev/tools/cli) (for frontend)

### Run with Docker

1. Copy environment variables (Docker Compose):
    ```
    cp deploy/.env.example deploy/.env
    ```
   Fill the required values in `.env`

2. From `deploy/`, start all services:
    ```
    docker compose up -d
    ```

3. (Optional) view logs:
    ```
    docker compose logs -f
    ```
Default endpoints:
- API Gateway: http://localhost:8080
- RabbitMQ UI: http://localhost:15672 (use credentials from .env)
- SQL Server: localhost,1433 (login sa + password from .env)

### Run without Docker

1. Copy the development settings file (within `Web/` of each service):
    ```
    cp appsettings.Development.json.example appsettings.Development.json
    ```
    Fill in local values.

2. In separate terminals:

    ```
    cd ListingService/Web   && dotnet run
    cd DiscoveryService/Web && dotnet run
    cd ProfileService/Web   && dotnet run
    cd ApiGatewayService/Web&& dotnet run

    ```

**Frontend:**

1. Navigate to the `BarterApp` directory

2. Start the development server:
    ```
    ng serve
    ```

---

## License

*Specify license here*

---
## Contributing

**Contributions are welcome!**  
Please fork the repository and submit a pull request with your changes.

---

## Contact

For any questions or feedback, feel free to reach out:

- **Email:** matty.tom@icloud.com
- **GitHub:** [Mattytomo365](https://github.com/Mattytomo365)
