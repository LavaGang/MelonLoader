package com.melonloader.exceptions;

public class MissingContext extends MelonException
{
    public MissingContext(String err)
    {
        super(err + ". Missing application context.");
    }
}