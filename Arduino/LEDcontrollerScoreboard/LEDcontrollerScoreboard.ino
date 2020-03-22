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
// If DEBUG is defined (uncommented)the arduino will do a RGB loop through every LED_GROUP
#define DEBUG
// Uncomment this to test voltage drop on led strip, only works if debug is defined
//#define DEBUG_VOLTAGE_DROP

// PIN definitions of led strip groups
#define LED_GROUP_1             2
#define LED_GROUP_2             3
#define LED_GROUP_3             4
#define LED_GROUP_4             5
#define LED_GROUP_5             6
#define LED_GROUP_6             7
// The total number of led strip groups:
#define NUM_OF_GROUPS           6
// Number of leds per group (all groups have the same amount of LEDS)
#define NUM_OF_PIXELS           98

// Max brightness of the leds 
#define BRIGHTNESS              200

#define LED_OFFSET_1            0
#define LED_OFFSET_2            (NUM_OF_PIXELS * 1)/ NUM_OF_GROUPS 
#define LED_OFFSET_3            (NUM_OF_PIXELS * 2)/ NUM_OF_GROUPS 
#define LED_OFFSET_4            (NUM_OF_PIXELS * 3)/ NUM_OF_GROUPS 
#define LED_OFFSET_5            (NUM_OF_PIXELS * 4)/ NUM_OF_GROUPS 
#define LED_OFFSET_6            (NUM_OF_PIXELS * 5)/ NUM_OF_GROUPS 
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                            GLOBAL VARIABLES                          */
/************************************************************************/
// One neopixel object is used, and the pin is switched when a different strip is needed to put on.
Adafruit_NeoPixel led_strip = Adafruit_NeoPixel(NUM_OF_PIXELS, LED_GROUP_1, NEO_RGB  + NEO_KHZ800);

String inputString = "";
bool messageReceived = false;

