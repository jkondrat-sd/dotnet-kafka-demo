## Kubernetes notes

These manifests provide a simple development deployment of the .NET services and a single-node Zookeeper + Kafka for development.
**Important:** For production use, use a proper Kafka operator (Strimzi, Confluent) or managed Kafka (Confluent Cloud, etc.).

Build the docker images (replace with your registry if pushing):
  docker build -t dotnet-kafka-demo/orderproducer:latest -f OrderProducer.Api/Dockerfile .
  docker build -t dotnet-kafka-demo/orderprocessor:latest -f OrderProcessor.Worker/Dockerfile .
  docker build -t dotnet-kafka-demo/inventory:latest -f Inventory.Service/Dockerfile .
  docker build -t dotnet-kafka-demo/analytics:latest -f Analytics.Service/Dockerfile .

Load images into your cluster (for kind/minikube) or push to container registry:
- For kind: `kind load docker-image dotnet-kafka-demo/orderproducer:latest`
- For minikube: `minikube image load dotnet-kafka-demo/orderproducer:latest`

Apply manifests:
  kubectl apply -f k8s-manifests/namespace.yaml
  kubectl apply -f k8s-manifests/configmap.yaml
  kubectl apply -f k8s-manifests/zookeeper.yaml
  kubectl apply -f k8s-manifests/kafka.yaml
  kubectl apply -f k8s-manifests/order-producer.yaml
  kubectl apply -f k8s-manifests/order-processor.yaml
  kubectl apply -f k8s-manifests/inventory-service.yaml
  kubectl apply -f k8s-manifests/analytics-service.yaml

After deployment, the API is exposed on NodePort 30080. Test by POSTing to http://<node-ip>:30080/api/orders
