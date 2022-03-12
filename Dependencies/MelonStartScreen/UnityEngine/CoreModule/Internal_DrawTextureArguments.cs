using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonUnityEngine
{
    // 2017.2.0
    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawTextureArguments_2017
    {
        public Rect screenRect;
        public Rect sourceRect;
        public int leftBorder;
        public int rightBorder;
        public int topBorder;
        public int bottomBorder;
        public Color32 color;
        public Vector4 borderWidths;
        public float cornerRadius;
        public int pass;
        public IntPtr texture;
        public IntPtr mat;
    }

    // 2017.3.0, 2018.1.0
    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawTextureArguments_2018
    {
        public Rect screenRect;
        public Rect sourceRect;
        public int leftBorder;
        public int rightBorder;
        public int topBorder;
        public int bottomBorder;
        public Color32 color;
        public Vector4 borderWidths;
        public Vector4 cornerRadius; // Int32 -> Vector4
        public int pass;
        public IntPtr texture;
        public IntPtr mat;
    }

    // 2018.2.0, 2019.1.0
    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawTextureArguments_2019
    {
        public Rect screenRect;
        public Rect sourceRect;
        public int leftBorder;
        public int rightBorder;
        public int topBorder;
        public int bottomBorder;
        public Color color; // Color32 -> Color
        public Vector4 borderWidths;
        public Vector4 cornerRadius;
        public int pass;
        public IntPtr texture;
        public IntPtr mat;
    }

    // 2019.3.0, 2020.1.0
    [StructLayout(LayoutKind.Sequential)]
    internal struct Internal_DrawTextureArguments_2020
    {
        public Rect screenRect;
        public Rect sourceRect;
        public int leftBorder;
        public int rightBorder;
        public int topBorder;
        public int bottomBorder;
        public Color leftBorderColor; // Added
        public Color rightBorderColor; // Added
        public Color topBorderColor; // Added
        public Color bottomBorderColor; // Added
        public Color color;
        public Vector4 borderWidths;
        public Vector4 cornerRadiuses;
        public bool smoothCorners; // Added
        public int pass;
        public IntPtr texture;
        public IntPtr mat;
    }
}
