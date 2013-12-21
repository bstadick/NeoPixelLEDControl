
// Fill the dots one after the other with a color
void colorWipe(uint32_t c, uint8_t wait) {
	for(uint16_t i=0; i<numPixels; i++) {
		strip.setPixelColor(i, c);
		strip.show();
		delay(wait);
	}
}

void rainbow(uint8_t wait) {
	uint16_t i, j;

	for(j=0; j<256; j++) {
              serialCheck();
		for(i=0; i<numPixels; i++) {
			strip.setPixelColor(i, Wheel((i+j) & 255));
		}
		strip.show();
		delay(wait);
	}
}

// Slightly different, this makes the rainbow equally distributed throughout
void rainbowCycle(uint8_t wait) {
  uint16_t i, j;
  for(j=0; j<256*5; j++) { // 5 cycles of all colors on wheel
    serialCheck();
    
    for(i=0; i< numPixels; i++) {
      strip.setPixelColor(i, Wheel(((i * 256 / strip.numPixels()) + j) & 255));
    }
    strip.show();
    delay(wait);
  }
}

// Input a value 0 to 255 to get a color value.
// The colors are a transition r - g - b - back to r.
uint32_t Wheel(byte WheelPos) {
	if(WheelPos < 85) {
		return strip.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
	} else if(WheelPos < 170) {
		WheelPos -= 85;
		return strip.Color(255 - WheelPos * 3, 0, WheelPos * 3);
	} else {
		WheelPos -= 170;
		return strip.Color(0, WheelPos * 3, 255 - WheelPos * 3);
	}
}

// Marches three colors along the strip
void march(uint32_t c1, uint32_t c2, uint32_t c3, uint8_t wait, uint16_t durations) {
  
  uint8_t state = 0;
  for(uint16_t j = 0; j < durations; j++) { 
    serialCheck();
    
    if(state == 0) {
      for(uint16_t i = 2; i < numPixels; i = i + 3) {
          strip.setPixelColor(i - 2, c1);
          strip.setPixelColor(i - 1, c2);
          strip.setPixelColor(i, c3);
        }
    }
    else if(state == 1){
      for(uint16_t i = 2; i < numPixels; i = i + 3) {
          strip.setPixelColor(i - 2, c2);
          strip.setPixelColor(i - 1, c3);
          strip.setPixelColor(i, c1);
        }
    }
    else {
      for(uint16_t i = 2; i < numPixels; i = i + 3) {
          strip.setPixelColor(i - 2, c3);
          strip.setPixelColor(i - 1, c1);
          strip.setPixelColor(i, c2);
        }
      }
      strip.show();
      state++;
      if(state > 2)
        state = 0;
      delay(wait);
  }
}

// Switches every other light between two colors
void alternate(uint32_t c1, uint32_t c2, uint8_t wait, uint16_t durations) {
  
  boolean state = true;
  for(uint16_t j = 0; j < durations; j++) { 
    serialCheck();
    
    if(state) {
      for(uint16_t i = 1; i < numPixels; i = i + 2) {
          strip.setPixelColor(i - 1, c1);
          strip.setPixelColor(i, c2);
        }
    }
    else {
      for(uint16_t i = 1; i < numPixels; i = i + 2) {
          strip.setPixelColor(i - 1, c2);
          strip.setPixelColor(i, c1);
        }
    }
    strip.show();
    state = !state;
    delay(wait);
  }
}

