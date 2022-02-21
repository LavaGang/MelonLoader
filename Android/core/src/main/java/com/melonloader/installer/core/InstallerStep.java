package com.melonloader.installer.core;

import java.io.IOException;

public abstract class InstallerStep {
    protected Properties properties;
    protected PathDefinitions paths;

    public void SetProps(Properties properties, PathDefinitions paths)
    {
        this.properties = properties;
        this.paths = paths;
    }

    public abstract boolean Run() throws Exception;
}
