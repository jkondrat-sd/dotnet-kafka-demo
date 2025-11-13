FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Inventory.Service/Inventory.Service.csproj"
RUN dotnet publish "Inventory.Service/Inventory.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "Inventory.Service.dll" ]
