Wi-Fi: h4prog
Wi-Fi password: 1234567890

Usernames used in the project: Nick / Root
Passwords used in the project: Datait2025! / password

IP addresses:
- ESXi (VM host): 172.31.0.10
- Linux server: 192.168.114.14
- Gateway: 172.31.0.1
- Broker: 192.168.114.14
- Database: 192.168.14.14:3306
- Grafana: 192.168.114.14:3000

# Project Overview

This project contains a weather station that collects weather data and stores it in a database. The data can be visualized using a third-party add-in.
An Arduino with a sensor is used to collect the weather data. The Arduino connects to Wi-Fi and publishes data via MQTT to a broker running on a Linux server.
The program2 file is a C++ client that subscribes to the data published by the Arduino through the MQTT server. This file is also hosted on the Linux server. The client receives the incoming weather data and stores it in a MySQL database.
The database is hosted on the server inside a Docker container.
There is also an ASP.NET Core backend in the project for creating and fetching weather data. However, in this setup it is only used for fetching, not for creating the data. The backend follows Clean Architecture principles to provide clear dependency direction, long-term maintainability, and good testability.
Grafana is used as the third-party visualization tool. It retrieves data directly from the database and displays it in charts.

# Running the System

To run the whole setup, you use SSH to connect to the Linux server: `ssh root@[IP address of Linux server]`
Once connected, the project runs in a virtual terminal so that the actual terminal can be used for other tasks.
To attach to the virtual terminal: tmux attach-session
To start the program: `./mqtt_logger_NP`

To stop it, press `Ctrl + C.`

This runs the C++ client that writes data to the database. The Arduino runs independently and continuously collects and publishes data.

# Viewing the Data

After the application is running, you can view the data in Grafana by visiting: `http://[Grafana IP address]/d/588b1b3e-e31b-494c-b1e0-af152854428b/hum?orgId=1&from=now-2d&to=now&timezone=browser`
