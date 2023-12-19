#craigslist-crawler-dotnet

## Test

### Task
It is necessary to implement a service with the following functionality in C# using the SDK .NET 6.0 framework.

### Requirements
Implement two console commands – Producer and Consumer.

* Producer:
   * Reads website addresses from a file.
   * The file name is passed as the launch argument.
   * Website addresses are written one per line.
   * For each site, Producer creates an event containing the site address in the RabbitMQ queue.

* Consumer:
   * Reads events from the RabbitMQ queue
   * Makes an HTTP GET request to the address specified in the event.
   * It must be possible to operate several Consumers simultaneously.
   * The received response is saved into a table in the MySQL 8.0 database containing the following fields:
   ```
      id – PK
      url – site address
      date – request date
      response – body of the response from the site
   ```

##### Requirements for submitted solutions:
* Completed assignments must be submitted in a zip archive.
* The source code must be compiled using MSVS 2022.
* The archive should not contain unused source code, resources or intermediate assembly files.
