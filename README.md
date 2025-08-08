# MAV AWS Playground

## 1. Introduction

### 1.1 Purpose of this repository

The intention for this repository is to act as a personal 'Playground' to demonstrate how to use the core AWS messaging features.

### 1.2 Key features

 - Publishing events vis AWS SNS topics
 - Processing received messages from AWS SQS message queues
 - Demonstrating an approach to register a queuelistener and processor for a single message type per queue (see `Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations.QueueListener<T>` and `Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations.QueuePoller<T>`)
 - Demonstrating an approach to register a queuelistener and processor for a multiple message types per queue (see `Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations.QueueListenerMultiType` and `Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations.QueuePollerMultiType`). This approach uses the message subject to determine the `Livestock.Cas.Infrastructure.Messaging.Handlers.IMessageHandler<>` that will handle the specific message type.

**Note**. Please run the API using the Docker Compose configuration which is set up to run using `localstack` in the container to test AWS connectivity.
