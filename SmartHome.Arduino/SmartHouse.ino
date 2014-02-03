#include "Arduino.h"
#include <EDB.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include <avr/eeprom.h>
#include <House.h>
#include <EEPROM.h>
#include <SPI.h>
#include <Ethernet.h>
#include <avr/pgmspace.h>
#include <aJSON.h>
#include <YalerEthernetServer.h>

#pragma region StartDefinition

#define StartInedexTermostat 44		
#define StartInedexSensor 1300		
#define StartInedexPinState 1000	
#define StartInedexConfiguration 2000

#define ArraySensorsSize 20
#define ArrayTermostatsSize 20
#define MaxNumberPins 54

#define SecretKey "2f40727a2b8c93bd3952"

#define DefaultTimeToSendTemp 60000
#define DefaultTimeToCheckTemp 60000
#define ServerPort 8080
#define ApiControllerURL "/Api/Arduino/"

struct Configuration
{  
	long timeToCheckTemp; 
	long timeToSendTemp; 
	int serverPort;
	IPAddress serverIp;
}configuration;


Sensor sensors[ArraySensorsSize];		  
Termostat termostats[ArrayTermostatsSize];
PinState pinsState[MaxNumberPins];  


byte mac[] = {0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED }; 
EthernetClient ethernetClient;

EthernetServer server(80);
//YalerEthernetServer server("try.yaler.net", 80, "gsiot-nn6r-wbbg");

IPAddress arduinoIp(192,168,3,2);
//IPAddress serverIp(192,168,2,108);

unsigned long time,checkTemploop,checkSendTemp;

#pragma endregion

void setup()
{
	Serial.begin(9600);
	delay(1000);
	freeMem("start");

	//ClearStateEEPROM();
	LoadConfigurationFromEEPROM();
	LoadStateFromEEPROM();

#pragma region DefaultState
	//configuration.serverPort=ServerPort;
	//configuration.serverIp=serverIp;
	//configuration.timeToCheckTemp=DefaultTimeToCheckTemp;
	//configuration.timeToSendTemp=DefaultTimeToSendTemp;
#pragma endregion 

#pragma region PrintInfo

	Serial.println("---------Print Sensors---------");
	PrintSensors();

	Serial.println("------Print Pin State------------");
	PrintPinsState();	

	Serial.println("---------Print Termostat---------");
	PrintTermostats();

	Serial.println("--------Print Configuration----------");
	PrintConfiguration();

	Serial.println("--------End Print----------");
#pragma endregion

	InitPinsState();	
	Ethernet.begin(mac,arduinoIp);	
	server.begin();

	unsigned long milis=millis();
	time=checkTemploop=checkSendTemp=milis;		
}

void loop() 
{

	while(true)
	{	
		time=millis();
		if (time >= checkTemploop)
		{ 
			checkTemploop = time +configuration.timeToCheckTemp;
			CheckTermostats();
			CheckSensorAlarm();
		}
		if(time >= checkSendTemp)
		{
			checkSendTemp=time +configuration.timeToSendTemp;
			SendTempsToServer();
		}
		CheckClient();
	}
}

void CheckClient()
{
	boolean enterData=false;
	char c;
	String readString = "";
	EthernetClient client = server.available();
	if (client)
	{
		readString="";
		while (client.connected())
		{
			if(client.available()) 
			{
				if(enterData==true)
				{				
					readString="";
					while (client.available())
					{
						c = client.read();
						readString += c;
					}
					enterData=false;
					client.println("HTTP/1.1 200 OK");
					client.println("Content-Type: application/json");
					client.println("Connnection: close");
					client.println();

					String result=ParseReadString(readString);
					Serial.println(result);
					client.println(result);
					delay(500);
					client.stop();
					return;
				}
				c = client.read();   //read char by char HTTP request
				readString +=c;

				if( readString.equals("POST"))
				{
					while (client.available())  
					{
						c = client.read();
						Serial.print(c);
						while(c != '\n')
						{
							c = client.read();

							Serial.print(c);
						}
						c = client.read();
						if (c=='\r') 
						{
							c = client.read();
						}
						if (c=='\n')
						{ 
							enterData=true;
							break;
						}
					} 
				} 
			}        
		}	     
	}
}

