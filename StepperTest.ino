#include <Servo.h>
#include <AFMotor.h>

AF_DCMotor motor(1);
Servo myservo;  // create servo object to control a servo
// twelve servo objects can be created on most boards

int pos = 30;    // variable to store the servo position

void setup() {
   myservo.attach(9);  // attaches the servo on pin 9 to the servo object
   motor.setSpeed(200);
   motor.run(RELEASE);
}

int i;
void loop() {
   for (pos = 0; pos <= 30; pos += 1) { // goes from 0 degrees to 180 degrees
     // in steps of 1 degree
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(45);                       // waits 15ms for the servo to reach the position
   }
   for (pos = 30; pos <= 60; pos += 1) { // goes from 0 degrees to 180 degrees
     // in steps of 1 degree
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(45);                       // waits 15ms for the servo to reach the position
   }
   for (pos = 60; pos <= 90; pos += 1) { // goes from 0 degrees to 180 degrees
     // in steps of 1 degree
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(45);                       // waits 15ms for the servo to reach the position
   }
   for (pos = 90; pos <= 120; pos += 1) { // goes from 0 degrees to 180 degrees
     // in steps of 1 degree
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(45);                       // waits 15ms for the servo to reach the position
   }
   for (pos = 120; pos >= 0; pos -= 1) { // goes from 180 degrees to 0 degrees
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(15);                       // waits 15ms for the servo to reach the position
   }

   motor.run(FORWARD);
  for (i=0; i<255; i++) {
    motor.setSpeed(i);  
    delay(3);
 }
 
  for (i=255; i!=0; i--) {
    motor.setSpeed(i);  
    delay(3);
 }
 
//  motor.run(BACKWARD);
//  for (i=0; i<255; i++) {
//    motor.setSpeed(i);  
//    delay(3);
// }
// 
//  for (i=255; i!=0; i--) {
//    motor.setSpeed(i);  
//    delay(3);
// }
}
