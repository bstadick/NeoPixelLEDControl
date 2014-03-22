#include <EEPROM.h>
#include <Adafruit_NeoPixel.h>

#define PIN 6
#define MONPIN 7
#define ADDRESS 0
#define NMEALENGTH 22
#define MAXPIXELS 60

byte numPixels = 30; // chassis LEDs
byte numMonitorPixels = 30; // enough for 4 monitors with 15 each
byte wait = 50;
boolean useFlair = true;

char buff[255];

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
Adafruit_NeoPixel strip = Adafruit_NeoPixel(MAXPIXELS, PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel monitorStrip = Adafruit_NeoPixel(MAXPIXELS, MONPIN, NEO_GRB + NEO_KHZ800);

void setup() {

    Serial.begin(9600);
  
    Serial.setTimeout(200);

    strip.begin();
    monitorStrip.begin();

    strip.show(); // Initialize all pixels to 'off'
    monitorStrip.show();
  
    delay(100);
    
    // Test code
    /*EEPROM.write(ADDRESS + 16, 255);
    EEPROM.write(ADDRESS + 17, 255);
    EEPROM.write(ADDRESS + 18, 255);
    EEPROM.write(ADDRESS + 19, 30);
    EEPROM.write(ADDRESS + 20, 1);
    EEPROM.write(ADDRESS + 5, 255);
    EEPROM.write(ADDRESS + 6, 255);
    EEPROM.write(ADDRESS + 7, 30);*/
  
    setMonitorFromMemory();
}

void loop() {
  
    serialCheck();
    setMonitorFromMemory();
    
    if(useFlair) {
        // Some example procedures showing how to display to the pixels:
        for(int i = 0; i < 5; i++) {
            
            colorWipe(strip.Color(255, 0, 0), wait); // Red
            
            serialCheck();
            if(!useFlair)
                break;
                
            colorWipe(strip.Color(0, 255, 0), wait); // Green
            
            serialCheck();
            if(!useFlair)
                break;
                
            colorWipe(strip.Color(0, 0, 255), wait); // Blue
            
            serialCheck();
            if(!useFlair)
                break;
        }
    }
    
    if(useFlair) {
        for(int i = 0; i < 5; i++) {
            rainbow(20);
            serialCheck();
            if(!useFlair)
                break;
        }
    }
    
    if(useFlair) {
        for(int i = 0; i < 5; i++) {
            rainbowCycle(20);
            serialCheck();
            if(!useFlair)
                break;
        }
    }
    
    if(useFlair) {
        //march(strip.Color(255, 0, 0), strip.Color(0, 255, 0), strip.Color(0, 0, 255), 500, 100);
        //alternate(strip.Color(255, 0, 0), strip.Color(0, 255, 0), 500, 100);
    }
}

void serialCheck() {
  
    uint8_t numBytes;
    uint8_t numRead;
    int i;
    
    int avail = Serial.available();
    
    // message format (string a bytes with no delimiters, spaces added for clarity)
    // $ msgLen r1 g1 b1 n1 r2 g2 b2 n2 r3 g3 b3 n3 r4 g4 b4 n4 rC gC bC nC flair
    if(avail >= 23) {
        if(Serial.find("$")) {
            numBytes = Serial.read();
            if(Serial.available() >= numBytes) {
                if((numRead = Serial.readBytes(buff, numBytes)) >= numBytes) {
                    for(i = 0; i < numRead; i++) {
                        EEPROM.write(ADDRESS + i, buff[i]);
                    }
                }
            }
        }
        setMonitorFromMemory();
    }
    else if(avail >= 2) { // inquiry for current saved values       
        if(Serial.find("%")) {
            if(Serial.read() == '%') {
                Serial.print('$');
                Serial.print(21);
                for(i = 0; i < 21; i++)
                    Serial.print(EEPROM.read(ADDRESS + i));
            }
        }
    }
}

void setMonitorFromMemory() {
    // set monitor backlighting
    setMonitorBacklight(
            monitorStrip.Color(EEPROM.read(ADDRESS), EEPROM.read(ADDRESS+1), EEPROM.read(ADDRESS+2)),      EEPROM.read(ADDRESS+3),
            monitorStrip.Color(EEPROM.read(ADDRESS+4), EEPROM.read(ADDRESS+5), EEPROM.read(ADDRESS+6)),    EEPROM.read(ADDRESS+7),
            monitorStrip.Color(EEPROM.read(ADDRESS+8), EEPROM.read(ADDRESS+9), EEPROM.read(ADDRESS+10)),   EEPROM.read(ADDRESS+11),
            monitorStrip.Color(EEPROM.read(ADDRESS+12), EEPROM.read(ADDRESS+13), EEPROM.read(ADDRESS+14)), EEPROM.read(ADDRESS+15)
    );
    
    // set chassis LEDs
    if(EEPROM.read(ADDRESS+20) == 1) { // turn flair off
        numPixels = EEPROM.read(ADDRESS+19);
        useFlair = false;
        int i;
        uint32_t color = strip.Color(EEPROM.read(ADDRESS+16), EEPROM.read(ADDRESS+17), EEPROM.read(ADDRESS+18));
        for(i = 0; i < numPixels; i++)
            strip.setPixelColor(i, color);
            
        strip.show();
    }
    else
        useFlair = true;
}

void setMonitorBacklight(uint32_t  c1, uint8_t n1, uint32_t c2, uint8_t n2, uint32_t c3, uint8_t n3, uint32_t c4, uint8_t n4) {

    uint8_t i;

    for(i = 0; i < n1; i++) {
        monitorStrip.setPixelColor(i, c1);
    }
 
    if(n1 && n2) {
        for(i = n1; i < n2; i++) {
            monitorStrip.setPixelColor(i, c2);
        }
        if(n2 && n3) {
            for(i = n2; i < n3; i++) {
                monitorStrip.setPixelColor(i, c3);
            }
            if(n3 && n4) {
                for(i = n3; i < n4; i++) {
                    monitorStrip.setPixelColor(i, c4);
                }
            }
        }
    }

    monitorStrip.show();
}