String ParseReadString(String message)
{
	String status="ok";
	String returnMessage="";
	freeMem("after  ProcessingJson");
	aJsonObject* dataJson = aJson.parse(&message[0]);
	if (dataJson != NULL) 
	{
#pragma region SetDevice
		if(aJson.getObjectItem(dataJson, "SetDevice") !=NULL)
		{	
			aJsonObject* setDeviceJson = aJson.getObjectItem(dataJson, "SetDevice"); 
			SetDeviceFromJson(setDeviceJson);
		}

#pragma endregion

#pragma region SetDevices
		else if(aJson.getObjectItem(dataJson, "SetDevices") !=NULL)
		{
			aJsonObject* setDevicesJson = aJson.getObjectItem(dataJson, "SetDevices");
			int arraySize=aJson.getArraySize(setDevicesJson);
			for(int i=0;i<arraySize;i++)
			{
				aJsonObject* setDeviceJson=aJson.getArrayItem(setDevicesJson,i);
				SetDeviceFromJson(setDeviceJson);
			}
		}
#pragma endregion

#pragma region SetDeleteDevice
		else if(aJson.getObjectItem(dataJson, "SetDeleteDevice") !=NULL)
		{
			byte pin;
			aJsonObject* setPinMode = aJson.getObjectItem(dataJson, "SetDeleteDevice"); 
			aJsonObject* pinJson=aJson.getObjectItem(setPinMode, "Pin");			
			pin=pinJson->valueint;	
			pinsState[pin].SetPinState(false);
			pinsState[pin].SaveChanges();
		}
#pragma endregion

#pragma region SetTermostat
		else if(aJson.getObjectItem(dataJson, "SetTermostat") !=NULL)
		{
			


			int termostatId;
			byte sensorId;
			int termostatDevicePin;
			boolean termostatState;
			boolean behavior;
			float targetTemp;

			char* SetTermostatJson;
			aJsonObject* termostatJson = aJson.getObjectItem(dataJson, "SetTermostat");
			SetTermostatJson=aJson.print(termostatJson);
			Serial.println(SetTermostatJson);

			aJsonObject* termostatIdJson=aJson.getObjectItem(termostatJson, "TermostatId");
			termostatId=termostatIdJson->valueint;
			Serial.print("Termostat id: ");Serial.println(termostatId);

			aJsonObject* sensorIdJson=aJson.getObjectItem(termostatJson, "SensorId");
			sensorId=sensorIdJson->valueint;
			Serial.print("sensorId : ");Serial.println(sensorId);

			aJsonObject* termostatDevicePinJson=aJson.getObjectItem(termostatJson, "TermostatDevicePin");
			termostatDevicePin=termostatDevicePinJson->valueint;
			Serial.print("termostatDevicePin : ");Serial.println(termostatDevicePin);

			aJsonObject* SendTermostatStateJsonToServer=aJson.getObjectItem(termostatJson, "TermostatState");
			termostatState=SendTermostatStateJsonToServer->valuebool;
			Serial.print("termostatState : ");Serial.println(termostatState);				

			termostats[termostatId]._termostat=termostatState;

			if(termostatState)
			{
				aJsonObject* behaviorJson=aJson.getObjectItem(termostatJson, "Behavior");
				behavior=behaviorJson->valuebool;
				Serial.print("behavior : ");Serial.println(behavior);

				aJsonObject* targetTempJson=aJson.getObjectItem(termostatJson, "TargetTemp");
				targetTemp=targetTempJson->valuefloat;
				Serial.print("targetTemp : ");Serial.println(targetTemp);

				termostats[termostatId]._behavior=behavior;
				termostats[termostatId]._targetTemp=targetTemp;
			}
			else 
			{
				termostats[termostatId].TurnOff();
			}

			if(sensorIdJson!=NULL)
			{			
				termostats[termostatId]._thermostatId=termostatId;
				termostats[termostatId]._termostatDevicePin=termostatDevicePin;
				termostats[termostatId]._sensor=&sensors[sensorId];
			}
			termostats[termostatId].SaveChanges();

			free(SetTermostatJson);
			freeMem("after deletion");
			PrintTermostats();
			CheckTermostats();
		}
#pragma endregion

#pragma region SetDeleteThermostat
		else if(aJson.getObjectItem(dataJson, "SetDeleteThermostat") !=NULL)
		{			
			aJsonObject* termostatJson = aJson.getObjectItem(dataJson, "SetDeleteThermostat");
			aJsonObject* termostatIdJson=aJson.getObjectItem(termostatJson, "TermostatId");
			int termostatId=termostatIdJson->valueint;
			Serial.print("Termostat id: ");Serial.println(termostatId);

			termostats[termostatId].Delete();
			termostats[termostatId].SaveChanges();


		}
#pragma endregion

#pragma region SetSensor

		else if(aJson.getObjectItem(dataJson, "SetSensor") !=NULL)
		{
			aJsonObject* setSensorJson = aJson.getObjectItem(dataJson, "SetSensor");
			boolean alarmCheck=false;
			float minTempAlert;
			float maxTempAlert;
			byte sensorPin;
			byte sensorId;
			aJsonObject* sensorIdJson=aJson.getObjectItem(setSensorJson, "SensorId");
			sensorId=sensorIdJson->valueint;
			sensors[sensorId]._sensorId=sensorId;

			aJsonObject* alartmCheckJson=aJson.getObjectItem(setSensorJson, "AlarmCheck");
			Serial.print("alarm value:");
			Serial.println(alartmCheckJson->valuebool);
			alarmCheck=alartmCheckJson->valuebool;
			sensors[sensorId]._alarmCheck=alarmCheck;

			Serial.print("alarmCheck : ");Serial.println(alarmCheck);

			if(alarmCheck)
			{
				aJsonObject* minTempAlertJson=aJson.getObjectItem(setSensorJson, "MinTempAlert");
				aJsonObject* maxTempAlertJson=aJson.getObjectItem(setSensorJson, "MaxTempAlert");
				sensors[sensorId]._minTempAlert=minTempAlertJson->valuefloat;
				sensors[sensorId]._maxTempAlert=maxTempAlertJson->valuefloat;
			}
			aJsonObject* sensorPinJson=aJson.getObjectItem(setSensorJson, "SensorPin");
			if(sensorPinJson!=NULL)
			{
				sensors[sensorId]._sensorPin=sensorPinJson->valueint;
				sensors[sensorId]._oneWire=OneWire(sensorPinJson->valueint);
			}
			sensors[sensorId].SaveChanges();

			PrintSensors();
			freeMem("after SetSensorFromJson");
		}
#pragma endregion 

#pragma region SetDeleteSensor
		else if(aJson.getObjectItem(dataJson, "SetDeleteSensor") !=NULL)
		{		
			byte sensorId;
			aJsonObject* sensorJson=aJson.getObjectItem(dataJson, "SetDeleteSensor");
			aJsonObject* sensorIdJson=aJson.getObjectItem(sensorJson, "SensorId");
			sensorId=sensorIdJson->valueint;

			sensors[sensorId].Delete();
			sensors[sensorId].SaveChanges();

			PrintSensors();
			freeMem("after SetSensorFromJson");
		}
#pragma endregion 

#pragma region SetConfiguration
		else if(aJson.getObjectItem(dataJson, "SetConfiguration") !=NULL)
		{
			aJsonObject* setConfigurationJson = aJson.getObjectItem(dataJson, "SetConfiguration");
			aJsonObject* serverIpJson=aJson.getObjectItem(setConfigurationJson, "ServerIp");
			aJsonObject* serverPortJson=aJson.getObjectItem(setConfigurationJson, "ServerPort");
			aJsonObject* timeToCheckTempJson=aJson.getObjectItem(setConfigurationJson, "TimeToCheckTemp");
			aJsonObject* timeToSendTempJson=aJson.getObjectItem(setConfigurationJson, "TimeToSendTemp");

			if(serverIpJson!=NULL)
			{
				String ip=serverIpJson->valuestring;
				byte a,b,c,d;						
				sscanf(&ip[0],"%i,%i,%i,%i",&a,&b,&c,&d);
				configuration.serverIp=IPAddress(a,b,c,d);
			}
			if(serverPortJson!=NULL)
			{
				configuration.serverPort=serverPortJson->valueint;
			}			
			long  timeToCheckTemp=60*1000L*timeToCheckTempJson->valueint;
			long  timeToSendTemp=60*1000L*timeToSendTempJson->valueint;
			configuration.timeToCheckTemp=timeToCheckTemp;
			configuration.timeToSendTemp=timeToSendTemp;
			eeprom_write_block((const void*)&configuration, (void*)StartInedexConfiguration, sizeof(configuration));
			PrintConfiguration();

		}
#pragma endregion

#pragma region getTemps
		else if(aJson.getObjectItem(dataJson, "getTemps") !=NULL)
		{
			aJsonObject* sensorJson=aJson.getObjectItem(dataJson, "getTemps");
			returnMessage=CreateStringWithSensors(100,false);
		}
#pragma endregion

#pragma region ClearEEPROM
		else if(aJson.getObjectItem(dataJson, "ClearEEPROM") !=NULL)
		{
			aJsonObject* termostat=aJson.getObjectItem(dataJson, "ClearEEPROM");
			ClearStateEEPROM();
			InitPinsState();
		}


#pragma endregion
		else 
		{
			status="error";
		}
	}
	else 
	{
		status="error";
	}
	aJson.deleteItem(dataJson);
	String result;	
	result="{\"Status\":\"" +status+"\",\"Message\":"; 	
	if(returnMessage=="")
	{
		result+="\"\"";
	}
	else 
	{
		result+=returnMessage;
	}
	result+="}";
	freeMem("after deletionEnd");
	return result;
}

