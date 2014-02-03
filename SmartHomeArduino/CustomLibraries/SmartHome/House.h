#ifndef House_h
#define House_h

#include "Arduino.h"

class PinState
{
public: 
	PinState();
	String PrintPinState();
	void SetPinState(boolean state);
	void SaveChanges();
public:
	byte _pin;
	boolean	_state;
	byte	_mode;
};


class Sensor
{
public:
	Sensor();				
	Sensor(int sensorId,int sensorPin,boolean alarmCheck,float minTempAlert,float maxTempAlert);
	float GetCurrentTemp(); 
	String GetStateInJson(bool allFields);
	void  PrintSensors();
	boolean Check();
	void Alert();
	void TurnOffAlarm();
	void Delete();
	void SaveChanges();
public:
	int _sensorId;
	boolean _changeStateAlarm; 
	float _minTempAlert;
	float _maxTempAlert;
	boolean _alarmCheck;
	int _sensorPin;		
	OneWire _oneWire;	
};
class Termostat
{
public:
	Termostat(int thermostatId,Sensor &sensor,boolean  termostat,boolean  behavior,float targetTemp,int termostatDevicePinfloat);
	Termostat();	
	
	void PrintTermostat();
	void Check();		
	void TurnOff();		
	float GetTargetTemp();
	float GetCurrentTemp();
	void Delete();
	void SaveChanges();

public:
	int _thermostatId;
	Sensor *_sensor;
	float _targetTemp;
	boolean  _behavior;
	boolean  _termostat;
	byte _termostatDevicePin;

};

#endif



