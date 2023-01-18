# Depth-quotes
Small pet project to demonstrate my programming skills

### The task is:

**Depth Quotes Producer**
* Taking a depth quotes from an Exchange of choice from the top 10
* Subscribe a quote for the Symbol provided in ENV Variable and push it to Nats.IO stream channel name you choose.

**Depth Quotes Consumer**
* Subscribe to a Nats.IO stream, get data and expose it to the WebSocket channel public.depth.$symbol_name

***Bonus task**
Client app:
* Make a client app React/Angular subscribing to a WebSocket and display data in the table with price levels

**Delivery**
* Put all apps in a docker-compose file, including Nats.
* The stack should run with docker-compose up command
* Extra features and add-ons are welcome

**Technical Restrictions:**
* You can use a simple version of the depth chart that produces 20-level snapshot updates.
* You should use recent package versions
* You can use single-node services without scale
* Using unit testing will be an advantage

**Hints**
* choose naming right
* take attention to code structure and code quality

### Architecture

![alt text](ArchitectureDiagram.png)

I used layered architecture with dividing into zones of responsibility to provide flexibility and scalability if new features needed to be added.
I relied mostly on KISS and Single Responsibility Principle of SOLID as fundamentals in my opinion.
Project Abstractions is like client library for exchanging messages between services, it contains objects that are serialized by producer and deserialized by consumer.
I used common interfaces for producer and consumer so they might be changed, it is possible to use another exchange instead of Binance or another transport instead of Nats.

### Might be added
* Configuration settings for switching between 100 and 1000 ms intervals of getting quotes;
* Scalability, using multiple instances of services. For example using Redis to store the names of pods that consume and produce quotes to ensure that only one instance is getting quotes of a particular symbol from the exchange at a time.