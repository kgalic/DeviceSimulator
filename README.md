# Message Publisher

Message Publisher Simulator is an application that helps in simulating a single device communication with IoT Hub, or any other message publisher that sends messages/events to Azure EventGrid and/or Azure  ServiceBus. It is possible to define JSON payload, and trigger message sending. Message sending can be also time triggered.

# IoT Hub Device Simulator

Message Publisher Simulator can work as the device simulator. It supports device-to-cloud communication, direct-method communication, and cloud-to-device communication. In order to enable communication, the connection string needs to be provided.

# Event Grid Publisher

Message Publisher Simulator can send events to the event grid topic. In order to enable communication, the following parameters need to be provided:
* endpoint URL 
* access key 
* topic name  
* subject
* data version
* event type.

Currently, it works only with Event Grid schema.

# Service Bus Queue/Topic message publisher

Message Publisher Simulator can send events to the service bus queue/topic. In order to enable communication, the following parameters need to be provided:
* connection string
* queue/topic name
* select if the target is either queue or topic.

## Expressions
It is possible to use the expression for generating random value(double or int). It is necessary to include the keyword "rnd" followed by min and max value separated with ":". For example:
```
{
   "Name":"Temperature",
   "Timestamp":"2016-12-31T01:00:03",
   "Value":"rnd:25.0:30"
}
```

If min or max value contains a comma, the randomly generated value will be double, otherwise int.

## Technical overview
Message Publisher Simulator is UWP(min version 1809, build 17763) application based on MVVMCross 6.x. The intention with cross-platform development is to offer simulator for MAC OS, iOS and Android in the near future.

## Next Steps
* MAC OS version of the application
* Extend the expression to support adding timestamp instead of hardcoding in the message payload
