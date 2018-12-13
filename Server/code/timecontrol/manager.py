#-*- coding:utf-8 -*-
import threading
import time

class CTimerManager(threading.Thread):
    def __init__(self):
        threading.Thread.__init__(self)
        self.m_dTimerDict = {}
        self.m_bStart = True
        self.start()

    def Stop(self):
        self.m_bStart = False
    
    def run(self):
        while self.m_bStart:
            time.sleep(0.1)
            self.CheckTimeOut()

    def Register(self, oFunc, iDelay, sKey):
        iEndTime = time.time() + iDelay
        self.m_dTimerDict[sKey] = (iEndTime, oFunc)

    def UnRegister(self, sKey):
        if sKey in self.m_dTimerDict:
            del self.m_dTimerDict[sKey]

    
    def CheckTimeOut(self):
        iCurTime = time.time()
        sKeyList = []
        for sKey, tInfo in self.m_dTimerDict.items():
            iEndTime, _ = tInfo
            if iEndTime > iCurTime:
                continue
            sKeyList.append(sKey)

        for sKey in sKeyList:
            tInfo = self.m_dTimerDict.pop(sKey)
            oFunc = tInfo[1]
            oFunc()
            
