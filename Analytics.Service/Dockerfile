FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Analytics.Service/Analytics.Service.csproj"
RUN dotnet publish "Analytics.Service/Analytics.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "Analytics.Service.dll" ]
