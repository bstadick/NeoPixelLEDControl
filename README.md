This software is licensed under the GNU GPL. A copy of which can be found in the base directory.


This software aims to create a high level interface between the user and the NeoPixel LED strips. It allows for control of two independent strips up to 60 LEDs per strip. It uses an Arduino based microcontroller to provide the driving signals for the LEDs. Power can be derived from a high current source such as a PC PSU or wall wart power supply. DO NOT power the LEDs through the Arduino or USB based supply. For more information about proper use of the LED strips along with where to purchase, see [the Adafruits tutorial](http://learn.adafruit.com/adafruit-neopixel-uberguide).


The intention is to be used with a computer case and monitor backlighting. 1 m of LED strip will cover the back of two 21" monitor or about half the perimeter of a mid-tower PC case. With the 30 LEDs per meter strip, this gives 15 LEDs per monitor back and 30 LEDs for the case. Modification of the firmware can allow for more LEDs and more dense LED numbers. Power for the LEDs is derived from the 5V molex pins of the PC PSU. The Arduino is plugged into an available USB port on the computer for its power and data connection. Data is transferred over the USB serial port. Each LED strip requires one pin on the Arduino for control. This is pin 6 (case) and 7 (monitors) initially, but can be changed by the user in the firmware. LED strips can be chained together to make longer strips.


Current functionality in the firmware allows for changing the solid color for each monitor and for the case using the desktop client. The user can also toggle between running a series of patterns or a simple solid color for the case. Patterns can be changed by the user in the firmware. The user can also choose how many LEDs there are for each monitor and case in the desktop client or in the firmware.


We're interested in seeing what cools designs and patterns you come up with. To contribute a pattern to our library, make a pull request or submit it to epoch365@gmail.com. Design ideas or other cool features/add-ons can be submitted to the repository wiki or emailed to epoch365@gmail.com.


To program the Arduino with the provided firmware you must install the Adafruit_NeoPixel Arduino library into your Arduino library folder. This library is available at [https://github.com/adafruit/Adafruit_NeoPixel](https://github.com/adafruit/Adafruit_NeoPixel). This library is licensed by Adafruit under the GNU LGPL license.


This software is developed by a third party not affiliated with Adafruit, NeoPixel or Arduino. For questions regarding the use or to report bugs, please visit the [software repository](https://bitbucket.org/bstadick/neopixelledcontrol) and post in the issues or see the wiki. You can also contact the directory directly at epoch365@gmail.com.


This software is distributed as is and without a warranty. Use and redistribution is free as long as the original source of the code is credited.


Adafruit, NeoPixel and Arduino are trademarks of their respective holders.