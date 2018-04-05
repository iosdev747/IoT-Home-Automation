#include <ESP8266WiFi.h>
#include <RH_ASK.h>
#include <SPI.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <DHT.h>
#define DATA_lines 101
#define DHTTYPE DHT11
#define DHTPIN 0
DHT dht(DHTPIN, DHTTYPE);

RH_ASK driver(2000, 13, 2);
Adafruit_SSD1306 display(-1);

struct Switch {
  boolean isEmpty = true;
  boolean state = false;  //false = LOW
  String Name;
  String URL;
} switches[99];
boolean DATAChanged = true;
String DATA[DATA_lines];
String DATADHTReadings;
String action;
int iterator = 0;
int Temperature;
int Humidity;
int readDHT;

const char* ssid = "iOSdev";
const char* password = "helloworld";

WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  delay(10);
  driver.init();
  WiFi.begin(ssid, password);

  int temp = 0;
  while (WiFi.status() != WL_CONNECTED) {
    delay(50);
  }
  dht.begin();
  server.begin();

  delay(3000);
}

void loop() {

  if (DATAChanged)
  {
    DATAChanged = false;
    readDHT = 0;
  }
  readDHT++;
  if (readDHT == 100) {
    Temperature = dht.readTemperature();
    Humidity = dht.readHumidity();
    if ((!(isnan(Temperature) || isnan(Humidity))) && Temperature < 99 && Humidity < 99) {
      float feel = dht.computeHeatIndex(Temperature, Humidity, false);
      DATADHTReadings = "T:" + String(Temperature) + "C  H:" + String(Humidity) + "% F:" + String(feel) + "C";
    }
    readDHT = 0;
  }

  if ( Temperature > 60 && Temperature < 150)
  {
    int i = 5;
    while (i--)
    {
      delay(10);
      Temperature = dht.readTemperature();
      if (!(Temperature > 80))
      {
        break;
      }
    }
    Serial.println("temp");
  }

  WiFiClient client = server.available();
  if (!client) {
    return;
  }

  //new client connected
  //  while (!client.available()) {
  //    delay(1);
  //  }

  ///added
  if (client.available() > 0)
  {
    String req = client.readStringUntil('\r');
    Serial.println(req);
    req = req.substring(0, req.length() - 9);
    delay(1);
    client.flush();
    //Serial.println(req);
    String module_number, switch_number, state, URL_string;
    //  long module_number_val;
    //  int switch_number_val, state_val;
    int index = req.indexOf("/iosdev/");
    if (index != -1) {
      int index_threshold = index + 8;
      URL_string = req.substring(index_threshold, index_threshold + 10);
      //Serial.println("URL extracted: " + URL_string);
      Serial.println(URL_string);
      for (int i = 0; i < 99; i++) {
        ////Serial.print(i + " ");
        //Serial.println("switches[i].URL: " + switches[i].URL + ", URL_string: " + URL_string);
        if (switches[i].URL.substring(0, 9) == URL_string.substring(0, 9)) {
          switches[i].state = (URL_string.substring(9, 10).toInt() == 0 ? false : true);
          action = switches[i].Name + " : " + (switches[i].state ? "ON" : "OFF");
          //Serial.println(switches[i].state);
          break;
        }
      }
    }
    else {
      int index = req.indexOf("/access/");
      if (index != -1) {
        int index_threshold = index + 8, data_xor = 0, k = 0;
        boolean switcher = false; //false for name
        int previous_index = 0;
        String Message, Name, URL;
        Message = req.substring(index_threshold, req.length());
        ////Serial.println("MSG: " + Message);
        for (int i = 0; i < 99; i++)
        {
          switches[i].isEmpty = true;
        }
        for (int j = 0; j < Message.length(); j++) {
          if (Message.substring(j, j + 1) == "/") {
            if (switcher) {
              URL = Message.substring(previous_index, j);
              data_xor ^= 1;
              ////Serial.println("url :" + URL + " at j = " + j + " XOR>" + data_xor);
            }
            else {
              Name = Message.substring(previous_index, j);
              ////Serial.println("name :" + Name + " at j = " + j + " XOR>" + data_xor);
              data_xor ^= 2;
            }
            previous_index = j + 1;
            switcher = !switcher;
          }
          if (data_xor == 3) {
            data_xor = 0;
            switches[k].Name = Name;
            switches[k].URL = URL;
            switches[k].isEmpty = false;
            switches[k].state = (URL.substring(9, 10).toInt() == 1 ? true : false);
            ////Serial.println("Name: " + Name + " URL: " + URL + " at k = " + k + ">>>>" + switches[k].state);
            k++;
            delay(50);
            Serial.println(URL);
          }
        }
        action = String(k) + " accessories refresh";
      }
      else {
        int index = req.indexOf("/remove/");
        if (index != -1)
        {
          int index_threshold = index + 8;
          String URL_delete = req.substring(index_threshold, index_threshold + 10);
          for (int i = 0; i < 99; i++) {
            //////Serial.print(i + " ");
            ////Serial.println("switches[i].URL: " + switches[i].URL + ", URL_delete: " + URL_delete);
            if (switches[i].URL.substring(0, 9) == URL_delete.substring(0, 9)) {
              switches[i].isEmpty = true;
              action = switches[i].Name + " removed";
              break;
            }
          }
        }
      }
    }
    client.flush();
    String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nMessage send to arduino";
    //  s += str;
    s += "</html>\n";
    client.print(s);
    delay(1);
    //////Serial.println("Client disonnected");
  }
}
