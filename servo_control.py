#!/usr/bin/python
import os
import time
import RPi.GPIO as gpio
import serial

ser = serial.Serial('/dev/ttyACM0', 38400)


try:
    k = 180
    l = 30
    i = 0;    
    while True:
        ser.write('#13')
        t = time.localtime()
        hour = t.tm_hour*100
        minute = t.tm_min
        time1 = hour+minute
        print(time1)
        i = i + 1;
        time.sleep(2)
except KeyboardInterrupt:
    gpio.cleanup()
    ser.close()
