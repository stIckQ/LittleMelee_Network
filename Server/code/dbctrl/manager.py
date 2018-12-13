# -*- coding:utf-8 -*-

import MySQLdb
import pubdefines
from . import defines

class CDBManager(object):
    def __init__(self):
        self.m_iPort = 3306
        self.m_sIP = "127.0.0.1"
        self.m_sUser =  "root"
        self.m_sPassword = "123456"
        self.m_ConnectList = []
        self.Init()
    
    def LogFile(self, sMsg):
        pubdefines.LogFile("dbmanager", sMsg)

    def Init(self):
        self.CheckDataBase()

    def CheckDataBase(self):
        sSql = "USE %s" % defines.DATABASE_NAME
        iErrNum =self.ExecSql(sSql)
        if iErrNum == 1049:
            self.LogFile("创建数据库%s" % defines.DATABASE_NAME)
            self.ExecSql("CREATE DATABASE %s" % defines.DATABASE_NAME)
            self.ExecSql(sSql)
        self.CheckTables()

    def CheckTables(self):
        sSql = "SHOW TABLES"
        resultList = self.Query(sSql)
        tableList = []
        for tInfo in resultList:
            tableList.append(tInfo[0])
        for sTableName, sTableSql in defines.TABLE_ALL.iteritems():
            if sTableName in tableList:
                continue
            self.LogFile("创建数据表%s" % sTableName)
            self.ExecSql(sTableSql)

    def GetConnect(self):
        if self.m_ConnectList:
            oConnect = self.m_ConnectList.pop(0)
        else:
            oConnect = MySQLdb.connect(host=self.m_sIP, port=self.m_iPort, user=self.m_sUser, passwd=self.m_sPassword)
            sSql = "USE %s" % defines.DATABASE_NAME
            cursor = oConnect.cursor()
            try:
                cursor.execute(sSql)
            except:
                cursor.close()
        return oConnect

    def ReleaseConnect(self, oConnect):
        self.m_ConnectList.append(oConnect)

    def DBErr(self, e):
        iErrNum = e.args[0]
        sErrMsg = e.args[1]
        pubdefines.LogFile("dbmanager", "%s" % sErrMsg)
        return iErrNum

    def ExecSql(self, sSql):
        oConnect = self.GetConnect()
        cursor = oConnect.cursor()
        iErrNum = 0
        try:
            cursor.execute(sSql)
            oConnect.commit()
        except MySQLdb.Error as e:
            iErrNum = self.DBErr(e)
        cursor.close()
        self.ReleaseConnect(oConnect)
        return iErrNum

    def Query(self, sSql):
        oConnect = self.GetConnect()
        cursor = oConnect.cursor()
        cursor.execute(sSql)
        resultList = cursor.fetchall()
        cursor.close()
        self.ReleaseConnect(oConnect)
        return resultList

