#include <EEPROM.h>
#include <Adafruit_NeoPixel.h>
//#include <NMEA.h>;


#define PIN 6
#define MONPIN 7
#define ADDRESS 0
#define NMEALENGTH 22

byte numPixels = 30;
byte wait = 50;

char buff[7];

// Message = $LED,command,R,G,B,time*checksum
// command = 0 to numPixels and A through Y (number -> individual LED, letter -> preset sequence)
// command = Z -> setup, with R = numPixels


// Parameter 1 = number of pixels in strip
// Parameter 2 = pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
Adafruit_NeoPixel strip = Adafruit_NeoPixel(numPixels, PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel monitorStrip = Adafruit_NeoPixel(numPixels, MONPIN, NEO_GRB + NEO_KHZ800);

//NMEA nmea = NMEA();

void setup() {
  Serial.begin(9600);
  
  Serial.setTimeout(100);

  strip.begin();
  monitorStrip.begin();
  strip.show(); // Initialize all pixels to 'off'
  monitorStrip.show();
  
  delay(1000);
  
  setMonitorFromMemory();
}

void loop() {
  
  setMonitorFromMemory();
  
  // Some example procedures showing how to display to the pixels:
  for(uint8_t i = 0; i < 10; i++) {
    colorWipe(strip.Color(255, 0, 0), wait); // Red
    delay(250);
    serialCheck();
    colorWipe(strip.Color(0, 255, 0), wait); // Green
    delay(250);
    serialCheck();
  }
//	colorWipe(strip.Color(0, 0, 255), wait); // Blue
//	rainbow(20);
//	rainbowCycle(20);

  march(strip.Color(255, 0, 0), strip.Color(0, 255, 0), strip.Color(255, 200, 0), 500, 100);
  alternate(strip.Color(255, 0, 0), strip.Color(0, 255, 0), 500, 100);
}

void serialCheck() {
  
  // $r1g1b1r2g2b2/ETX
  if(Serial.available() >= 7) {
   if(Serial.find("$")) {
     if(Serial.available() >= 6) {
       if(Serial.readBytesUntil((char)3, buff, 7) >= 6) {
         setMonitorBacklight(monitorStrip.Color(buff[0], buff[1], buff[2]), monitorStrip.Color(buff[3], buff[4], buff[5]));
         EEPROM.write(ADDRESS, buff[0]);
         EEPROM.write(ADDRESS+1, buff[1]);
         EEPROM.write(ADDRESS+2, buff[2]);
         EEPROM.write(ADDRESS+3, buff[3]);
         EEPROM.write(ADDRESS+4, buff[4]);
         EEPROM.write(ADDRESS+5, buff[5]);
       }
     }
   }
  }
}

void setMonitorFromMemory() {
  setMonitorBacklight(
    monitorStrip.Color(EEPROM.read(ADDRESS), EEPROM.read(ADDRESS+1), EEPROM.read(ADDRESS+2))
    , monitorStrip.Color(EEPROM.read(ADDRESS+3), EEPROM.read(ADDRESS+4), EEPROM.read(ADDRESS+5)));
}

void setMonitorBacklight(uint32_t  c1, uint32_t c2) {
  for(uint8_t i = 0; i < numPixels/2; i++) {
     monitorStrip.setPixelColor(i, c1);
  }
  monitorStrip.show();
  
  for(uint8_t i = 15; i < numPixels; i++) {
     monitorStrip.setPixelColor(i, c2);
  }
  monitorStrip.show();
}

