# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (cached unless the csproj changes)
COPY ["WebApplication1.csproj", "./"]
RUN dotnet restore "WebApplication1.csproj"

# Copy the rest and publish
COPY . .
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# The aspnet:10.0 image serves HTTP on 8080 by default.
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "WebApplication1.dll"]
