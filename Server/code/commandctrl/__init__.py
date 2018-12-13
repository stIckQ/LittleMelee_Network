# -*- coding: utf-8 -*-
import pubglobalmanager

from . import manager
from . import defines

def Init():
    if pubglobalmanager.GetGlobalManager(defines.COMMAND_CTRL_NAME):
        return
    oManager = manager.CCommandManager()
    pubglobalmanager.SetGlobalManager(defines.COMMAND_CTRL_NAME, oManager)

def OnCommand(oClient, dData):
    oManager = pubglobalmanager.GetGlobalManager(defines.COMMAND_CTRL_NAME)
    oManager.OnCommand(oClient, dData)