// mqtt_mysql_logger.cpp
#include <iostream>
#include <string>
#include <sstream>
#include <thread>
#include <chrono>
#include <queue>
#include <mutex>
#include <condition_variable>
#include <atomic>
#include <csignal>

#include <mqtt/async_client.h>

#include <mysql_connection.h>
#include <mysql_driver.h>
#include <mysql_error.h>
#include <cppconn/prepared_statement.h>

// ---------------------- Config ----------------------
const std::string MQTT_BROKER    = "tcp://192.168.114.14";
const std::string MQTT_TOPIC     = "sensors/weather";  
const std::string MQTT_CLIENT_ID = "cpp_logger";

const std::string MYSQL_HOST = "tcp://127.0.0.1:3306";
const std::string MYSQL_USER = "root";
const std::string MYSQL_PASS = "password";
const std::string MYSQL_DB   = "weather";

// Optional tuning
constexpr int      QOS                 = 1;
constexpr int      KEEP_ALIVE_SECONDS  = 20;  
constexpr int      RECONNECT_MIN_MS    = 100;   
constexpr int      RECONNECT_MAX_MS    = 2000;

// ---------------------- Globals ----------------------
std::mutex qmx;
std::condition_variable qcv;
std::queue<std::string> inbox;
std::atomic<bool> stop_flag{false};

// ---------------------- MQTT Callback ----------------------
class callback : public virtual mqtt::callback {
public:
    void message_arrived(mqtt::const_message_ptr msg) override {
        {
            std::lock_guard<std::mutex> lk(qmx);
            inbox.push(msg->to_string());
        }
        qcv.notify_one();
    }

    void connection_lost(const std::string& cause) override {
        std::cerr << "[MQTT] Connection lost: " << (cause.empty() ? "(no cause)" : cause) << std::endl;
    }
};

// ---------------------- DB Worker ----------------------
void db_worker() {
    sql::mysql::MySQL_Driver* driver = sql::mysql::get_mysql_driver_instance();
    std::unique_ptr<sql::Connection> con;
    std::unique_ptr<sql::PreparedStatement> ps;

    auto ensure_conn = [&]() {
        if (!con) {
            con.reset(driver->connect(MYSQL_HOST, MYSQL_USER, MYSQL_PASS));
            con->setSchema(MYSQL_DB);
            ps.reset(con->prepareStatement(
                "INSERT INTO Data (temp, pressure, humidity) VALUES (?, ?, ?)"
            ));
        }
    };

    int backoff_ms = RECONNECT_MIN_MS;

    while (!stop_flag.load()) {
        std::unique_lock<std::mutex> lk(qmx);
        qcv.wait(lk, [] { return !inbox.empty() || stop_flag.load(); });
        if (stop_flag && inbox.empty()) break;

        std::string payload = std::move(inbox.front());
        inbox.pop();
        lk.unlock();

        try {
            ensure_conn();

            std::istringstream iss(payload);
            std::string sTemp, sPress, sHum;
            if (!std::getline(iss, sTemp, ',') ||
                !std::getline(iss, sPress, ',') ||
                !std::getline(iss, sHum, ',')) {
                std::cerr << "[Parse] Bad payload, expected 3 comma-separated values: " << payload << std::endl;
                continue;
            }

            double temp     = std::stod(sTemp);
            double pressure = std::stod(sPress);
            double humidity = std::stod(sHum);

            ps->setDouble(1, temp);
            ps->setDouble(2, pressure);
            ps->setDouble(3, humidity);
            ps->execute();

            backoff_ms = RECONNECT_MIN_MS;
        }
        catch (const sql::SQLException& e) {
            std::cerr << "[MySQL] " << e.what() << " (state=" << e.getSQLStateCStr() << ", code=" << e.getErrorCode() << ")\n";
            con.reset();
            ps.reset();
            std::this_thread::sleep_for(std::chrono::milliseconds(backoff_ms));
            backoff_ms = std::min(backoff_ms * 2, RECONNECT_MAX_MS);
        }
        catch (const std::exception& e) {
            std::cerr << "[Parse] " << e.what() << " while handling payload: " << payload << std::endl;
        }
    }

    while (!inbox.empty()) {
        inbox.pop();
    }
}

// ---------------------- Signal Handling ----------------------
void handle_sigint(int) {
    stop_flag = true;
    qcv.notify_all();
}

// ---------------------- Main ----------------------
int main() {
    std::signal(SIGINT, handle_sigint);
#ifdef SIGTERM
    std::signal(SIGTERM, handle_sigint);
#endif

    mqtt::async_client client(MQTT_BROKER, MQTT_CLIENT_ID);
    callback cb;
    client.set_callback(cb);

    mqtt::connect_options connOpts;
    connOpts.set_clean_session(false);
    connOpts.set_automatic_reconnect(true);
    connOpts.set_keep_alive_interval(std::chrono::seconds(KEEP_ALIVE_SECONDS));

    client.set_connected_handler([&client](const std::string&) {
        try {
            client.subscribe(MQTT_TOPIC, QOS)->wait();
            std::cout << "[MQTT] (Re)subscribed to " << MQTT_TOPIC << std::endl;
        } catch (const mqtt::exception& e) {
            std::cerr << "[MQTT] Resubscribe failed: " << e.what() << std::endl;
        }
    });

    std::thread worker(db_worker);

    try {
        client.connect(connOpts)->wait();
        std::cout << "[MQTT] Connected to broker: " << MQTT_BROKER << std::endl;

        client.subscribe(MQTT_TOPIC, QOS)->wait();
        std::cout << "[MQTT] Subscribed to: " << MQTT_TOPIC << std::endl;

        while (!stop_flag.load()) {
            std::this_thread::sleep_for(std::chrono::seconds(1));
        }

        try {
            client.unsubscribe(MQTT_TOPIC)->wait();
        } catch (...) {}
        try {
            client.disconnect()->wait();
        } catch (...) {}
    }
    catch (const mqtt::exception& e) {
        std::cerr << "[MQTT] Error: " << e.what() << std::endl;
    }

    stop_flag = true;
    qcv.notify_all();
    if (worker.joinable()) worker.join();

    std::cout << "Exited cleanly.\n";
    return 0;
}
