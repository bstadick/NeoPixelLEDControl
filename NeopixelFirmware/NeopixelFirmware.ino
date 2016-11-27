#include <EEPROM.h>
#include <Adafruit_NeoPixel.h>

#define PIN 0
#define MONPIN 1
#define ADDRESS 0
#define NMEALENGTH 22
#define MAXPIXELS 60

#define MODE_SOLID 1
#define MODE_FLAIR 2
#define MODE_CHRISTMAS 3
#define MODE_RAINBOW 4
#define MODE_ALLRAINBOW 5

byte numPixels = 30; // case LEDs
byte numMonitorPixels = 30; // enough for 4 monitors with 15 each
byte wait = 50;
byte mode = MODE_SOLID;

uint32_t solidColor;

char buff[255];

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
    
    setMonitorFromMemory(); // load previous configurations
}

void loop() {
    serialCheck();
    
    switch(mode){
        case MODE_SOLID:
            solid(solidColor);
        break;
        case MODE_FLAIR:
            // wipes a solid color read, then green, then blue. Does this 5 times
            for(int i = 0; i < 5; i++) {
                colorWipe(strip.Color(255, 0, 0), wait); // Red
                
                serialCheck(); // perform periodic serial checks between each pre-defined pattern call
                if(mode != MODE_FLAIR) { // serial says not to use flair now so break from this loop to change to solid color
                    break;
                }
                
                colorWipe(strip.Color(0, 255, 0), wait); // Green
                
                serialCheck();
                if(mode != MODE_FLAIR) {
                    break;
                }
                
                colorWipe(strip.Color(0, 0, 255), wait); // Blue
                
                serialCheck();
                if(mode != MODE_FLAIR) {
                    break;
                }
            }

            // performs a rainbow wipe five times
            for(int i = 0; i < 5; i++) {
                rainbow(20);
                serialCheck();
                if(mode != MODE_FLAIR) {
                    break;
                }
            }

            // performs a rainbow cycle five times
            for(int i = 0; i < 5; i++) {
                rainbowCycle(20);
                serialCheck();
                if(mode != MODE_FLAIR) {
                    break;
                }
            }
        break;
        case MODE_CHRISTMAS:
            // wipes a solid color read, then green. Does this 5 times
            for(int i = 0; i < 5; i++) {
                colorWipe(strip.Color(255, 0, 0), wait); // Red
                
                serialCheck(); // perform periodic serial checks between each pre-defined pattern call
                if(mode != MODE_CHRISTMAS) { // serial says not to use flair now so break from this loop to change to solid color
                    break;
                }
                
                colorWipe(strip.Color(0, 255, 0), wait); // Green
                
                serialCheck();
                if(mode != MODE_CHRISTMAS) {
                    break;
                }
                
                colorWipe(strip.Color(255, 255, 0), wait); // Green
                
                serialCheck();
                if(mode != MODE_CHRISTMAS) {
                    break;
                }
            }

            // marches red, green and gold
            march(strip.Color(255, 0, 0), strip.Color(0, 255, 0), strip.Color(255, 255, 0), 500, 100);
            serialCheck();

            // alternating red/green pattern
            alternate(strip.Color(255, 0, 0), strip.Color(0, 255, 0), 500, 100);
            serialCheck();
        break;
        case MODE_RAINBOW:
            // performs a rainbow wipe
            rainbowCycle(20);
            serialCheck();
            if(mode != MODE_RAINBOW) {
                break;
            }
        break;
        case MODE_ALLRAINBOW:
            // performs a rainbow wipe
            allRainbowCycle(20);
            serialCheck();
            if(mode != MODE_ALLRAINBOW) {
                break;
            }
        break;
    }
}

// This function is called periodically to check for new messages from the desktop.
void serialCheck() {
    uint8_t numBytes;
    uint8_t numRead;
    int i;
    
    int avail = Serial.available();
    
    // message format (string a bytes with no delimiters, spaces added for clarity)
    // $ msgLen r1 g1 b1 n1 r2 g2 b2 n2 r3 g3 b3 n3 r4 g4 b4 n4 rC gC bC nC mode
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
    }
    else if(avail >= 2) { // inquiry for current saved values (currently not working)  
        if(Serial.find("%")) {
            if(Serial.read() == '%') {
                Serial.print('$');
                Serial.print(21);
                for(i = 0; i < 21; i++) {
                    Serial.print(EEPROM.read(ADDRESS + i));
                }
            }
        }
    }
    setMonitorFromMemory();
}

// reads the memory to set the monitor backlight and case lights to the previous state
void setMonitorFromMemory() {
    // set monitor backlighting
    if(mode != MODE_ALLRAINBOW) {
        setMonitorBacklight(
            monitorStrip.Color(EEPROM.read(ADDRESS), EEPROM.read(ADDRESS+1), EEPROM.read(ADDRESS+2)),      EEPROM.read(ADDRESS+3),
            monitorStrip.Color(EEPROM.read(ADDRESS+4), EEPROM.read(ADDRESS+5), EEPROM.read(ADDRESS+6)),    EEPROM.read(ADDRESS+7),
            monitorStrip.Color(EEPROM.read(ADDRESS+8), EEPROM.read(ADDRESS+9), EEPROM.read(ADDRESS+10)),   EEPROM.read(ADDRESS+11),
            monitorStrip.Color(EEPROM.read(ADDRESS+12), EEPROM.read(ADDRESS+13), EEPROM.read(ADDRESS+14)), EEPROM.read(ADDRESS+15)
        );
    }
    
    // set case LEDs
    mode = EEPROM.read(ADDRESS+20);
    numPixels = EEPROM.read(ADDRESS+19);
    solidColor = strip.Color(EEPROM.read(ADDRESS+16), EEPROM.read(ADDRESS+17), EEPROM.read(ADDRESS+18));
}

// sets the monitor backlights
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

