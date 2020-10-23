/************************************************************************/
/*                               INCLUDES                               */
/************************************************************************/
#include <Adafruit_NeoPixel.h>
#include <EEPROM.h>
/************************************************************************/
/*                                                                      */
/************************************************************************/
// Protocol works as following:
// Arduino receives commands from rpi of size: 6 bytes. In following structure:
// gXpXXX -> the first X is for the group 
// groupNr points
// Brightness is also adjustable using:
// bbbXXX => XXX is brightness from 0 to 255
// Party mode is actived using:
// pppXXX => XXX is party mode program selection
// Stop party mode:
// sXXXXX => dont care as long if msg is 6 bytes
/************************************************************************/
/*                             DEFINITIONS                              */
/************************************************************************/
// If DEBUG is defined (uncommented)the arduino will do a RGB loop through every LED_GROUP
//#define DEBUG
// Uncomment this to test voltage drop on led strip, only works if debug is defined
//#define DEBUG_VOLTAGE_DROP

// PIN definitions of led strip groups
#define LED_GROUP_1             2
#define LED_GROUP_2             3
#define LED_GROUP_3             4
#define LED_GROUP_4             5
#define LED_GROUP_5             6
#define LED_GROUP_6             7

// INPUTS
#define BTN_ARCADE              40
// OUTPUTS
#define BTN_INDICATOR           41

// The total number of led strip groups:
#define NUM_OF_GROUPS           6
// Number of leds per group (all groups have the same amount of LEDS)
#define NUM_OF_PIXELS           98

// Max brightness of the leds 
#define BRIGHTNESS              200
#define BRIGHTNESS_ADDRESS      0

#define LED_OFFSET_1            0
#define LED_OFFSET_2            (255 * 1)/ NUM_OF_GROUPS 
#define LED_OFFSET_3            (255 * 2)/ NUM_OF_GROUPS 
#define LED_OFFSET_4            (255 * 3)/ NUM_OF_GROUPS 
#define LED_OFFSET_5            (255 * 4)/ NUM_OF_GROUPS 
#define LED_OFFSET_6            (255 * 5)/ NUM_OF_GROUPS 


/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                            GLOBAL VARIABLES                          */
/************************************************************************/
// One neopixel object is used, and the pin is switched when a different strip is needed to put on.
Adafruit_NeoPixel led_strip;

String inputString = "";
String ledMsg = "";
bool messageReceived = false;
bool IsPartyMode = false;
bool IsArcadeBtnEnabled = false;
bool PartyProgramChanged = true;
uint8_t PartyProgram = 0;
uint16_t PartyDelayTime = 5;

int GroupNr = 0;
int Points = 0;

const char SoftwareVersion[] = "V0.1";
const char compile_date[] = __DATE__ " " __TIME__;
const uint32_t MY_COLORS[] = {
  led_strip.Color(0, 255, 0),
  led_strip.Color(255, 0, 0),
  led_strip.Color(0, 0, 255),
  led_strip.Color(255, 255, 0),
  led_strip.Color(0, 255, 255),
  led_strip.Color(255, 0, 255),
  led_strip.Color(255,255,255)
};

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
/*                               STRUCTS                                */
/************************************************************************/
struct RainDrop_t
{
  int8_t State;
  uint8_t LedPin;
  uint8_t Location;
};
/************************************************************************/
/*                                                                      */
/************************************************************************/


/************************************************************************/
/*                             PROTOTYPES                               */
/************************************************************************/
void DeciferMessage();
void WriteLedStrip();
int groupNrToPin(int nr);
int groupNrToOffset(int nr);
uint32_t Wheel(byte WheelPos);
void DebugLoop(void);
void ClearLEDstrips(void);
// Party prototypes
void PartyLoop(void);
void PP_Full_rainbow(void);
void PP_Random_Rain(void);
void PP_ColorWipes(void);
// Button control prototypes
void CheckButtonControl(void);

