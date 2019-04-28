# DeviceSimulator

Device Simulator is an application that helps in simulating device to cloud communication with Azure IoT Hub. It is possible to define JSON payload, and trigger message sending. Message sending can be also time triggered.

## Technical overview and future plans
Device Simulator is UWP application based on MVVMCross 6.x. The intention with cross-platform development is to offer simulator for MAC OS, iOS and Android in the near future.
The plan is to extend the simulator with features for randomizing messages, handling cloud to device messages, and simulating more than one device from one application. 

## Expressions
It is possible to use expression for generating random value(double or int). It is neccessary to include keyword "rnd" followed by min and max value separated with ":". For example:
```
{
   "Name":"Temperature",
   "Timestamp":"2016-12-31T01:00:03",
   "Value":"rnd:25.0:30"
}
```

If min or max value contains comma, random generated value will be double, otherwise int.
