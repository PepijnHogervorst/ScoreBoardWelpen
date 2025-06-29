# Interactive score board

A RGB scoreboard for 6 teams to keep track of the team score during `Welpenkamp`.
A intelegent scoreboard that keeps track of score, person that should press the button and controls led strips using an arduino with ethernet communication. The communication between PC and arduino is MQTT. Using simple topics

## How to use

* Laptop is required with ethernet connection (wifi won't work)
* Ethernet cable between laptop and controller

Ethernet settings:  

* IP Laptop / PC = `192.168.178.99`
* IP Arduino = `192.168.178.98`

## MQTT API

MQTT settings:

* No username / password
* Port 1883

### Scoreboard program

Topic: `Scoreboard/ButtonProgram`

Payload:

```json
{
    "Group":1,
    "Points":55
}
```

The arduino will respond with a ready signal if the button is pressed:

Topic: `Scoreboard/Ready`

Payload: No payload required

### Party mode

Topic: `Scoreboard/Party`

Payload:

```json
{
    "Program":0,
    "Delay":1
}
```

Where program is a number with enum values:

``` csharp
public enum PartyProgram
{
    FullRainbow = 0,
    RandomRain,
    ColorWipe,
    CollorCyclePillar,
}
```

### Strobe

Topic: `Scoreboard/Strobe`

Payload:

```json
{
    "Program":0,
    "Delay":1
}
```
