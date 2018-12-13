# -*-  coding:utf-8 -*-

import pubglobalmanager
import pubdefines

from . import manager
from . import defines


def Init():
    if pubglobalmanager.GetGlobalManager(defines.TIME_CONTRL_MANAGER):
        return
    oManager = manager.CTimerManager()
    pubglobalmanager.SetGlobalManager(defines.TIME_CONTRL_MANAGER, oManager)


def Stop():
    pubglobalmanager.CallManagerFunc(defines.TIME_CONTRL_MANAGER, "Stop")


#定时回调，单位秒
def Call_Out(oFunc, iDelay, sKey):
    if not iDelay:
        pubdefines.FormatPrint("回调时间不能为0")
        return
    if not oFunc:
        return 
    pubglobalmanager.CallManagerFunc(defines.TIME_CONTRL_MANAGER, "Register", oFunc, iDelay, sKey)


def Remove_Call_Out(sKey):
    pubglobalmanager.CallManagerFunc(defines.TIME_CONTRL_MANAGER,"UnRegister", sKey)