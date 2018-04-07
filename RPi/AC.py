import datetime
import time
import threading
import requests
from tkinter import *
import socket
import sys
import re

inBlock = False

def orect():
    x1 = 0
    x2 = 400
    y1 = 0
    y2 = 400
    if(int(coordinate[0])>x1 and int(coordinate[0])<x2):
        if(int(coordinate[1])>y1 and int(coordinate[1])<y2):
            return True
    return False

def irect():
    x1 = 100
    x2 = 300
    y1 = 100
    y2 = 300
    if(int(coordinate[0])>x1 and int(coordinate[0])<x2):
        if(int(coordinate[1])>y1 and int(coordinate[1])<y2):
            return True
    return False

def server():
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_address = ('localhost', 10000)
    sock.bind(server_address)
    sock.listen(1)
    while True:
        # Wait for a connection
        connection, client_address = sock.accept()
        try:
            # Receive the data in small chunks and retransmit it
            data = connection.recv(16).decode()
            print('..')
            print(data)
            global coordinate
            d_coordinate = re.sub(r'\s', '', data).split(',')
            coordinate = []
            coordinate.append(int(d_coordinate[0]))
            coordinate.append(int(d_coordinate[1]))
            print(coordinate)
            if not data:
                break
                # connection.sendall(data.encode())
            # else:
            #      break
        finally:
            # Clean up the connection
            connection.close()

def b1_h():
    global rev
    if rev == 1:
        global b1
        b1+=3
        print('b1_h called' + str(b1))
        rev = 0
def b1_c():
    global rev
    if rev == 1:
        global b1
        b1-=3
        print('b1_c called' + str(b1))
        rev = 0
def b2_h():
    global rev
    if rev == 2:
        global b2
        b2-=1
        print('b2_h called' + str(b2))
        rev = 0
def b2_c():
    global rev
    if rev == 2:
        global b2
        b1+=1
        print('b2_c called' + str(b2))
        rev = 0

def rev3_y():
    global rev
    global action
    rev = 0
    action[now.hour%24]=1

def rev0():
    global rev
    rev = 0

def sendURL(state):
    AC_ip = 'http://'+str(ip)+'/iosdev/'+str(ACUID)+str(state)+'/'
    print(AC_ip)
    try:
        r = requests.get(AC_ip)
        # if r.status_code == requests.code.ok:
        #     print('\nMessage sent sucessfully!!\n')
        #     print('Message:')
        #     print(r)
        # else:
        #     print('Error: Check your internet connection and hub')
    except Exception as ex:
        print(ex)


def ACAction():
    while True:
        global ACState
        global rev
        global now
        global inBlock
        # print(ACState)
        if irect() and not inBlock:
            print('User in inner block')
            ACState = True
            rev = 3
            inBlock = True
            sendURL(1)
            continue
        if not orect() and inBlock:
            inBlock = False
            sendURL(0)
        if ACState:
            global action
            if(action[(now.hour+1)%24] == 0):
                global b2
                if (not 60-b2>now.minute):
                    print('AC off and rev = 2')
                    ACState = False
                    sendURL(0)
                    rev = 2
        else:
            # print('AC is off')
            if(action[(now.hour+1)%24] == 1):
                global b1
                if ((not 60-b1>now.minute)and(orect())):
                    print('AC on and rev = 1')
                    ACState = True
                    sendURL(1)
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

def ProgramTime():
    while True:
        global now
        now = datetime.datetime.now()
        x = now.microsecond
        time.sleep(float((1000000 - x)/1000000))

def setVal():
    global ip
    global ACUID
    ip = str(ESPip.get())
    ACUID = str(AC_id.get())
    print("Values changed")


Window = Tk()
Window.title("IoT by iOSDev")
Window.configure(background="black")
action = [0,1,0,0,0,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,1]
ACState = False
b1 = 15 #time before AC starts
b2 = 5 #time before AC stops
rev = 0
ip = "172.20.10.4"
ACUID = "124680007"
coordinate = [600, 600]
now = datetime.datetime.now()

t1 = threading.Thread(target=ACAction)
t2 = threading.Thread(target=Review)
t3 = threading.Thread(target=ProgramTime)
t4 = threading.Thread(target=server)
##creating labels and button for GUI
Label(Window, text="IoT by iOSDev", bg="black", fg="green", font="none 20").grid(row=0, column=0, sticky=W)

Label(Window, text="Is user coming in next hour:", bg="black", fg="white", font="none 13").grid(row=3, rowspan=3,column=0, sticky=W)
userState = Text(Window, width=3, height=1, wrap=WORD, background="black", fg="white")
userState.grid(row=4, column=2, columnspan=2, sticky=E)

Label(Window, text="When you enter the room, how the temperature is?:", bg="black", fg="white", font="none 15").grid(row=9, rowspan=3,column=0, sticky=W)
buttonHot_b1 = Button(Window, text="HOT~~",bg="black",fg="red",command=b1_h).grid(row=17,column=0,sticky=E)
buttonNeutral_b1 = Button(Window, text="Neutral :)",bg="black",fg="yellow",command=rev0).grid(row=17,column=1,sticky=W)
buttonCool_b1 = Button(Window, text="COOL!!",bg="black",fg="blue",command=b1_c).grid(row=17,column=2,sticky=W)

Label(Window, text="When you exit the room, how the temperature is?:", bg="black", fg="white", font="none 15").grid(row=22, rowspan=3,column=0, sticky=W)
buttonHot_b2 = Button(Window, text="HOT~~",bg="black",fg="red",command=b2_h).grid(row=27,column=0,sticky=E)
buttonNeutral_b2 = Button(Window, text="Neutral :)",bg="black",fg="yellow",command=rev0).grid(row=27,column=1,sticky=W)
buttonCool_b2 = Button(Window, text="COOL!!",bg="black",fg="blue",command=b2_c).grid(row=27,column=2,sticky=W)

Label(Window, text="Do you enter here regularly?:", bg="black", fg="white", font="none 15").grid(row=50, rowspan=3,column=0, sticky=W)
buttonYes = Button(Window, text="Yes",bg="black",fg="blue",command=rev3_y).grid(row=50,column=0,sticky=E)
buttonNo = Button(Window, text="No",bg="black",fg="red",command=rev0).grid(row=50,column=1,sticky=W)

Label(Window, text="Enter ESP I.P. : ", bg="black", fg="yellow", font="none 10").grid(row=100,column=0,sticky=W)
ESPip = Entry(Window,width=15,bg="black",fg="gray")
ESPip.grid(row=100,column=1,sticky=W)

Label(Window, text="9 digit ID on which AC is connected : ", bg="black", fg="yellow", font="none 10").grid(row=120,column=0,sticky=W)
AC_id = Entry(Window,width=10,bg="black",fg="gray")
AC_id.grid(row=120,column=1,sticky=W)

setValues = Button(Window, text="Set Values",bg="gray",fg="black",command=setVal).grid(row=145,column=0,sticky=W)

t1.start()
t2.start()
t3.start()
t4.start()
Window.mainloop()
t1.join()
t2.join()
t3.join()
t4.join()