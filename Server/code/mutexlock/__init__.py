# -*- coding: utf-8 -*-
import pubglobalmanager

from . import manager
from . import defines

def Init():
    if pubglobalmanager.GetGlobalManager(defines.MUTEXLOCK_NAME):
        return
    oManager = manager.CMutexLockManager()
    pubglobalmanager.SetGlobalManager(defines.MUTEXLOCK_NAME, oManager)

def AutoLock(sName = "mutex"):
    def _AutoLock(func):
        def ___AutoLock(*args, **kwargs):
            oMgr = pubglobalmanager.GetGlobalManager(defines.MUTEXLOCK_NAME)
            oMgr.Acquire(sName)
            ret = func(*args, **kwargs)
            oMgr.Release(sName)
            return ret
        return ___AutoLock
    return _AutoLock