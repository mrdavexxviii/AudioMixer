#include <LiquidCrystal_I2C.h>

const int SliderCount = 4;
const int analogueInputs[SliderCount] = { A0, A1, A2, A3 };
const int pin_Button = 2;
const int pin_LEDState = 3;
const int pin_LEDState_Sl0 = 4;
const int pin_LEDState_Sl1 = 5;
const int rounding = 10;
const int StabilityValue = 20;
const int HeartbeatPulse = 1000 * 60;
const int HeartbeatPingAllowance = 5000;
const long maxLong = 2147483647L;
int currentValues[SliderCount] = {0,0,0,0};
int changedValCount[SliderCount] = {100,100,100,100};
long MustHaveHeartbeatBy;
long NextHeartbeat = 0;
bool buttonPressed = false;
bool buttonSent = false;
bool active = false;

LiquidCrystal_I2C lcd(0x27,20,4);

void setup() {
   for (int i = 0; i < SliderCount; i++) {
    pinMode(analogueInputs[i], INPUT);
  }
  MustHaveHeartbeatBy = maxLong;
  pinMode(pin_Button,INPUT);
  pinMode(pin_LEDState,OUTPUT);
  pinMode(pin_LEDState_Sl0,OUTPUT);
  pinMode(pin_LEDState_Sl1,OUTPUT);
  digitalWrite(pin_LEDState, LOW);
  digitalWrite(pin_LEDState_Sl0, LOW);
  digitalWrite(pin_LEDState_Sl1, LOW);
  active = false;
  lcd.init();
  lcd.noBacklight();
  lcd.clear();

  Serial.begin(9600);
  
}

void GatedSendString (String s)
{
  if (active)
  {
    Serial.println(s);
  }
}

bool updateButton()
{
  if (!active) return;
  buttonPressed = digitalRead(pin_Button);
  if (buttonPressed)
  {
    if (!buttonSent)
    {
      GatedSendString("Toggle");
      buttonSent = true;
    }
  } else {
    buttonSent = false;
  }
}

bool UpdateSliderValue (int index)
{
  int raw = (int)analogRead(analogueInputs [index]);
  int remainder = raw % rounding;
  raw = raw - remainder;
  if (remainder > 2 ) raw += rounding;
  if (raw > 1000) raw = 1000;
  raw = raw / 10;
  raw = 100 - raw;
  if (currentValues[index] != raw)
  {
    changedValCount[index] += 1;
    if (changedValCount[index] > StabilityValue)
    {
      changedValCount[index] = 0;
      currentValues[index] = raw;
      return true;
    }
  } else changedValCount[index] = 0;
  return false;
}

bool UpdateSliderValues()
{
   bool retVal = false;
   for (int i = 0; i < SliderCount; i++) {
    retVal |= UpdateSliderValue(i);
   }
   return retVal;
}

void SendCommand()
{
  if (!active) return;
  String retVal = "";
   for (int i = 0; i < SliderCount; i++) {
    retVal += String(currentValues[i]);
    lcd.setCursor(i * 4,0);
    lcd.print(String(currentValues[i]));
    lcd.print(" ");
    if (i < SliderCount - 1) {
      retVal += String("|");
    }
  }
  GatedSendString(retVal);
}

void SerialCommandHandler()
{
  String myString = "";
  while (Serial.available() > 0)
  {
    myString = Serial.readStringUntil('\n');
    if (myString == "Hello")
    {
      wakeUp();
    }
    else if (myString == "Bye")
    {
      Shutdown();
    } else if (myString == "Beat")
    {
      MustHaveHeartbeatBy = maxLong;
    }
    else if (myString == "LED0")
    {
      digitalWrite(pin_LEDState,LOW);
    } else if (myString == "LED1")
    {
      digitalWrite(pin_LEDState, HIGH);
    } else if (myString == "Select_SL0")
    {
      digitalWrite(pin_LEDState_Sl0, HIGH);
      digitalWrite(pin_LEDState_Sl1, LOW);
      lcd.setCursor(0,1);
      lcd.print("Headset ");
    }else if (myString == "Select_SL1")
    {
      digitalWrite(pin_LEDState_Sl1, HIGH);
      digitalWrite(pin_LEDState_Sl0,LOW);
      lcd.setCursor(0,1);
      lcd.print("Speakers");
    }
  }
}

void wakeUp()
{
      active = true;
      GatedSendString("Hi");
      SendCommand();
      lcd.backlight();
}

void Shutdown()
{
      active = false;
      digitalWrite(pin_LEDState,LOW);
      digitalWrite(pin_LEDState_Sl0, LOW);
      digitalWrite(pin_LEDState_Sl1, LOW);
      NextHeartbeat = HeartbeatPulse;
      MustHaveHeartbeatBy = maxLong;
      lcd.noBacklight();
      lcd.clear();
}

void HandleHeartbeat()
{
  if (active)
  {
    if (millis() > NextHeartbeat)
    {
        GatedSendString("Beat");
        MustHaveHeartbeatBy = millis() + HeartbeatPingAllowance;
        NextHeartbeat = millis() + HeartbeatPulse;
    }
    if (millis() > MustHaveHeartbeatBy)
    {
        NextHeartbeat = millis() + HeartbeatPulse;
        MustHaveHeartbeatBy = maxLong;
        Shutdown();
    }
  } else {
        Serial.println("Waiting");
        delay(5000);
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  if (UpdateSliderValues())
  {
     SendCommand();
  }
  updateButton();
  SerialCommandHandler();
 // HandleHeartbeat();
}



