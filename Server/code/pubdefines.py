# -*- coding:utf-8 -*-

import traceback
import sys
import os
import time

def PythonError(sMsg = ""):
    sErrTrace = traceback.format_exc()
    sLog = "---------TraceErr---------\n%s---------extra---------\n%s" % (sErrTrace, sMsg)
    ErrLog(sLog)

def TraceMsg():
    sTxtList = []
    sTxtList.append("")
    stack = traceback.extract_stack()
    for filename, linenum, fun, _ in stack[:-1]:
        txt = "File '%s', line %s ,in %s" % (filename, linenum, fun)
        sTxtList.append(txt)
    for sTxt in sTxtList:
        print sTxt.decode("utf-8").encode("gbk")

def ErrLog(txt):
    LogFile("err", txt)

def LogFile(sFileName, sTxt):
    sPrograPath = os.getcwd()
    iIndex = sFileName.rfind("/")
    if iIndex < 0 :
        sTempPath = ""
        sFile = sFileName
    else:
        sTempPath = sFileName[:iIndex]
        sFile = sFileName[iIndex:]
    sDir = "%s/log/%s" % (sPrograPath, sTempPath)
    if not os.path.exists(sDir):
        try:
            os.mkdir(sDir)
        except:
            pass
    sPath = "%s%s.txt" % (sDir, sFile)
    sTxt = "[%s] %s\n" % (TimeStr(), sTxt)
    with open(sPath, "a") as f:
        f.writelines(sTxt)

def TimeStr(ti=-1, sFormat="%Y-%m-%d %H:%M:%S"):
    if ti > 0:
        t = time.localtime(ti)
    else:
        t = time.localtime()
    sTime = time.strftime(sFormat, t)
    return sTime

def FormatPrint(sMsg):
    sMsg = "-------------------%s-------------------"%sMsg
    sMsg = sMsg.decode("utf-8").encode("gbk")
    print sMsg