void ColorWipe(uint32_t color, int delayTime);
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

  Serial.print("LEDcontrollerScoreboard ");
  Serial.println(SoftwareVersion);
  Serial.print("Compile date: ");
  Serial.println(compile_date);
  // Allocate memory for string
  inputString.reserve(200);
  
  // Startup delay to let power supply stabilize
  delay(3000);
  led_strip = Adafruit_NeoPixel(NUM_OF_PIXELS, LED_GROUP_1, NEO_RGB  + NEO_KHZ800);

  // Retrieve brightness from eeprom
  int brightness = EEPROM.read(BRIGHTNESS_ADDRESS);
  Serial.print("Brightness set to: ");
  Serial.println(brightness);
  if (brightness == 0)
  {
    brightness = BRIGHTNESS;
  }

  // Init the led strip
  led_strip.setBrightness(brightness);
  led_strip.begin();
  led_strip.clear();

  ClearLEDstrips();

  // Init IO
  pinMode(BTN_ARCADE, INPUT);
  pinMode(BTN_INDICATOR, OUTPUT);
  digitalWrite(BTN_INDICATOR, LOW);

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
    DeciferMessage();
    
    // Clear event
    messageReceived = false;
    inputString = "";
  }
  if (IsPartyMode)
  {
    PartyLoop();
  }
  if (IsArcadeBtnEnabled)
  {
    if (digitalRead(BTN_ARCADE) == HIGH)
    {
      delay(75);
      if (digitalRead(BTN_ARCADE) == HIGH)
      {
        WriteLedStrip();
      }
    }
  }
  else
  {
    CheckButtonControl();
  }
  
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
void serialEvent1() 
{
  static unsigned long prevEventTime = 0;
  unsigned long currentTime = millis();
  
  
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

    if ((currentTime - prevEventTime) > 750 ||
       (((prevEventTime + 750) > currentTime) && prevEventTime > currentTime))
    {
      inputString = "";
      Serial.println("Purged serial input buffer");
    }
  }
  prevEventTime = millis();
}
/************************************************************************/
/*                                                                      */
/************************************************************************/

/************************************************************************/
/*                              FUNCTIONS                               */
/************************************************************************/
// Function where most of the magic happens, 
// decifers serial input and sets leds using a color wheel
void DeciferMessage()
{
  uint8_t program;
  String BufPartyTime;
  char functionChar = inputString.charAt(0);
  
  // Check first char if msg is led command or brightness or else
  switch (functionChar)
  {
    case 'g':
      // Reset party mode on serial receiving
      if (IsPartyMode)
      {
        Serial.println("Party canceled..");
        ClearLEDstrips();
      }
      IsPartyMode = false;
      Serial.println("Enabling button!");
      // Enable the button, when pressed write ledstrip
      EnableArcadeButton();
      break;

    case 'b':
      // Led program
      SetLEDBrightness();
      break;

    case 'p':
      // Pary mode!
      IsPartyMode = true;
      // Get program number 
      program = inputString.charAt(1) - '0';
      // Get party speed (delay time in ms)
      BufPartyTime = inputString.substring(2, 4); 
      PartyDelayTime = BufPartyTime.toInt();
      if (PartyDelayTime > 99 || PartyDelayTime == 0)
      {
        PartyDelayTime = 5;
      }
      Serial.print("Party Delay Time = ");
      Serial.println(PartyDelayTime);

      if (program >= 0 && program < 9)
      {
        if (program != PartyProgram)
        {
          ClearLEDstrips();
          PartyProgram = program;
          PartyProgramChanged = true;
        }
      }
      break;

    case 's':
      IsPartyMode = false;
      Serial.println("Party canceled..");
      ClearLEDstrips();
      break;
    
    case 'S': 
      // Strobe value
      break;

    case 'c':
      Serial.println("Clearing ledstrips");
      if (IsPartyMode)
      {
        
        ClearLEDstrips();
      }
      IsPartyMode = false;
      ClearLEDstrips();
      break;
  
    default:
      break;
  }
}

