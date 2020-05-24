// DogsHeaven Observatory Focuser Control
// Version 1.0
// Public Domain
//
// Paulo Cacella


#include <CmdMessenger.h>

#include <AccelStepper.h>

#define HALFSTEP 8
#include <EEPROM.h>
#define POSITIONMICROMETER 0

// Motor pin definitions
#define ins  11   
#define outs  12     
#define mspeed 8       

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  // Commands
  kMove ,
  kHalt ,
  kAcknowledge,
  kError
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);  
  cmdMessenger.attach(kMove,OnArduinoMove);
  cmdMessenger.attach(kHalt,OnArduinoHalt);  
  cmdMessenger.attach(kAcknowledge,OnArduinoReady); 
}


// ------------------  C A L L B A C K S -----------------------

void OnArduinoMove()
{
  // Retrieve first parameter as float
  long time = cmdMessenger.readInt32Arg();  // 1000 = 1 second
  if (time<0)
  {
      time=-time;
      digitalWrite(11, HIGH);
      digitalWrite(12, LOW);
      delay(time);   
      digitalWrite(11, HIGH);
      digitalWrite(12, HIGH);  
      time=-time;   
  }
  
  if (time==0) 
  {
      digitalWrite(11, HIGH);
      digitalWrite(12, HIGH);
  }
  
  if (time>0)
  {
      digitalWrite(11, LOW);
      digitalWrite(12, HIGH);
      delay(time);  
      digitalWrite(12, HIGH); 
      digitalWrite(11, HIGH);     
  }
  
}

void OnArduinoHalt()
{
      digitalWrite(11, HIGH);
      digitalWrite(12, HIGH);
}

void OnArduinoReady()
{
  cmdMessenger.sendCmd(kAcknowledge,"Ok");
}

void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"CC");
}


void setup() {
  int speed = 128;
  pinMode(11, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(7, OUTPUT);  
  analogWrite(7, speed);
  Serial.begin(115200);  
  cmdMessenger.printLfCr();   
  attachCommandCallbacks();
}

void loop() {
    cmdMessenger.feedinSerialData();
}
