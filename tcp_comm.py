#!/usr/bin/python
import socket
import sys
import time
import string
import thread
import Queue
import serial
import os

start_time = None
interval = None
num_interval = None
angA = []
angH = []
data = None
ser = serial.Serial('/dev/ttyACM0', 38400)
t = 0
START = 0

def Init():
        global s, port
        s = socket.socket()
        port = 12345
        s.connect(('192.168.0.134', port))
        print('Connected')

def streamCheck(stream):
    msg = stream.split(',')
    msg = list(filter(None, msg))
    return(msg)

def DataIn():
    global start_time, interval, num_interval, angA, angH, data
    while True:
        msg = s.recv(1024)
        if msg is not None:
                data = streamCheck(msg)
                print(data)
                #print(len(data))
                if ((len(data)) > 0):
                    start_time = data[0]
                    interval = data[1]
                    num_interval = int(data[2])
                    angA = data[3:3+num_interval]
                    angH = data[3+num_interval+1:len(data)-1]
                    #print('start %s, int %s, num %s, angA %s, angH %s' %(start_time, interval, num_interval, angA, angH))
                    #q.task_done()
            #q.put(msg)
                    
def start_time_check():
        global START
        if start_time is not None:
                ti = time.localtime()
                hour = ti.tm_hour*100
                minute = ti.tm_min
                time1 = hour+minute
                if abs(int(start_time)-time1) == 0:
                        START = 1
                        print('Starting')

        
def Send2Arduino():
        global t, START
        #when time to send
        print('checking start')
        print(START)
        if START is 1:
                print('start found')
                while t <= int(num_interval)-1:
                        ser.write('#') #start indicator
                        time.sleep(2)
                        ser.write(str(angA[t]))
                        time.sleep(2)
                        ser.write(str(angH[t]))
                        t = t + 1
                        print('Sent angA, angH')
                        time.sleep(int(interval))

                t = 0
                
                START = 0
                start_time = None
                        
        
if __name__ == '__main__':
    print('init')
    Init()
    print('finished init')
    #q.join()
    #Init thread to receive data from TCP comm
    t1 = thread.start_new_thread(DataIn, ())
    while True:
        try:
                start_time_check()
                Send2Arduino()
                #print('start %s, int %s, num %s, angA %s, angH %s' %(start_time, interval, num_interval, angA, angH))
        except KeyboardInterrupt:
            break

    t1.exit()
    s.close()
    sys.exit()
                
        
