#include <Wire.h>
#include <Adafruit_BME280.h>
#include <WiFiS3.h>
#include <WiFiClient.h>
#include <ArduinoMqttClient.h>

Adafruit_BME280 bme;

const char* ssid     = "h4prog";
const char* password = "1234567890";

const char* mqttBroker = "192.168.114.14"; // your broker IP
const int   mqttPort   = 1883;              // change if your broker uses a non-default port
const char* topic      = "sensors/weather"; // <-- add your topic

WiFiClient   wifiClient;
MqttClient   mqttClient(wifiClient);

void setup() {
  Serial.begin(9600);

  if (!bme.begin(0x76)) {
    Serial.println("BME280 not found!");
    while (1) {}
  }

  // WiFi
  Serial.print("Connecting to WiFi");
  while (WiFi.begin(ssid, password) != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  while (WiFi.localIP() == INADDR_NONE) {
    delay(100);
  }
  Serial.println("\nConnected!");
  Serial.print("IP: "); Serial.println(WiFi.localIP());

  // MQTT
  Serial.println("Connecting to MQTT broker...");
  while (!mqttClient.connect(mqttBroker, mqttPort)) {
    Serial.print("MQTT connection failed! Error code = ");
    Serial.println(mqttClient.connectError());
    Serial.println("Retrying in 5 seconds...");
    delay(5000);
  }
  Serial.println("Connected to MQTT broker!");
}

void loop() {
  mqttClient.poll(); // keep connection alive

  if (!mqttClient.connected()) {
    Serial.println("MQTT disconnected, trying to reconnect...");
    while (!mqttClient.connect(mqttBroker, mqttPort)) {
      Serial.print("MQTT reconnect failed! Error code = ");
      Serial.println(mqttClient.connectError());
      delay(5000);
    }
    Serial.println("MQTT reconnected!");
  }

  publishBme();
  delay(10000); // 10 seconds
}

void publishBme() {
  float temp     = bme.readTemperature();
  float pressure = bme.readPressure() / 100.0F; // hPa
  float hum      = bme.readHumidity();

  // CSV payload: temp,pressure,humidity
  String payload = String(temp, 2) + "," + String(pressure, 2) + "," + String(hum, 2);

  if (mqttClient.beginMessage(topic)) {
    mqttClient.print(payload);
    mqttClient.endMessage();
    Serial.print("Published: "); Serial.println(payload);
  } else {
    Serial.println("Failed to start MQTT message (not connected?)");
  }
}
