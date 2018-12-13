# -*- coding: utf-8 -*-
import  threading

class CMutexLockManager(object):
    def __init__(self):
        self.m_dLockItem = {}

    def Create(self, sName):
        if sName in self.m_dLockItem:
            return 
        oLock = threading.Lock()
        self.m_dLockItem[sName] = oLock

    def Acquire(self, sName):
        if not sName in self.m_dLockItem:
            self.Create(sName)
        oLock = self.m_dLockItem[sName]
        oLock.acquire()

    def Release(self, sName):
        if not sName in self.m_dLockItem:
            return
        oLock = self.m_dLockItem[sName]
        oLock.release()