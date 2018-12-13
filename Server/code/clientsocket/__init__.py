# -*- coding: utf-8 -*- 
import pubglobalmanager

from . import manager
from . import defines

def Init():
    if pubglobalmanager.GetGlobalManager(defines.SOCKET_MANAGER):
        return
    oManager = manager.CSocketManger()
    pubglobalmanager.SetGlobalManager(defines.SOCKET_MANAGER, oManager)


def Start():
    oManager = pubglobalmanager.GetGlobalManager(defines.SOCKET_MANAGER)
    oManager.Start()