const int SliderCount = 4;
const int analogueInputs[SliderCount] = { A0, A1, A2, A3 };
const int pin_Button = 2;
const int pin_LEDState = 3;
const int rounding = 10;
const int StabilityValue = 5;
int currentValues[SliderCount] = {0,0,0,0};
int changedValCount[SliderCount] = {100,100,100,100};
bool buttonPressed = false;
bool buttonSent = false;
bool active = false;


void setup() {
   for (int i = 0; i < SliderCount; i++) {
    pinMode(analogueInputs[i], INPUT);
  }

  pinMode(pin_Button,INPUT);
  pinMode(pin_LEDState,OUTPUT);
  active = false;
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

    if (i < SliderCount - 1) {
      retVal += String("|");
    }
  }
  GatedSendString(retVal);
}

void SerialCommandHandler()
{
  String myString = "";
  if (Serial.available() > 0)
  {
    myString = Serial.readStringUntil('\n');
    if (myString == "Hello")
    {
      active = true;
      GatedSendString("Hi");
    }
    else if (myString == "Bye")
    {
      active = false;
    }
    else if (myString == "Get")
    {
      SendCommand();
    }
    else if (myString == "LED0")
    {
      digitalWrite(pin_LEDState,LOW);
    } else if (myString == "LED1")
    {
      digitalWrite(pin_LEDState, HIGH);
    }
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
}