void SendDateToServer(String dataSent,String api)
{
	Serial.print("Ip:"); 
	Serial.println(configuration.serverIp); 
	Serial.print("port:"); 
	Serial.println(configuration.serverPort); 
	Serial.println(dataSent); 

	if (dataSent!=""&&ethernetClient.connect(configuration.serverIp, configuration.serverPort)) {
		Serial.println("connect to server");
		ethernetClient.print("POST ");
		ethernetClient.print(ApiControllerURL);
		ethernetClient.print(api);			
		ethernetClient.println(" HTTP/1.1");
		ethernetClient.print("Host: ");
		ethernetClient.println(configuration.serverIp);
		ethernetClient.println("User-Agent: Arduino/1.0");
		ethernetClient.println("Connection: close");
		ethernetClient.println("Content-Type: application/json");
		ethernetClient.print("Content-Length: ");
		ethernetClient.println(dataSent.length());
		ethernetClient.println();
		ethernetClient.println(dataSent);
		ethernetClient.stop();
	}
	else{
		Serial.println("not connect");
		ethernetClient.stop();
	}
}

void SendTempsToServer()
{
	String jsonSensors=CreateStringWithSensors(100,false);
	if(jsonSensors!=NULL)
	{	
		SendDateToServer(jsonSensors,"PostSensors");
	}
}

