package com.melonloader.installer.core;

import java.io.BufferedOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;

public class LogOutputStream extends OutputStream {
    protected ILogger customLogger;

    protected byte[] buf;
    protected int count;

    public LogOutputStream(ILogger var1) {
        this(var1, 8192);
    }

    public LogOutputStream(ILogger var1, int var2) {
        this.customLogger = var1;

        if (var2 <= 0) {
            throw new IllegalArgumentException("Buffer size <= 0");
        } else {
            this.buf = new byte[var2];
        }
    }

    protected void flushBuffer() throws IOException {
        if (this.count > 0) {
            this.customLogger.Log(new String(this.buf, 0, count));
            this.count = 0;
        }
    }

    public synchronized void write(int var1) throws IOException {
        if (var1 == '\n') {
            this.flushBuffer();
            return;
        }

        if (this.count >= this.buf.length) {
            this.flushBuffer();
        }

        this.buf[this.count++] = (byte)var1;
    }

    public synchronized void flush() throws IOException {
        this.flushBuffer();
    }
}
