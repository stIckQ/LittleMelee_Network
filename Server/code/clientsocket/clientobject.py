# -*- coding: utf-8 -*-
#client socket instance

import pubdefines
import threading
import pubglobalmanager
import struct
import json
import commandctrl

from . import defines

class CClientObj(threading.Thread):
    def __init__(self, uid, oSocket, ip, port):
        threading.Thread.__init__(self)
        self.m_ID = uid
        self.m_oSocket = oSocket
        self.m_IP = ip
        self.m_Port = port
        self.m_bConnected = True
        self.m_iMaxSize = 1024
        self.m_sData = ""
        self.InitObj()

    def InitObj(self):
        self.start()

    def run(self):
        while self.m_bConnected:
            try:
                sData = self.m_oSocket.recv(1024*1024)    
                if sData:
                    print(sData)
                    if sData=="CloseConnect":
                        self.Disconnected("Client Close Connect")
                        return 
                    dData=json.loads(sData)
                    account=dData['account']
                    password=dData['password']
                    #print(account),password
                    if "score" in dData:
                        print("sava score")
                        score=dData['score']
                        time=dData['time']
                        pubglobalmanager.CallManagerFunc(defines.DEMO,"SaveScore",self.m_ID,account,score,time)
                    else:
                        print("login")
                        pubglobalmanager.CallManagerFunc(defines.DEMO,"Login",self.m_ID,account,password)         
            except:
                self.Disconnected("recvfail")
                return
            try:
                self.Recv(sData)
            except:
                pubdefines.PythonError("recvfail2")

    def Recv(self, sData):
        self.m_sData += sData
        while True:
            iLen = len(self.m_sData)
            if iLen < 4:    #包基本格式不齐
                return
            sSize = self.m_sData[:4]          
            iSize = struct.unpack("i", sSize)[0]
            if iSize > iLen:    #未发完，继续等待
                return
            sPack = self.m_sData[4:iSize]
            self.m_sData = self.m_sData[iSize:]
            dPackData = json.loads(sPack, encoding="gbk")
            dPackData = self.TrasnData(dPackData)
            self.OnCommand(dPackData) 

    def TrasnData(self, obj):
        if isinstance(obj, dict):
            res = {}
            for key, value in obj.iteritems():
                tempkey = self.TrasnData(key)
                tempvalue = self.TrasnData(value)
                res[tempkey] = tempvalue
            pubdefines.FormatPrint("dict")
            return res
        elif isinstance(obj, list):
            res = []
            for value in obj:
                res.append(self.TrasnData(value))
            pubdefines.FormatPrint("list")
            return res
        elif isinstance(obj, unicode):
            pubdefines.FormatPrint("unicode")
            return obj.encode("utf-8")
        else:
            pubdefines.FormatPrint("other")
            return obj


    def Send(self, dData):
        sData = json.dumps(dData)
        iSize = len(sData)
        iTotalSize = iSize + 4
        sSize = struct.pack("i", iTotalSize)
        sTotalStr = sSize + sData

        iCount = (iTotalSize - 1)/self.m_iMaxSize +1 
        for i in range(iCount):
            iBegin = self.m_iMaxSize * i
            iEnd = iBegin + self.m_iMaxSize
            sSubSend = sTotalStr[iBegin:iEnd]
            try:
                self.m_oSocket.send(sSubSend)
            except:
                self.Disconnected("sendfail")
                return
    
    def SendMsg(self,msg):
        try:
            self.m_oSocket.send(msg)
        except:
            self.Disconnected("sendfail")
            return

    def Release(self):
        try:
            self.m_bConnected = False
            self.m_oSocket.close()
        except:
            pubdefines.PythonError()

    def Disconnected(self, sReason):
        pubglobalmanager.CallManagerFunc(defines.SOCKET_MANAGER, "Disconnected", self.m_ID, sReason)

    def OnCommand(self, dData):
        commandctrl.OnCommand(self, dData)