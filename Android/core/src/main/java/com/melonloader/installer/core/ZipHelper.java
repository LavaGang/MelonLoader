package com.melonloader.installer.core;

import net.lingala.zip4j.model.FileHeader;
import net.lingala.zip4j.model.ZipParameters;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
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

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        for (String fileName : addMap.keySet()) {
            File file = new File(fileName);

            Main.GetProperties().logger.Log("Adding: " + fileName + " -> " + addMap.get(fileName));

            zip.addFile(file);
            zip.renameFile(file.getName(), addMap.get(fileName));
        }
        
        zip.close();
        addMap.clear();
    }

    public void Extract() throws IOException { Extract(false); }

    public void Extract(boolean createFolder) throws IOException {
        if (addMap.isEmpty())
            return;

        net.lingala.zip4j.ZipFile zip = new net.lingala.zip4j.ZipFile(path);

        for (String fileName : extractMap.keySet()) {
            Main.GetProperties().logger.Log("Extracting: " + fileName + " -> " + addMap.get(fileName));

            zip.extractFile(fileName, extractMap.get(fileName));
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
