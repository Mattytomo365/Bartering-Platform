# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY ["Web/Web.csproj", "Web/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]
RUN dotnet restore "Web/Web.csproj"
# Copy everything else and build & publish
COPY . .
WORKDIR "/src/Web"
RUN dotnet publish "Web.csproj" -c Release -o /app/publish #--no-restore

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Web.dll"]