// Lookup table 
byte neopix_gamma[] = {
    0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
    0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  1,  1,  1,
    1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,
    2,  3,  3,  3,  3,  3,  3,  3,  4,  4,  4,  4,  4,  5,  5,  5,
    5,  6,  6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  9,  9,  9, 10,
   10, 10, 11, 11, 11, 12, 12, 13, 13, 13, 14, 14, 15, 15, 16, 16,
   17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 24, 24, 25,
   25, 26, 27, 27, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 35, 36,
   37, 38, 39, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 50,
   51, 52, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 67, 68,
   69, 70, 72, 73, 74, 75, 77, 78, 79, 81, 82, 83, 85, 86, 87, 89,
   90, 92, 93, 95, 96, 98, 99,101,102,104,105,107,109,110,112,114,
  115,117,119,120,122,124,126,127,129,131,133,135,137,138,140,142,
  144,146,148,150,152,154,156,158,160,162,164,167,169,171,173,175,
  177,180,182,184,186,189,191,193,196,198,200,203,205,208,210,213,
  215,218,220,223,225,228,231,233,236,239,241,244,247,249,252,255 };
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                             PROTOTYPES                               */
/************************************************************************/
void WriteLedStrip();
int groupNrToPin(int nr);
uint32_t Wheel(byte WheelPos);
void DebugLoop(void);
void PartyLoop(void);
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                                 SETUP                                */
/************************************************************************/
void setup() {
  // Init serial communication:
  Serial.begin(115200);
  Serial1.begin(9600);

  // Allocate memory for string
  inputString.reserve(200);
  
  // Init the led strip
  led_strip.setBrightness(BRIGHTNESS);
  led_strip.begin();
  led_strip.clear();

  // Clear all ledstrip groups
  int pin = 0;
  for (int i = 1; i <= NUM_OF_GROUPS; i++)
  {
    pin = groupNrToPin(i);
    led_strip.setPin(pin);
    led_strip.clear();
  }
  

  // Extra randomness for start color 
  randomSeed(analogRead(0));

  Serial.println("Initialization done!");
  #ifdef DEBUG 
  #ifdef DEBUG_VOLTAGE_DROP
  Serial.println("Debug VOLTAGE DROP mode active");
  #else
  Serial.println("Debug mode active");
  #endif
  #endif
}
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                                MAIN LOOP                             */
/************************************************************************/
void loop() {
  #ifndef DEBUG
  // Check if serial message is received
  if(messageReceived)
  {
    // Handle event
    Serial.println("Message received: " + inputString);
    WriteLedStrip();
    
    // Clear event
    messageReceived = false;
    inputString = "";
  }`
  #else
  // Debug mode! 
  PartyLoop();
  // DebugLoop();
  #endif
}
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                              EVENTS                                  */
/************************************************************************/
void serialEvent1() {
  while (Serial1.available()) 
  {
    // get the new byte:
    char inChar = (char)Serial1.read();
    // add it to the inputString:
    inputString += inChar;
    
    //if inputstring is length 6, the full message is received so signal 
    if (inputString.length() >= 6) 
    {
      messageReceived = true;
    }
  }
}
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                              FUNCTIONS                               */
/************************************************************************/
// Function where most of the magic happens, 
// decifers serial input and sets leds using a color wheel
void WriteLedStrip()
{
  int groupNr = 0; int16_t points = 0;
  int ledPin = 0; 
  char grpChar = inputString.charAt(1);
  String pointsString = inputString.substring(3);

  // Get the group nr and pin nr
  groupNr = grpChar - '0';
  ledPin = groupNrToPin(groupNr);
  // Check if pin is valid otherwise return
  if(ledPin == 0)
    return;
    
  for(int i = 0; i < pointsString.length(); i++)
  {
    if(!isDigit(pointsString.charAt(i)))
    {
      // points string not digit, return
      Serial.println("POINTS NOT ALL DIGITS, NOT SETTING LEDS");
      return;
    }
  }

  // Set points
  points = pointsString.toInt();
  if (points > led_strip.numPixels())
  {
    points = led_strip.numPixels();
  }
  else if (points < 0)
  {
    points = 0;
  }

  Serial.print("Group: ");
  Serial.print(groupNr);
  Serial.print("   Points: ");
  Serial.println(points);
  
  // Set ledstrip pin to group ledstrip pin
  led_strip.clear();
  led_strip.setPin(ledPin);

  // Get random offset 
  long randomNr = random(256);
  uint16_t loopCount = 1;
  uint16_t delayTime = (5000 / points) - 1;
  
  do
  {
    for(int16_t i = 0; i < loopCount; i++)
    {
      led_strip.setPixelColor(i, Wheel(((i * 256 / led_strip.numPixels()) + randomNr) & 255));
    }
    led_strip.show();
    delay(delayTime);
    loopCount++;
  }
  while(loopCount < points);

  Serial.println("LEDS set! Waiting for new serial command");
}

int groupNrToPin(int nr)
{
  int pin = 0;
  switch(nr)
  {
    case 1: pin = LED_GROUP_1; break;
    case 2: pin = LED_GROUP_2; break;
    case 3: pin = LED_GROUP_3; break;
    case 4: pin = LED_GROUP_4; break;
    case 5: pin = LED_GROUP_5; break;
    case 6: pin = LED_GROUP_6; break;
  }

  return pin;
}

// Input a value 0 to 255 to get a color value.
// The colours are a transition r - g - b - back to r.
uint32_t Wheel(byte WheelPos) 
{
  WheelPos = 255 - WheelPos;
  if(WheelPos < 85) {
    return led_strip.Color(255 - WheelPos * 3, 0, WheelPos * 3,0);
  }
  if(WheelPos < 170) {
    WheelPos -= 85;
    return led_strip.Color(0, WheelPos * 3, 255 - WheelPos * 3,0);
  }
  WheelPos -= 170;
  return led_strip.Color(WheelPos * 3, 255 - WheelPos * 3, 0,0);
}

void DebugLoop(void)
{
  static uint8_t groupCounter = 1;
  int ledPin = 0;
  if (groupCounter > NUM_OF_GROUPS)
  {
    groupCounter = 1;
  }
  // Set pin to right group:
  ledPin = groupNrToPin(groupCounter);
  led_strip.clear();
  led_strip.setPin(ledPin);

  // Randomness
  long randomNr = random(256);

  // Show LEDS 
  for(int16_t i = 0; i < led_strip.numPixels(); i++)
  {
    #ifdef DEBUG_VOLTAGE_DROP
    led_strip.setPixelColor(i, led_strip.Color(255,255,255));
    #else
    led_strip.setPixelColor(i, Wheel(((i * 256 / led_strip.numPixels()) + randomNr) & 255));
    #endif
  }
  led_strip.show();
  Serial.print("LEDs group ");
  Serial.print(groupCounter);
  Serial.println(" set!");
  delay(250);

  groupCounter++;
}

void PartyLoop(void)
{
  static uint16_t colorOffset = 0; 
  int ledPin = 0;
  uint16_t ledStart = 0;
  // Check if offset is at end of led total count
  if (colorOffset >= (NUM_OF_GROUPS * NUM_OF_PIXELS))
  {
    colorOffset = 0;
  }
  // Loop through all led strips
  for (uint8_t groupCount = 1; groupCount <= NUM_OF_GROUPS; groupCount++)
  {
    ledPin = groupNrToPin(groupCount);
    led_strip.setPin(ledPin);
    // Each led strip has his own start point 
    ledStart = (groupCount * led_strip.numPixels()) + colorOffset;

    for (uint16_t i = 0; i < led_strip.numPixels(); i++)
    {
      led_strip.setPixelColor(i, Wheel(((i * 256 / led_strip.numPixels()) + ledStart) & 255));
    }
    led_strip.show();
  }
  delay(5);
  
  colorOffset++;
}
/************************************************************************/
/*                                                                      */
/************************************************************************/
