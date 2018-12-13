# -*- coding: utf-8 -*-

import pubglobalmanager

from . import manager
from . import defines

def Init():
    if pubglobalmanager.GetGlobalManager(defines.DBCTRL_MANAGER_NAME):
        return
    oManager = manager.CDBManager()
    pubglobalmanager.SetGlobalManager(defines.DBCTRL_MANAGER_NAME, oManager)