void EnableArcadeButton()
{
  IsArcadeBtnEnabled = true;
  // Copy message to ledMsg
  ledMsg = inputString;
  // Set LED in arcade button to signal it is active
  digitalWrite(BTN_INDICATOR, HIGH);

  // Calculate group and points
  char grpChar = ledMsg.charAt(1);
  String pointsString = inputString.substring(3);

   // Get the group nr and pin nr
  GroupNr = grpChar - '0';

  for(int i = 0; i < pointsString.length(); i++)
  {
    if(!isDigit(pointsString.charAt(i)))
    {
      // points string not digit, return
      Serial.println("POINTS NOT ALL DIGITS, NOT SETTING LEDS");
      Serial.print("String = ");
      Serial.println(pointsString);
      return;
    }
  }

  // Set points
  Points = pointsString.toInt();
  if (Points > led_strip.numPixels())
  {
    Points = led_strip.numPixels();
  }
  else if (Points < 0)
  {
    Points = 0;
  }
}

void WriteLedStrip()
{
  int ledPin = 0; 

  delay(100);
  IsArcadeBtnEnabled = false;
  digitalWrite(BTN_INDICATOR, LOW);

  ledPin = groupNrToPin(GroupNr);
  // Check if pin is valid otherwise return
  if(ledPin == 0)
    return;
  
  Serial.print("Group: ");
  Serial.print(GroupNr);
  Serial.print("   Points: ");
  Serial.println(Points);
  
  // Set ledstrip pin to group ledstrip pin
  led_strip.setPin(ledPin);

  // Get random offset 
  long randomNr = random(256);
  uint16_t loopCount = 1;
  uint16_t delayTime = (5000 / Points) - 1;
  
  // Clear previously show points on strip
  led_strip.clear();
  led_strip.show();
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
  while(loopCount <= Points);

  Serial.println("LEDS set! Waiting for new serial command..");

  Serial1.print("DONE");
}

