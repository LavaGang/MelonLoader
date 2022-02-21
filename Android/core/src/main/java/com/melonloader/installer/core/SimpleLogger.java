package com.melonloader.installer.core;

import com.melonloader.installer.core.ILogger;

public class SimpleLogger implements ILogger
{
    @Override
    public void Log(String msg) {
        System.out.println(msg);
    }
}