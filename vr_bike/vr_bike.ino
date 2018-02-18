#include <math.h>
#include <SerialCommand.h>

#define RADIUS 0.3
#define THRESHOLD 8.0
#define MIN_SPEED 0.3
#define MAX_SPEED 7.5
#define TICK 20

#define LOWER_BOUND 116
#define UPPER_BOUND 846
#define ANGLE_RANGE 180
#define POT_PIN 2

SerialCommand sCmd;
float r_sensor; //Resistance of sensor in K
int revolutions = 0;
int ms = 0;
float mps = 0;
float heading = 0;
int prev_ms = 0;
float prev_mps = 0;
bool prev_dark = false;


void setup() {
 Serial.begin(9600); //Start the Serial connection
}

void loop() {
 Serial.println(String(mps) + " " + String(heading));
 int sensor_value = analogRead(0);
 r_sensor = (float)(1023-sensor_value)*10/sensor_value;
 //Serial.println(r_sensor);
 bool dark = r_sensor > THRESHOLD;
 if (dark ^ prev_dark) {
  prev_dark = dark;
  revolutions++;
  update_speed(true);
 } else {
  update_speed(false);
 }
 update_heading();
 delay(TICK);
 ms += TICK;
}

void update_speed(bool boundary) {
  int delta = ms - prev_ms;
  mps = M_PI*RADIUS/((float)delta/1000);
  if (boundary) {
    prev_ms = ms;
    prev_mps = mps;
  };
  mps = min(prev_mps, mps);
  mps = (mps < MIN_SPEED) ? 0 : mps;
  mps = min(mps, MAX_SPEED);
}

float rRange = (UPPER_BOUND - LOWER_BOUND);
float centrePoint = rRange / 2 + LOWER_BOUND;
float degreePerR = ANGLE_RANGE / rRange;

void update_heading() {
  int sensor_val = analogRead(POT_PIN);
  heading = (sensor_val-centrePoint) * degreePerR;
  //Serial.println(heading);
}
