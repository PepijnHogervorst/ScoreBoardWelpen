/************************************************************************/
/*                               INCLUDES                               */
/************************************************************************/
#include <Adafruit_NeoPixel.h>

/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                             DEFINITIONS                              */
/************************************************************************/
#define LED_GROUP_1             5
#define LED_GROUP_2             5
#define LED_GROUP_3             5
#define LED_GROUP_4             5
#define LED_GROUP_5             5
#define LED_GROUP_6             5

#define NUM_OF_PIXELS           100
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                            GLOBAL VARIABLES                          */
/************************************************************************/
Adafruit_NeoPixel pixels_group1 = Adafruit_NeoPixel(NUM_OF_PIXELS, LED_GROUP_1, NEO_RGB  + NEO_KHZ800);
/************************************************************************/
/*                                                                      */
/************************************************************************/
void setup() {
  // put your setup code here, to run once:

}

void loop() {
  // put your main code here, to run repeatedly:

}