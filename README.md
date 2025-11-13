# .NET + Kafka Demo

Folder structure:
- SharedModels/ (shared Order model)
- OrderProducer.Api/ (ASP.NET Core Web API that produces to Kafka topic `orders`)
- OrderProcessor.Worker/ (background worker that consumes `orders` and produces `orders_processed`)
- Inventory.Service/ (simple consumer subscribing to `orders_processed`)
- Analytics.Service/ (simple consumer subscribing to `orders_processed`)
- docker-compose.yml (runs Zookeeper and Kafka locally using Bitnami images)

## Quick start (local dev)
1. Ensure you have .NET 8 SDK and Docker installed.
2. Start Kafka:
   ```bash
   docker-compose up -d
   ```
   Wait for Kafka to be ready. You can create topics using Kafka CLI or allow automatic topic creation.
3. In one terminal run the API:
   ```bash
   cd OrderProducer.Api
   dotnet run
   ```
4. In other terminals run the worker and services:
   ```bash
   cd ../OrderProcessor.Worker
   dotnet run

   cd ../Inventory.Service
   dotnet run

   cd ../Analytics.Service
   dotnet run
   ```
5. POST an order:
   ```bash
   curl -X POST http://localhost:5000/api/orders \
     -H "Content-Type: application/json" \
     -d '{ "OrderId":"1", "CustomerId":"C1", "Items":[{"Id":"A1","Quantity":2}], "Total":10.5, "Timestamp":"2025-11-13T00:00:00Z" }'
   ```

## Notes
- The Confluent.Kafka client expects Kafka reachable at `localhost:9092` by default. If running Kafka in Docker, ensure `advertised.listeners` is configured so .NET clients can reach it.
- This is a minimal demo — in production add retries, error handling, schema registry or Protobuf/Avro, secure Kafka (SASL/TLS), observability and proper topic creation scripts.
