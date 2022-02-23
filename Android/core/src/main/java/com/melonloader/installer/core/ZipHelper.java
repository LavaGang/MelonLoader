package com.melonloader.installer.core;

import net.lingala.zip4j.model.FileHeader;
import net.lingala.zip4j.model.UnzipParameters;
import net.lingala.zip4j.model.ZipParameters;
import net.lingala.zip4j.tasks.AddFolderToZipTask;
import org.apache.commons.io.FileUtils;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.List;

public class ZipHelper {
    private static final int BUFFER_SIZE = 4096;
    private String path;
    private HashMap<String, String> extractMap = new HashMap<>();
    private List<String> deleteMap = new ArrayList<>();
    private HashMap<String, String> addMap = new HashMap<>();

    public ZipHelper(String _path)
    {
        path = _path;
    }

    public List<String> GetFiles() throws IOException {
        List<String> entries = new ArrayList<>();

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        for (FileHeader fileHeader : zip.getFileHeaders()) {
            entries.add(fileHeader.getFileName());
        }

        zip.close();

        return entries;
    }

    public void QueueExtract(String zipPath, String outputPath)
    {
        extractMap.put(zipPath, outputPath);
    }
    public void QueueWrite(String path, String zipPath) { addMap.put(path, zipPath); }

    public void QueueDelete(String zipPath) { deleteMap.add(zipPath); }

    public void Delete() throws IOException {
        if (deleteMap.isEmpty())
            return;

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        for (String fileName : deleteMap) {
            Main.GetProperties().logger.Log("Removing: " + fileName);
        }

        zip.removeFiles(deleteMap);

        zip.close();
        deleteMap.clear();
    }
    
    public void Write() throws IOException {
        if (addMap.isEmpty())
            return;

        String tempBuildDir = path + ".temp";

        if (Files.exists(Paths.get(tempBuildDir)))
            FileUtils.deleteDirectory(new File(tempBuildDir));

        Files.createDirectories(Paths.get(tempBuildDir));

        for (String fileName : addMap.keySet()) {
            Main.GetProperties().logger.Log("Adding: " + fileName + " -> " + addMap.get(fileName));
            Path fileTempPath = Paths.get(tempBuildDir, addMap.get(fileName));
            Path folder = fileTempPath.getParent();

            Files.createDirectories(folder);

            Files.move(Paths.get(fileName), fileTempPath, StandardCopyOption.REPLACE_EXISTING);
        }

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        ZipParameters params = new ZipParameters();
        params.setDefaultFolderPath("");

        List<File> filesToAdd = new ArrayList<>();

        for (String s : (new File(tempBuildDir)).list()) {
            File subfile = new File(Paths.get(tempBuildDir, s).toString());
            if (subfile.isDirectory()) {
                Main.GetProperties().logger.Log("adding: " + subfile.getAbsolutePath());
                zip.addFolder(subfile, params);
            }
            else {
                filesToAdd.add(subfile);
            }
        }

        Main.GetProperties().logger.Log("writing base files");
        zip.addFiles(filesToAdd);

        zip.close();
        addMap.clear();

        FileUtils.deleteDirectory(new File(tempBuildDir));
    }

    public void Extract() throws IOException { Extract(false); }

    public void Extract(boolean checked) throws IOException {
        if (extractMap.isEmpty())
            return;

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        for (String fileName : extractMap.keySet()) {
            Main.GetProperties().logger.Log("Extracting: " + fileName + " -> " + extractMap.get(fileName));

            String rawPath = extractMap.get(fileName);
            String folderOut = new File(rawPath).getParent().toString();
            String fileOut = new File(rawPath).getName();

            if (!checked)
                zip.extractFile(fileName, folderOut, fileOut);
            else
                try { zip.extractFile(fileName, folderOut, fileOut); } catch (Exception e) { e.printStackTrace(); }
        }

        zip.close();
        extractMap.clear();
    }

    public static String GetBaseName(String zipPath)
    {
        String[] splitPaths = zipPath.split("/");
        String baseName = splitPaths[splitPaths.length - 1];
        return baseName;
    }
}
