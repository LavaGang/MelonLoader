package com.melonloader.bridge;

public class Assertion {
    public static native boolean getShouldContinue();
    public static native boolean getDontDie();
    public static native void ThrowInternalFailure(String msg);
}
