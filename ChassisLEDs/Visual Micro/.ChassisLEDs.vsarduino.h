#ifndef _VSARDUINO_H_
#define _VSARDUINO_H_
//Board = Arduino Micro
#define __AVR_ATmega32u4__
#define __AVR_ATmega32U4__
#define ARDUINO 105
#define ARDUINO_MAIN
#define __AVR__
#define __avr__
#define F_CPU 16000000L
#define __cplusplus
#define __inline__
#define __asm__(x)
#define __extension__
#define __ATTR_PURE__
#define __ATTR_CONST__
#define __inline__
#define __asm__ 
#define __volatile__

#define __builtin_va_list
#define __builtin_va_start
#define __builtin_va_end
#define __DOXYGEN__
#define __attribute__(x)
#define NOINLINE __attribute__((noinline))
#define prog_void
#define PGM_VOID_P int
            
typedef unsigned char byte;
extern "C" void __cxa_pure_virtual() {;}

//
//
void serialCheck();
void serialCheck();
void setMonitorFromMemory();
void setMonitorBacklight(uint32_t  c1, uint8_t n1, uint32_t c2, uint8_t n2, uint32_t c3, uint8_t n3, uint32_t c4, uint8_t n4);
void colorWipe(uint32_t c, uint8_t wait);
void rainbow(uint8_t wait);
void rainbowCycle(uint8_t wait);
uint32_t Wheel(byte WheelPos);
void march(uint32_t c1, uint32_t c2, uint32_t c3, uint8_t wait, uint16_t durations);
void alternate(uint32_t c1, uint32_t c2, uint8_t wait, uint16_t durations);

#include "D:\Program Files (x86)\Arduino\hardware\arduino\cores\arduino\arduino.h"
#include "D:\Program Files (x86)\Arduino\hardware\arduino\variants\micro\pins_arduino.h" 
#include "D:\Bryan Stadick\SkyDrive\Documents\Visual Studio 2012\Projects\NeopixelLEDControl\ChassisLEDs\ChassisLEDs.ino"
#include "D:\Bryan Stadick\SkyDrive\Documents\Visual Studio 2012\Projects\NeopixelLEDControl\ChassisLEDs\Predefined.ino"
#endif
