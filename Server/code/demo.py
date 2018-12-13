# -*- coding: utf-8 -*-
"""
记录登录
"""
import dbctrl.saveobject
import timecontrol
import pubcore
import pubdefines
import pubglobalmanager
import json
import math
from threading import Timer

class CDemo(dbctrl.saveobject.CSaveData):
    def GetKey(self):
        return "loginrecord"

    def Init(self):
        super(CDemo, self).Init()
        self.CheckTimer()

    def CheckTimer(self):
        timecontrol.Remove_Call_Out("loginrecord")
        pubdefines.FormatPrint("定时器统计：目前总连接记录 %s" % self.m_Data.get("total", 0))
        timecontrol.Call_Out(pubcore.Functor(self.CheckTimer), 300, "loginrecord")

    def NewItem(self):
        self.m_Data.setdefault("total", 0)
        self.m_Data["total"] += 1
        self.Save()

    def CalPos(self, oClient, dData):
        r1 = dData["radius1"]
        r2 = dData["radius2"]
        lPos1 = dData["pos1"]
        lPos2 = dData["pos2"]
        iDicstacne = int(math.sqrt(pow(lPos1[0]-lPos2[0],2) + pow(lPos1[1]-lPos2[1],2)))
        dReturn = {
            "action": "show",
            "flag" : iDicstacne <= r1+r2,
        }
        oClient.Send(dReturn)
    
    def Login(self,m_uid,account,password):
        sValue=self.Query(account)
        dvalue=json.loads(sValue)
        score="-1"
        time="-1"
        socketMgr=pubglobalmanager.GetGlobalManager("socketmgr")
        #register success
        if sValue=="-1":     
            sendMsg="register success"
            socketMgr.GetItem(m_uid).SendMsg(sendMsg)
            value="{\"password\":\""+password+"\",\"score\":\""+score+"\",\"time\":\""+time+"\"}"
            print(value)
            self.Add(account,value)
            t=Timer(1,self.SendScore,[m_uid])
            t.start()
        #login success
        elif dvalue['password']==password:
            sendMsg="Login success"
            #print(sendMsg)
            #print(pubglobalmanager.GetGlobalManager("socketmgr").m_UID)
            socketMgr.GetItem(m_uid).SendMsg(sendMsg)
            t=Timer(1,self.SendScore,[m_uid])
            t.start()
        #password is wrong
        elif dvalue['password']!=password:
            sendMsg="Password is wrong"
            socketMgr.GetItem(m_uid).SendMsg(sendMsg)
            #print(dvalue['password'])   
        self.Save()

    def SaveScore(self,m_uid,account,score,time):
        sValue=self.Query(account)
        sendMsg=""
        if sValue=="-1":
            sendMsg="Save Error!"
        else:       
            dValue=json.loads(sValue)
            dValue['score']=score
            dValue['time']=time
            value=json.dumps(dValue)
            self.Add(account,value)
            sendMsg="Save Success!"
        socketMgr=pubglobalmanager.GetGlobalManager("socketmgr")
        socketMgr.GetItem(m_uid).SendMsg(sendMsg)
        print("save")
        self.Save()
    
    def SendScore(self,m_uid):
        dScore={}      
        for key,value in self.m_Data.items():          
            if key!="total":   
                dValue=json.loads(value)             
                score=dValue['score']
                dScore[key]=score
                #print(dScore[key]),key  
        #sorted by scorenum            
        sortdScore=sorted(dScore.items(),key=lambda dScore:dScore[1],reverse=True)  
        #get the valid data what the sender requires (score!=-1)   
        sendMsg=[]
        i=0     
        for tup in sortdScore:
            account=tup[0]
            score=tup[1]                       
            if score!="-1":
                value=self.m_Data[account]              
                dValue=json.loads(value)  
                msgRow="{\"account\":\""+account+"\",\"score\":\""+dValue['score']+"\",\"time\":\""+dValue['time']+"\"}"               
                sendMsg.append(msgRow)          
                i+=1
                if i==10:
                    break       
        #convert to json string,send to server
        sendString="["
        for sMsg in sendMsg:
            if sendMsg.index(sMsg)!=0:
                sendString+=','
            sendString+=sMsg
        sendString+=']'
        print(sendString)
        socketMgr=pubglobalmanager.GetGlobalManager("socketmgr")
        socketMgr.GetItem(m_uid).SendMsg(sendString)
        print("Send Score Success")
        

def Init():
    if pubglobalmanager.GetGlobalManager("demo"):
        return
    oManger = CDemo()
    pubglobalmanager.SetGlobalManager("demo", oManger)
    oManger.Init()

def Record():
    pubglobalmanager.CallManagerFunc("demo", "NewItem")

def OnCommand(oClient, dData):
    pubglobalmanager.CallManagerFunc("demo", "CalPos", oClient, dData)
        