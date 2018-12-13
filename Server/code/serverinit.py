# -*- coding: utf-8 -*-

import clientsocket
import timecontrol
import threading
import pubdefines
import commandctrl
import mutexlock
import dbctrl
import demo
import os

def InitModule():
    InitLogDir()
    clientsocket.Init()
    timecontrol.Init()
    commandctrl.Init()
    mutexlock.Init()
    dbctrl.Init()
    demo.Init()

def StartGame():
    pubdefines.FormatPrint("启动服务器")
    LoadServerConsole()
    clientsocket.Start()

def Main():
    InitModule()
    StartGame()

def LoadServerConsole():
    oFunc = threading.Thread(target=ServerConsoleStart)
    oFunc.start()

def ServerConsoleStart():
    while True:
        sInput = raw_input("")
        try:
            exec(sInput)
        except Exception as e:
            print e

def InitLogDir():
    try:
        os.mkdir("log")
    except:
        pubdefines.FormatPrint("log目录已经存在")

if __name__ == "__main__":
    Main()