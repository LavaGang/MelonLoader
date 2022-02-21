package com.melonloader.installer.core.steps;

import com.melonloader.installer.core.ILogger;
import com.melonloader.installer.core.InstallerStep;
import com.melonloader.installer.core.PathDefinitions;
import com.melonloader.installer.core.Properties;
import lanchon.dexpatcher.Configuration;
import lanchon.dexpatcher.Processor;
import lanchon.dexpatcher.core.Context;
import lanchon.dexpatcher.core.logger.BasicLogger;
import lanchon.dexpatcher.core.logger.Logger;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;

public class Step__10__PatchDex extends InstallerStep {
    public boolean Run() throws IOException {
        properties.logger.Log("Patching dex");

        ManagedLogger logger = new ManagedLogger(properties.logger);

        return Processor.processFiles(logger, new Configuration() {{
            sourceFile = paths.dexOriginal.toString();
            patchFiles = Arrays.asList(new String[]{paths.dexPatch.toString()});
            multiDex = true;
            annotationPackage = Context.DEFAULT_ANNOTATION_PACKAGE;
            constructorAutoIgnoreDisabled = false;
            patchedFile = paths.dexOutput.toString();
            logLevel = Logger.Level.DEBUG;
            timingStats = true;
            mapSource = false;
        }});
    }

    class ManagedLogger extends Logger
    {
        private ILogger melonLogger;

        public ManagedLogger(ILogger logger)
        {
            melonLogger = logger;
        }

        @Override
        protected void doLog(Level level, String message, Throwable throwable) {
            melonLogger.Log("[DexPatcher] [" + level.toString() + "] " + message);
        }

        @Override
        public void flush() {
            melonLogger.Log("--------");
        }

        @Override
        public void close() {

        }
    }
}
