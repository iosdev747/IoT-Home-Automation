import requests
from twython import TwythonStreamer
from auth import (
    consumer_key,
    consumer_secret,
    access_token,
    access_token_secret
)

Url = ''
#Url = 192.168.0.101
def sendURL(s):
    s = s.replace('RT @iOSDev747: ', '')
    if 'on' not in s: 
        state = '0'
        s = s.replace('off', '')
    else:
        state = '1'
        s = s.replace('on', '')
    try:
        r = requests.get('http://'+Url+'/'+state+'/'+s)
        if r.status_code == requests.code.ok:
            print('\nMessage sent sucessfully!!\n')
            print('Message:')
            print(r)        
        else:
            print('Error: Check your internet connection and hub')
    except:
        print('Exception occurs: Check your internet connection and hub')
        print('http://'+Url+'/'+state+'/'+s)

class MyStreamer(TwythonStreamer):
    print ('.'),
    def on_success(self, data):
        if 'text' in data:
            print(data['text'])
            sendURL(data['text'].replace('Hey raapberry ', ''))

def main():
    print('input')
    global Url
    Url = input()
    stream = MyStreamer(
        consumer_key,
        consumer_secret,
        access_token,
        access_token_secret
    )
    print('finding')
    stream.statuses.filter(track='Hey raapberry ')


main()
