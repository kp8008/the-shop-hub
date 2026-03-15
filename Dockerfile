# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ECommerceAPI/ECommerceAPI.csproj", "ECommerceAPI/"]
RUN dotnet restore "ECommerceAPI/ECommerceAPI.csproj"

COPY . .
WORKDIR "/src/ECommerceAPI"
RUN dotnet build "ECommerceAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ECommerceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ECommerceAPI.dll"]
