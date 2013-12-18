#include <EEPROM.h>
#include <Adafruit_NeoPixel.h>
//#include <NMEA.h>;


#define PIN 6
#define ADDRESS 0
#define NMEALENGTH 22

byte numPixels = 30;
byte wait = 50;

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

//NMEA nmea = NMEA();

void setup() {
  Serial.begin(9600);
  Serial.setTimeout(10000);
  
  byte numPixelsValue = EEPROM.read(ADDRESS);
  
  Serial.setTimeout(100);

  strip.begin();
  strip.show(); // Initialize all pixels to 'off'
}

void loop() {
  
	//nmea.read();

	// Some example procedures showing how to display to the pixels:
  for(uint8_t i = 0; i < 10; i++) {
    colorWipe(strip.Color(255, 0, 0), wait); // Red
    delay(250);
    colorWipe(strip.Color(0, 255, 0), wait); // Green
    delay(250);
  }
//	colorWipe(strip.Color(0, 0, 255), wait); // Blue
//	rainbow(20);
//	rainbowCycle(20);

  march(strip.Color(255, 0, 0), strip.Color(0, 255, 0), strip.Color(255, 200, 0), 500, 100);
  alternate(strip.Color(255, 0, 0), strip.Color(0, 255, 0), 500, 100);
}


