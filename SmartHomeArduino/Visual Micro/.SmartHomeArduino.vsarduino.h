#ifndef _VSARDUINO_H_
#define _VSARDUINO_H_
//Board = Arduino Mega 2560 or Mega ADK
#define __AVR_ATmega2560__
#define ARDUINO 101
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
void CheckClient();
String ParseReadString(String message);
void SendDateToServer(String dataSent, String api);
void SendTempsToServer();
String CreateStringWithSensors(int sensorId, bool allFields);
void SetDeviceFromJson(aJsonObject* json);
void CheckTermostats();
void CheckSensorAlarm();
void AlertSensor(int sensorId);
void InitPinsState();
void LoadConfigurationFromEEPROM();
void LoadStateFromEEPROM();
void ClearStateEEPROM();
void PrintTermostats();
void PrintSensors();
void PrintPinsState();
void PrintConfiguration();
uint16_t freeMem(uint16_t *biggest);
void freeMem(char* message);

#include "C:\Program Files (x86)\Arduino\hardware\arduino\cores\arduino\arduino.h"
#include "C:\Program Files (x86)\Arduino\hardware\arduino\variants\mega\pins_arduino.h" 
#include "C:\Users\Kalin-PC\Documents\Arduino\SmartHomeArduino\SmartHomeArduino.ino"
#endif
