# -*- coding: utf-8 -*-

import marshal
import pubglobalmanager
import pubdefines

from . import defines

class CSaveData(object):
    def __init__(self):
        self.m_Data = {}
        self.m_Loaded = True

    def Query(self, sAttr, default = "-1"):
        return self.m_Data.get(sAttr, default)

    def Add(self, sAttr, iVal):
        #self.m_Data.setdefault(sAttr, "-1")  
        self.m_Data[sAttr] = iVal

    def Delete(self, sAttr):
        if sAttr in self.m_Data:
            del self.m_Data[sAttr]

    def GetKey(self):
        raise Exception("未实现Key")

    def GetCreateSql(self):
        sData = marshal.dumps(self.m_Data)
        sSql = "insert into tbl_global(rl_sName, rl_dmSaveTime, rl_sData) values('%s', now(), '%s')" % (self.GetKey(), sData)
        return sSql

    def GetUpdateSql(self):
        sData = marshal.dumps(self.m_Data)
        sSql = "update tbl_global set rl_sData='%s',rl_dmSaveTime=now() where rl_sName='%s'" % (sData, self.GetKey())
        return sSql

    def GetQuerySql(self):
        sSql = "select rl_sData from tbl_global where rl_sName ='%s'" % self.GetKey()
        return sSql

    def Init(self):
        if self.Load():
            return
        self.New()

    def Load(self):
        #self.Save();
        sSql = self.GetQuerySql()
        resultList = pubglobalmanager.CallManagerFunc(defines.DBCTRL_MANAGER_NAME, "Query", sSql)
        
        if resultList:
            sData = resultList[0][0]
            self.m_Data = marshal.loads(sData)
            return True
        return False

    def New(self):
        sSql = self.GetCreateSql()
        pubglobalmanager.CallManagerFunc(defines.DBCTRL_MANAGER_NAME, "ExecSql", sSql)

    def Save(self):
        sSql = self.GetUpdateSql()
        pubglobalmanager.CallManagerFunc(defines.DBCTRL_MANAGER_NAME, "ExecSql", sSql)