String CreateStringWithSensors(int sensorId,bool allFields)
{
	bool sentSensort=false;
	String json;
	String result;
	if(sensorId==100)
	{
		for(int i=0;i<20;i++)
		{
			if(sensors[i]._sensorPin==100)
			{
				continue;
			}
			json.concat(sensors[i].GetStateInJson(allFields));
			json.concat(",");
			sentSensort=true;
		}
	}
	else 
	{
		if(sensors[sensorId]._sensorPin!=100)
		{
			json.concat(sensors[sensorId].GetStateInJson(allFields));
			sentSensort=true;
		}
	}
	if(sentSensort)
	{
		result.concat("{\"Temps\":[");
		result.concat(json);
		result.concat("]}");
	}
	return result;
}

void SetDeviceFromJson(aJsonObject* json)
{
	byte pin;
	boolean state;	 
	aJsonObject* pinJson=aJson.getObjectItem(json, "Pin");
	aJsonObject* stateJson=aJson.getObjectItem(json, "State");
	pin=pinJson->valueint;
	state=stateJson->valuebool;
	pinsState[pin].SetPinState(state);
	pinsState[pin].SaveChanges();	
}

void CheckTermostats()
{
	for(int i=0;i<ArrayTermostatsSize;i++)
	{
		termostats[i].Check();
	}
}

