#include <Servo.h>
#include <AFMotor.h>

AF_DCMotor motor(4);
Servo myservo;  // create servo object to control a servo
// twelve servo objects can be created on most boards
int pos = 0;    // variable to store the servo position


String in;
char buf[200];
int oneIfATwoIfH = 0;
int aSteps = 0;
int servoAngle = 0;
int Ready = 0;
int oldServo = 0;
int oldA = 0;
   
void setup(){
  
  //Serial.begin(11400);
  Serial.begin(38400);
   myservo.attach(9);  // attaches the servo on pin 9 to the servo object
   motor.setSpeed(200);
   motor.run(RELEASE);
   
   for (pos = 180; pos >= 0; pos -= 1) { // goes from 180 degrees to 0 degrees
     myservo.write(pos);              // tell servo to go to position in variable 'pos'
     delay(15);                       // waits 15ms for the servo to reach the position
   }
     // turn on motor
  motor.setSpeed(200);

  motor.run(RELEASE);
}

void loop()
{
  if(Serial.available() >0)
  {
    in = Serial.readString();
    //in = Serial.parseInt();

    if(in.equals("#")){
      oneIfATwoIfH = 1;
    
    } else if(oneIfATwoIfH == 1){
      aSteps = in.toInt(); 
      oneIfATwoIfH = 2;
    
    } else if(oneIfATwoIfH == 2){
      servoAngle = in.toInt();
      oneIfATwoIfH = 0;
      Ready = 1;
  } 
  
  if(Ready == 1){
    Serial.print("Angle A Steps: ");
    Serial.println(aSteps);
    Serial.print("Servo Angle: ");
    Serial.println(servoAngle);
    if(oldServo <= servoAngle){
     for (pos = oldServo; pos <= servoAngle; pos += 1) { // goes from 0 degrees to 180 degrees 
       myservo.write(pos);              // tell servo to go to position in variable 'pos'
       delay(45); 
      }
                      // waits 15ms for the servo to reach the position
   } else if(oldServo > servoAngle){
      for (pos = oldServo; pos >= servoAngle; pos -= 1) { // goes from 0 degrees to 180 degrees 
       myservo.write(pos);              // tell servo to go to position in variable 'pos'
       delay(45); 
      }
   }
     if(aSteps > oldA and (aSteps - oldA < 180)){
    motor.run(BACKWARD);
    motor.setSpeed(255);
    delay(1000*(aSteps-oldA)/30);
    motor.run(RELEASE);
  } else if((oldA > aSteps) and (oldA - aSteps > 180)){
    motor.run(BACKWARD);
    motor.setSpeed(255);
    delay(1000*(360-oldA+aSteps)/30);
    motor.run(RELEASE);
  } else if(oldA > aSteps){
    motor.run(FORWARD);
    motor.setSpeed(255);
    delay(1000*(oldA-aSteps)/30);
    motor.run(RELEASE);    
  }else{
    motor.run(FORWARD);
    motor.setSpeed(255);
    delay(1000*(360-aSteps + oldA)/30);
    motor.run(RELEASE);
  }
   oldA  = aSteps;
   Ready = 0;
   oldServo = servoAngle;
  
    
  }

  }
}