void SetLEDBrightness()
{
  String buf = inputString.substring(3);
  int bright = buf.toInt();
  led_strip.setBrightness(bright);
  EEPROM.write(BRIGHTNESS_ADDRESS, bright);

  Serial.println();
  Serial.print("Brightness set to: ");
  Serial.println(bright);
  Serial.println("Waiting for new serial command..");
  Serial.println();
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

int groupNrToOffset(int nr)
{
  int pin = 0;
  switch(nr)
  {
    case 1: pin = LED_OFFSET_1; break;
    case 2: pin = LED_OFFSET_2; break;
    case 3: pin = LED_OFFSET_3; break;
    case 4: pin = LED_OFFSET_4; break;
    case 5: pin = LED_OFFSET_5; break;
    case 6: pin = LED_OFFSET_6; break;
  }

  return pin;
}

void ClearLEDstrips()
{
  int pin = 0;
  for (int i = 1; i <= NUM_OF_GROUPS; i++)
  {
    pin = groupNrToPin(i);
    led_strip.setPin(pin);
    led_strip.clear();
    led_strip.show();
  }
}

#pragma region PARTY PROGRAM FUNCTIONS
void PartyLoop(void)
{
  switch (PartyProgram)
  {
  case 0:
    if (PartyProgramChanged)
    {
      PartyProgramChanged = false;
      Serial.println("Full Rainbow Party!!");
    }
    PP_Full_Rainbow();
    break;
  
  case 1:
    if (PartyProgramChanged)
    {
      PartyProgramChanged = false;
      Serial.println("Random Rain Party!!");
    }
    PP_Random_Rain();
    break;

  case 2:
    if (PartyProgramChanged)
    {
      PartyProgramChanged = false;
      Serial.println("Color Wipes Party!!");
    }
    PP_ColorWipes();
    break;

  default:
    break;
  }
  
}

void PP_Full_Rainbow(void)
{
  static uint16_t colorOffset = 0; 
  int ledPin = 0;
  int groupOffset = 0;
  uint16_t ledStart = 0;
  // Check if offset is at end of led total count
  if (colorOffset >= 255)
  {
    colorOffset = 0;
  }
  // Loop through all led strips
  for (uint8_t groupCount = 1; groupCount <= NUM_OF_GROUPS; groupCount++)
  {
    ledPin = groupNrToPin(groupCount);
    groupOffset = groupNrToOffset(groupCount);

    led_strip.setPin(ledPin);
    // Each led strip has his own start point 
    ledStart = groupOffset + colorOffset;

    for (uint16_t i = 0; i < led_strip.numPixels(); i++)
    {
      led_strip.setPixelColor(i, Wheel(((i * 256 / led_strip.numPixels()) + ledStart) & 255));
    }
    led_strip.show();
  }
  delay(PartyDelayTime);
  
  colorOffset++;
}

// Random rain drops in random color
void PP_Random_Rain(void)
{
  static RainDrop_t rainScene = { 0, 0, 0};

  int ledPin = 0;

  for (uint8_t i = 1; i <= NUM_OF_GROUPS; i++)
  {
    ledPin = groupNrToPin(i);
    led_strip.setPin(ledPin);

    // Always clear strip
    led_strip.clear();
    led_strip.show();

    led_strip.setPixelColor(
      random(4, led_strip.numPixels() - 3),
      MY_COLORS[random(sizeof(MY_COLORS) / sizeof(MY_COLORS[0]))]
    );
    led_strip.show();
  }
  
  delay(PartyDelayTime * 10);

  // Update state of 
  rainScene.State++;
}

void PP_ColorWipes(void)
{
  static uint8_t loopCount = 0;
  static uint8_t colorLoopCount = 0;
  static uint32_t activeColor = MY_COLORS[0];

  uint8_t ledPin = 0;
  uint32_t newColor;

  // Check if loop is at end, start over again with new color
  if (loopCount >= led_strip.numPixels())
  {
    loopCount = 0;
    // Select new color
    if (colorLoopCount >= (sizeof(MY_COLORS) / sizeof(MY_COLORS[0])) )
    {
      colorLoopCount = 0;
    }
    else
    {
      colorLoopCount++;
    }
    
    // Select new random color from array
    do
    {
      newColor = MY_COLORS[random(sizeof(MY_COLORS) / sizeof(MY_COLORS[0]))];
    } while (newColor == activeColor);
    
    activeColor = newColor;
  }

  // Idea is that all led bars do the same color
  for (uint8_t groupCount = 1; groupCount <= NUM_OF_GROUPS; groupCount++)
  {
    // Set the led functions to the right led bar pin
    ledPin = groupNrToPin(groupCount);
    led_strip.setPin(ledPin);

    // Set LEDS
    for (uint8_t i = 0; i < loopCount; i++)
    {
      led_strip.setPixelColor(i, activeColor);
    }
    
    // Show LEDs
    led_strip.show();
  }
  
  // Delay a small period of time
  delay(PartyDelayTime);

  // Increment loop count
  loopCount++;
}

void ColorWipe(uint32_t color, int delayTime)
{
  for(int i = 0; i < led_strip.numPixels(); i++)
  {
    led_strip.setPixelColor(i, color);
    led_strip.show();
    delay(delayTime);
  }
}
#pragma endregion

#pragma region Button control
void CheckButtonControl(void)
{
  static bool IsBtnHeldDown = false;
  static long timePressed = 0;

  // Check if button is pressed for certain time
    bool btnPressed;
    btnPressed = (digitalRead(BTN_ARCADE) == HIGH);
    if (btnPressed && !IsBtnHeldDown)
    {
      // Small debounce delay
      delay(25);
      // Get time to compare when button is released
      IsBtnHeldDown = true;
      timePressed = millis();
    }
    else if (!btnPressed && IsBtnHeldDown)
    {
      IsBtnHeldDown = false;
      unsigned long currentTime = millis();
      unsigned long timePassed = currentTime - timePressed;
      if (timePassed > 0 && timePassed < 1000)
      {
        if (IsPartyMode)
        {
          // Change party mode
          ClearLEDstrips();
          if (PartyProgram > 3)
          {
            PartyProgram = 0;
          }
          else
          {
            PartyProgram++;
          }
          PartyProgramChanged = true;
        }
      }
      else if (timePassed >= 2000 && timePassed < 5000)
      {
        IsPartyMode = !IsPartyMode;
        if (IsPartyMode)
        {
          Serial.println("Party!");
          digitalWrite(BTN_INDICATOR, HIGH);
        }
        else
        {
          Serial.println("Party stopped..");
          digitalWrite(BTN_INDICATOR, LOW);
          ClearLEDstrips();
        }
      }
    }
}
#pragma endregion
/************************************************************************/
/*                                                                      */
/************************************************************************/