void CheckSensorAlarm()
{
	for(int i=0;i<ArraySensorsSize;i++)
	{
		if(sensors[i].Check())
		{
			AlertSensor(i);
			sensors[i].TurnOffAlarm();
			sensors[i].SaveChanges();
		}
	}
}

void AlertSensor(int sensorId)
{
	sensors[sensorId].Alert();
	String sensor=sensors[sensorId].GetStateInJson(false);
	SendDateToServer(sensor,"PostAlarm");
}

void InitPinsState()
{
	for(int i=1;i<MaxNumberPins;i++)
	{
		pinsState[i]._pin=i;
		if(pinsState[i]._mode!=3)
		{
			pinMode(i,pinsState[i]._mode);
			digitalWrite(i,pinsState[i]._state);
		}
	}

}

void LoadConfigurationFromEEPROM()
{
	eeprom_read_block((void*)&configuration, (void*)StartInedexConfiguration, sizeof(configuration));
}

void LoadStateFromEEPROM()
{
	eeprom_read_block((void*)&termostats, (void*)StartInedexTermostat, sizeof(termostats));
	eeprom_read_block((void*)&sensors, (void*)StartInedexSensor, sizeof(sensors));
	eeprom_read_block((void*)&pinsState, (void*)StartInedexPinState, sizeof(pinsState));

}

void ClearStateEEPROM()
{
	freeMem("Before clear ram");
	Termostat clearTermostats[ArrayTermostatsSize];
	Sensor clearSensors[ArraySensorsSize];
	PinState clearPinsState[MaxNumberPins];
	Configuration clearConfiguration={DefaultTimeToCheckTemp,DefaultTimeToSendTemp,ServerPort,(0,0,0,0)};
	eeprom_write_block((const void*)&clearTermostats, (void*)StartInedexTermostat, sizeof(termostats));
	eeprom_write_block((const void*)&clearSensors, (void*)StartInedexSensor, sizeof(clearSensors));
	eeprom_write_block((const void*)&clearPinsState, (void*)StartInedexPinState, sizeof(clearPinsState));
	eeprom_write_block((const void*)&clearConfiguration, (void*)StartInedexConfiguration, sizeof(clearConfiguration));
	freeMem("After clear ram");
}

#pragma region Print

void PrintTermostats()
{
	for(int i=0;i<ArrayTermostatsSize;i++)
	{
		termostats[i].PrintTermostat();
	}
}

void PrintSensors()
{
	for(int i=0;i<ArraySensorsSize;i++)
	{
		sensors[i].PrintSensors();
	}
}

void PrintPinsState()
{
	for(int i=1;i<54;i++)
	{	
		pinsState[i].PrintPinState();
	}
}

void PrintConfiguration()
{
	Serial.print("ServerIP IP address: ");
	Serial.println(configuration.serverIp);
	Serial.print("timeToSendTemp : ");Serial.println(configuration.timeToSendTemp);
	Serial.print("TimeToCheckTemp : ");Serial.println(configuration.timeToCheckTemp);
}
#pragma endregion 

#pragma region Memory

struct __freelist {
	size_t sz;
	struct __freelist *nx;
};

extern char * const __brkval;
extern struct __freelist *__flp;

uint16_t freeMem(uint16_t *biggest)
{
	char *brkval;
	char *cp;
	unsigned freeSpace;
	struct __freelist *fp1, *fp2;

	brkval = __brkval;
	if (brkval == 0) {
		brkval = __malloc_heap_start;
	}
	cp = __malloc_heap_end;
	if (cp == 0) {
		cp = ((char *)AVR_STACK_POINTER_REG) - __malloc_margin;
	}
	if (cp <= brkval) return 0;

	freeSpace = cp - brkval;

	for (*biggest = 0, fp1 = __flp, fp2 = 0;
		fp1;
		fp2 = fp1, fp1 = fp1->nx) {
			if (fp1->sz > *biggest) *biggest = fp1->sz;
			freeSpace += fp1->sz;
	}

	return freeSpace;
}

uint16_t biggest;

void freeMem(char* message) {
	Serial.print(message);
	Serial.print(":\t");
	Serial.println(freeMem(&biggest));
}
#pragma endregion 