#include <ESP8266WiFi.h>
#include <RH_ASK.h>
#include <SPI.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <DHT.h>
#define DATA_lines 101
#define GASPIN A0
#define PIRPIN 12///////
#define BUZZERPIN 14//////
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
int line_number[8] = {0, 8, 16, 24, 32, 40, 48, 56};
unsigned long Alarm_UID;
unsigned long Alarm_milli;
unsigned long Alarm_milli_recordedAt;
boolean DATAChanged = true;
boolean isTimeUpdate = false;
boolean newtime = false;
String DATA[DATA_lines];
String DATATime;
String Atime;
String DATADHTReadings;
String action;
int iterator = 0;
int Temperature;
int Humidity;
int readDHT;
int pirValue; // variable to store read PIR Value
unsigned long currentMillis;
unsigned long updatedMillis;

const char* ssid = "iOSdev";
const char* password = "helloworld";

#if (SSD1306_LCDHEIGHT != 64)
#error("Height incorrect!");
#endif

void clearpixel(int x1, int y1, int x2, int y2, boolean isBlack = true, boolean doDisplay = true)
{
  if (isBlack)
  {
    for (int x = x1; x < x2; x++)
    {
      for (int y = y1; y < y2; y++)
      {
        display.drawPixel(x, y, BLACK);
      }
    }
  }
  else
  {
    for (int x = x1; x < x2; x++)
    {
      for (int y = y1; y < y2; y++)
      {
        display.drawPixel(x, y, WHITE);
      }
    }
  }
  if (doDisplay)
    display.display();
}

void printOLED(String str, int x = 0, int y = 0, boolean doClear = false)
{
  Serial.println("called" + String(doClear));
  if (doClear)
  {
    display.clearDisplay();
  }
  display.setTextSize(1);
  display.setTextColor(WHITE);
  display.setCursor(x, y);
  display.println(str);
  display.display();
}

void printDATA()
{
  display.clearDisplay();
  iterator = 0;
  for (int i = 0; i < 99; i++)
  {
    if (!switches[i].isEmpty)
    {
      DATA[iterator] = switches[i].Name + " : " + (switches[i].state ? "ON" : "OFF");
      if (iterator % 5 == 0) {
        delay(5000);
        display.clearDisplay();
      }
      int temp = iterator % 5 + 2;
      printOLED(DATA[iterator++], 0, line_number[temp]);
    }
  }
  printOLED(action, 0, 56);
}

void buzzMode(boolean buzzmode = true)
{
  digitalWrite(BUZZERPIN, (buzzmode ? HIGH : LOW));
}

void SendRF(String url) {
  char msg[10];
  url.toCharArray(msg, 11);
  //  char *msg_pointer = msg;
  driver.send((uint8_t *)msg, strlen(msg));
  if (driver.waitPacketSent(3000))
  {
    display.drawPixel(127, 63, BLACK);
  }
  else
  {
    display.drawPixel(127, 63, WHITE);
  }
}

void calTime()
{
  unsigned long duration = millis() - updatedMillis;
  unsigned long DATATime_millis = DATATime.substring(11, DATATime.length()).toInt();
  unsigned long long ActualTime_millis = duration + DATATime_millis;
  int Hour, Minute, Second, Millisecond;
  Second = (int) (ActualTime_millis / 1000) % 60 ;
  Minute = (int) ((ActualTime_millis / (1000 * 60)) % 60);
  Hour   = (int) ((ActualTime_millis / (1000 * 60 * 60)) % 24);
  if (isTimeUpdate) {
    Atime = DATATime.substring(0, 10) + " " + String(Hour) + ":" + String(Minute) + ":" + String(Second);
  }
  else {
    Atime = String(Hour) + ":" + String(Minute) + ":" + String(Second);
  }
  Serial.println("got before: " + DATATime + " and after func Atime: " + Atime);
}

WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  display.begin(SSD1306_SWITCHCAPVCC, 0x3C);  //my OLED is on 0x3C address
  display.clearDisplay();
  display.display();
  delay(10);
  driver.init();
  printOLED("Server Started", 0, 0, true);
  printOLED("Connecting to ", 0, 8);
  printOLED(ssid, 0, 16);
  WiFi.begin(ssid, password);

  int temp = 0;
  while (WiFi.status() != WL_CONNECTED) {
    delay(50);
    printOLED(".", temp++, 24);
  }
  printOLED("Wifi connected", 20, 32);
  dht.begin();
  pinMode(PIRPIN, INPUT);
  pinMode(BUZZERPIN, OUTPUT);
  printOLED("Sensors loaded", 20, 40);
  server.begin();
  printOLED("Server Started", 20, 48);

  delay(3000);
  display.clearDisplay();
  display.display();
}

void loop() {

  if (DATAChanged)
  {
    printDATA();
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
      clearpixel(0, 8, 127, 15, true, false);
      printOLED(DATADHTReadings, 0, 8);
    }
    clearpixel(0, 0, 127, 7, true, false);
    calTime();
    printOLED(Atime, 0, 0);
    readDHT = 0;
  }

  if (Alarm_UID != 0 && (((millis() - Alarm_milli_recordedAt) - Alarm_milli) < 2000))
  {
    Serial.println("sending RF");
    SendRF(String(Alarm_UID));
    Alarm_milli_recordedAt = 0;
    Alarm_milli = 0;
    Serial.println(Alarm_UID);
    Alarm_UID = 0;
    for (int i = 0; i < 99; i++) {
      if (switches[i].URL.substring(0, 9).toInt() == ((Alarm_UID) - Alarm_UID % 10) / 10) {
        switches[i].state = Alarm_UID % 10 == 0 ? false : true;
        DATAChanged = true;
        break;
      }
    }
    Serial.println("sent RF");
  }
  //  clearpixel(0, 0, 127, 7, true, false);
  //  printOLED(String(millis()), 0, 0);

  if (digitalRead(PIRPIN))
  {
    buzzMode();
    Serial.print(digitalRead(PIRPIN));
  }

  if (analogRead(GASPIN) > 1000)
  {
    int i = 5;
    while (i--)
    {
      delay(10);
      if (!(analogRead(GASPIN) > 1000))
      {
        break;
      }
    }
    buzzMode();
    Serial.println(analogRead(GASPIN));
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
    buzzMode();
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
      SendRF(URL_string);
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
            SendRF(URL);
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
        else {
          int index = req.indexOf("/setime/");
          newtime = true;
          if (index != -1)
          {
            int index_threshold = index + 8;
            String _time = req.substring(index_threshold);
            Serial.println("time got from URL: " + _time);
            DATATime = _time.substring(0, 2) + ":" + _time.substring(2, 4) + ":" + _time.substring(4, 8) + " " + _time.substring(8);
            //          DATATime = _time.substring(index_threshold, index_threshold + 2) + ":" + _time.substring(index_threshold + 2, index_threshold + 4) + ":" + _time.substring(index_threshold + 4, index_threshold + 6) + " " + _time.substring(index_threshold + 6);
            //            DATATime = DATATime.substring(0, DATATime.length() - 1);
            Serial.println("GET:" + DATATime);
            updatedMillis = millis();
            isTimeUpdate = true;
          }
          else {
            int index = req.indexOf("/action/");
            if (index != -1)
            {
              int index_threshold = index + 8;
              String action = req.substring(index_threshold);
              Serial.println(action);
              printOLED(action, 0, 56);
            }
            else {
              int index = req.indexOf("/salarm/");
              if (index != -1)
              {
                int index_threshold = index + 8;
                String action = req.substring(index_threshold, index_threshold + 10);
                Alarm_UID = action.toInt();
                Alarm_milli_recordedAt = millis();
                String milli_str = req.substring(index_threshold + 10);
                Alarm_milli = milli_str.toInt();
                Serial.println(action);
                action = "Alarm for Module ID: " + String((Alarm_UID - (Alarm_UID % 10)) / 10);
              }
              else {
                ////Serial.println("URL INCORRECT");
                client.stop();
                return;
              }
            }
          }
        }
      }
    }
    if (newtime)
    {
      DATAChanged = false;
      newtime = false;
    }
    else
    {
      DATAChanged = true;
    }
    client.flush();
    String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nMessage send to arduino";
    //  s += str;
    s += "</html>\n";
    client.print(s);
    buzzMode(false);
    delay(1);
    //////Serial.println("Client disonnected");
  }
}
