import datetime
import time
import threading
from tkinter import *

def ACAction():
    while True:
        global ACState
        global rev
        global now
        # print(ACState)
        if ACState:
            #TODO:include temperature control for AC
            #currentTemperature = readDHT()
            # print('AC is on')
            global action
            if(action[(now.hour+1)%24] == 0):
                global b2
                if not 60-b2>now.minute:
                    ACState = False
                    rev = 2
        else:
            # print('AC is off')
            if(action[(now.hour+1)%24] == 1):
                global b1
                if not 60-b1>now.minute:
                    ACState = True
                    rev = 1
        
def Review():
    while True:
        global rev
        global b1
        global b2
        global action
        if action[(now.hour + 1) % 24] == 1 and "Yes" not in str(userState.get(0.0,END)):
            userState.delete(0.0, END)
            userState.insert(END, "Yes")
        elif action[(now.hour + 1) % 24] == 0 and "No" not in str(userState.get(0.0,END)):
            userState.delete(0.0, END)
            userState.insert(END, "No")
        if not rev == 0:
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
                    action[now.hour] = 1
            rev = 0


def ProgramTime():
    while True:
        global now
        now = datetime.datetime.now()
        x = now.microsecond
        time.sleep(float((1000000 - x)/1000000))

Window = Tk()
Window.title("IoT by iOSDev")
Window.configure(background="black")
action = [0,1,0,0,0,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,1]
ACState = False
b1 = 15 #time before AC starts
b2 = 5 #time before AC stops
rev = 0
now = datetime.datetime.now()

t1 = threading.Thread(target=ACAction)
t2 = threading.Thread(target=Review)
t3 = threading.Thread(target=ProgramTime)
##creating labels and button for GUI
Label(Window, text="IoT by iOSDev", bg="black", fg="green", font="none 20").grid(row=0, column=0, sticky=W)

Label(Window, text="Is user coming in next hour:", bg="black", fg="white", font="none 13").grid(row=3, rowspan=3,column=0, sticky=W)
userState = Text(Window, width=3, height=1, wrap=WORD, background="black", fg="white")
userState.grid(row=4, column=2, columnspan=2, sticky=E)

Label(Window, text="When you enter the room, how the temperature is?:", bg="black", fg="white", font="none 15").grid(row=9, rowspan=3,column=0, sticky=W)
buttonHot_b1 = Button(Window, text="HOT~~",bg="black",fg="red").grid(row=17,column=0,sticky=E)
buttonNeutral_b1 = Button(Window, text="Neutral :)",bg="black",fg="yellow").grid(row=17,column=1,sticky=W)
buttonCool_b1 = Button(Window, text="COOL!!",bg="black",fg="blue").grid(row=17,column=2,sticky=W)

Label(Window, text="When you exit the room, how the temperature is?:", bg="black", fg="white", font="none 15").grid(row=22, rowspan=3,column=0, sticky=W)
buttonHot_b2 = Button(Window, text="HOT~~",bg="black",fg="red").grid(row=27,column=0,sticky=E)
buttonNeutral_b2 = Button(Window, text="Neutral :)",bg="black",fg="yellow").grid(row=27,column=1,sticky=W)
buttonCool_b2 = Button(Window, text="COOL!!",bg="black",fg="blue").grid(row=27,column=2,sticky=W)

t1.start()
t2.start()
t3.start()
Window.mainloop()
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
