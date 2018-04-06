import datetime
import time
import threading

def ACAction():
    while True:
        global ACState;global rev;global now
        if ACState:
            #TODO:include temperature control for AC
            #currentTemperature = readDHT()
            print('AC is on')
            global action
            if(action[(now.hour+1)%24] == 0):
                global b2
                if not 60-b2>now.minute:
                    ACState = False
                    rev = 2
        else:
            if(action[(now.hour+1)%24] == 1):
                global b1
                if not 60-b1>now.minute:
                    ACState = True
                    rev = 1
        
                
def Review():
    while True:
        global rev;global b1;global b2
        if not rev == 0:
            rev = 0
            if rev == 1:
                print('1. : HOT~~\n2. : COLD!!\n3. : Fine')
                print('How was the temperature of the room:')
                i = int(input())
                #when user enter
                if i == 1:
                    b1+=3
                if i == 2:
                    b1-=3
            elif rev == 2:
                print('1. : HOT~~\n2. : COLD!!\n3. : Fine')
                print('How was the temperature of the room:')
                i = int(input())
                #when user exit
                if i == 1:
                    b2 -= 1;
                if i == 2:
                    b2 -= 1
            elif rev == 3:
                ##GPS inputs
                print('Do you enter here now regularly at this time?')
                i = input()
                if i.contains('Yes') or i.contains('yes') or i.contains('Y') or i.contains('y') :
                    global action
                    action[now.hour] = 1


def ProgramTime():
    while True:
        global now
        now = datetime.datetime.now()
        x = now.microsecond
        time.sleep(float((1000000 - x)/1000000))

action = [0,1,0,0,0,1,1,1,0,1,0,0,0,0,1,1,1,1,1,1,0,0,0,1]
ACState = False
b1 = 15 #time before AC starts
b2 = 5 #time before AC stops
rev = 0
now = datetime.datetime.now()
t1 = threading.Thread(target=ACAction)
t2 = threading.Thread(target=Review)
t3 = threading.Thread(target=ProgramTime)
t1.start()
t2.start()
t3.start()
t1.join()
t2.join()
t3.join()









##while True:
##    now = datetime.datetime.now()
##
##    print ("Current date and time using str method of datetime object:")
##    print (str(now))
##
##    print ("Current date and time using instance attributes:")
##    print ("Current year: %d" % now.year)
##    print ("Current month: %d" % now.month)
##    print ("Current day: %d" % now.day)
##    print ("Current hour: %d" % now.hour)
##    print ("Current minute: %d" % now.minute)
##    print ("Current second: %d" % now.second)
##    print ("Current microsecond: %d" % now.microsecond)
##
##    print ("Current date and time using strftime:")
##    print (now.strftime("%Y-%m-%d %H:%M"))
##
##    print ("Current date and time using isoformat:")
##    print (now.isoformat())
##    time.sleep(1)
