# -*- coding: utf-8 -*-

class Functor:
    def __init__(self, fn, *args, **kwargs):
        self._fn = fn
        self._args = args
        self._kwargs = kwargs
        self.m_Type = ""

    def __call__(self, *args, **kwargs):
        dKwargs = {}
        dKwargs.update(self._kwargs)
        dKwargs.update(kwargs)
        return self._fn(*(self._args + args),**kwargs)

    def Type(self):
        return self.m_Type

    def SetType(self, value):
        self.m_Type = value