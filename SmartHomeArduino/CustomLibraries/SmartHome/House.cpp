#include <OneWire.h>
#include <DallasTemperature.h>
#include "Arduino.h"
#include "House.h"
#include <string.h>
#include <stdlib.h>
#include <WString.h>
#include <avr/eeprom.h>
#include <aJSON.h>

#define StartInedexTermostat 44		
#define StartInedexSensor 1300		
#define StartInedexPinState 1000	

PinState::PinState()
{
	_pin=0;
	_state=LOW;
	_mode=3;
}

String PinState::PrintPinState()
{
	String pinState="State: ";
	pinState.concat(_state);
	pinState.concat("\n");
	pinState.concat("Mode: ");
	pinState.concat(_mode);
	pinState.concat("\n");
	return pinState;
}

void PinState::SetPinState(boolean state)
{
	_state=state;
	_mode=1;
	pinMode(_pin,1);
	digitalWrite(_pin,state);

}

void PinState::SaveChanges()
{

	int index=StartInedexPinState;
	index=index+_pin*sizeof(PinState);
	eeprom_write_block((const void*)this, (void*)index, sizeof(PinState));
}


Sensor::Sensor()
{
	_changeStateAlarm=false;
	_sensorPin=100;
	float _minTempAlert=-100.0;
	float _maxTempAlert=100.0;
	_alarmCheck=false;
}

Sensor::Sensor(int sensorId,int sensorPin,boolean alarmCheck,float minTempAlert=0.0,float maxTempAlert=0.0)
{
	_sensorId=sensorId;
	_sensorPin=sensorPin;
	_minTempAlert=minTempAlert;
	_maxTempAlert=maxTempAlert;
	_alarmCheck=alarmCheck;
	_oneWire=OneWire(_sensorPin);
}

float Sensor::GetCurrentTemp()
{
	DallasTemperature _sensor=DallasTemperature(&_oneWire);
	_sensor.begin();
	_sensor.requestTemperatures(); // Send the command to get temperatures
	return _sensor.getTempCByIndex(0);
}

void  Sensor::PrintSensors()
{
	if(_sensorPin==100)
		return;

	Serial.print("Current temp: ");Serial.println(GetCurrentTemp());
	Serial.print("SensorPin: ");Serial.println(_sensorPin);
	Serial.print("alarmCheck: ");Serial.println(_alarmCheck);
	Serial.print("_minTempAlert: ");Serial.println(_minTempAlert);
	Serial.print("_maxTempAlert: ");Serial.println(_maxTempAlert);
	Serial.println();

}

String Sensor::GetStateInJson(bool allFields)
{
	String result;
	char*  string;
	aJsonObject* root = aJson.createObject();
	aJsonObject* sensor = aJson.createObject();
	aJson.addItemToObject(root,"Sensor",sensor);
	aJson.addNumberToObject(sensor,"ArduinoArraySensorsId",_sensorId);
	aJson.addNumberToObject(sensor,"currentTemp",GetCurrentTemp());
	if(allFields)
	{
		aJson.addNumberToObject(sensor,"AttachedPin",_sensorPin);
		aJson.addNumberToObject(sensor,"alarmCheck",_alarmCheck);
		aJson.addNumberToObject(sensor,"minTempAlert",_minTempAlert);
		aJson.addNumberToObject(sensor,"maxTempAlert",_maxTempAlert);
	}	
	string = aJson.print(sensor);
	result=string;
	free(string);	
	aJson.deleteItem(root);
	return result;
}

boolean  Sensor::Check()
{
	if(_sensorPin!=100&&_alarmCheck==true)
	{
		float currentTemp =GetCurrentTemp();
		if(currentTemp>_minTempAlert && currentTemp<_maxTempAlert)
		{
			return false;
		}
		else 
		{
			return true;
		}
	}
	else 
	{
		return false;
	}
}

void Sensor::Alert()
{
	Serial.println("----------------------");
	Serial.println("Alert");
	PrintSensors();
	Serial.println("----------------------");
}

void Sensor::TurnOffAlarm()
{
	_alarmCheck=false;
}

void Sensor::Delete()
{
	_alarmCheck=false;
	_sensorPin=100;
	_oneWire=OneWire(100);
	_changeStateAlarm=false;
}

void Sensor::SaveChanges()
{
	int index=StartInedexSensor;
	index=index+_sensorId*sizeof(Sensor);
	eeprom_write_block((const void*)this, (void*)index, sizeof(Sensor));
}

Termostat::Termostat(int thermostatId,Sensor &sensor,boolean  termostat,boolean  behavior,float targetTemp,int termostatDevicePin)
{
	_thermostatId=thermostatId;
	_targetTemp=targetTemp;
	_behavior=behavior;
	_termostat=termostat;
	_termostatDevicePin=termostatDevicePin;
	_sensor=&sensor;

	if(!termostat)
	{
		TurnOff();
	}
}

Termostat::Termostat()
{ 
	_targetTemp=0.0;
	_behavior=true;
	_termostat=false;
	_termostatDevicePin=100;
	_sensor=NULL;
}

void  Termostat::PrintTermostat()
{
	if(_sensor!=NULL)
	{
		Serial.print("Thermostat Id ");Serial.println(_thermostatId);
		Serial.print("Target temp: ");Serial.println(_targetTemp);
		Serial.print("Termostat: ");Serial.println(_termostat);
		Serial.print("Behavior: ");Serial.println(_behavior);
		Serial.print("TermostatDevicePin: ");Serial.println(_termostatDevicePin);
		
	}
}

void Termostat::Check()
{
	if (_termostat == true&&_sensor!=NULL)
	{
		char s[10];
		float currentTemp = _sensor->GetCurrentTemp();
		String result="Pin: ";
		result.concat(_sensor->_sensorPin);
		result.concat("Current temp: ");
		result.concat(dtostrf(currentTemp,2,1,s));
		if (currentTemp < _targetTemp)
		{
			digitalWrite(_termostatDevicePin,_behavior);
		}
		else
		{
			digitalWrite(_termostatDevicePin,!_behavior);
		}
	}
}

void Termostat::TurnOff()
{
	digitalWrite(_termostatDevicePin,LOW);
}

float Termostat::GetCurrentTemp()
{
	return _sensor->GetCurrentTemp();
}


void Termostat::Delete()
{
	_termostat=false;
	TurnOff();
}

void Termostat::SaveChanges()
{
	int index=StartInedexTermostat;
	index=index+_thermostatId*sizeof(Termostat);
	eeprom_write_block((const void*)this, (void*)index, sizeof(Termostat));
}


