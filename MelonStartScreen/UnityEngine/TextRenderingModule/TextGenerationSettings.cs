using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class TextGenerationSettings : Il2CppObjectBase
    {
        private static readonly int classsize;

        private static readonly IntPtr f_font;
        private static readonly IntPtr f_color;
        private static readonly IntPtr f_fontSize;
        private static readonly IntPtr f_lineSpacing;
        private static readonly IntPtr f_richText;
        private static readonly IntPtr f_scaleFactor;
        private static readonly IntPtr f_fontStyle;
        private static readonly IntPtr f_textAnchor;
        //private static readonly IntPtr f_alignByGeometry;
        //private static readonly IntPtr f_resizeTextForBestFit;
        //private static readonly IntPtr f_resizeTextMinSize;
        //private static readonly IntPtr f_resizeTextMaxSize;
        //private static readonly IntPtr f_updateBounds;
        private static readonly IntPtr f_verticalOverflow;
        //private static readonly IntPtr f_horizontalOverflow;
        private static readonly IntPtr f_generationExtents;
        private static readonly IntPtr f_pivot;
        //private static readonly IntPtr f_generateOutOfBounds;

        static TextGenerationSettings()
        {
            Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "TextGenerationSettings");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr);
            uint align = 0;
            classsize = IL2CPP.il2cpp_class_value_size(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, ref align);


            f_font = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "font");
			f_color = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "color");
			f_fontSize = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "fontSize");
			f_lineSpacing = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "lineSpacing");
			f_richText = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "richText");
			f_scaleFactor = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "scaleFactor");
			f_fontStyle = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "fontStyle");
			f_textAnchor = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "textAnchor");
			//f_alignByGeometry = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "alignByGeometry");
			//f_resizeTextForBestFit = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextForBestFit");
			//f_resizeTextMinSize = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextMinSize");
			//f_resizeTextMaxSize = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextMaxSize");
			//f_updateBounds = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "updateBounds");
			f_verticalOverflow = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "verticalOverflow");
			//f_horizontalOverflow = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "horizontalOverflow");
			f_generationExtents = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "generationExtents");
			f_pivot = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "pivot");
			//f_generateOutOfBounds = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, "generateOutOfBounds");
		}

        public TextGenerationSettings(IntPtr ptr) : base(ptr) { }

        public unsafe TextGenerationSettings()
        {
            byte** data = stackalloc byte*[classsize];
            IntPtr pointer = IL2CPP.il2cpp_value_box(Il2CppClassPointerStore<TextGenerationSettings>.NativeClassPtr, (IntPtr)data);
            myGcHandle = IL2CPP.il2cpp_gchandle_new(pointer, false);
        }

        public unsafe Font font
        {
            get
            {
                IntPtr intPtr = *(IntPtr*)((uint)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_font));
                return (intPtr != IntPtr.Zero) ? new Font(intPtr) : null;
            }
            set => *(IntPtr*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_font)) = IL2CPP.Il2CppObjectBaseToPtr(value);
        }

        public unsafe Color color
        {
            get => *(Color*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_color));
            set => *(Color*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_color)) = value;
        }

        public unsafe int fontSize
        {
            get => *(int*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_fontSize));
            set => *(int*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_fontSize)) = value;
        }

        public unsafe float lineSpacing
        {
            get => *(float*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_lineSpacing));
            set => *(float*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_lineSpacing)) = value;
        }

        public unsafe bool richText
        {
            get => *(bool*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_richText));
            set => *(bool*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_richText)) = value;
        }

        public unsafe float scaleFactor
        {
            get => *(float*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_scaleFactor));
            set => *(float*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_scaleFactor)) = value;
        }

        public unsafe FontStyle fontStyle
        {
            get => *(FontStyle*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_fontStyle));
            set => *(FontStyle*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_fontStyle)) = value;
        }

        public unsafe TextAnchor textAnchor
        {
            get => *(TextAnchor*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_textAnchor));
            set => *(TextAnchor*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_textAnchor)) = value;
        }

        public unsafe VerticalWrapMode verticalOverflow
        {
            get => *(VerticalWrapMode*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_verticalOverflow));
            set => *(VerticalWrapMode*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_verticalOverflow)) = value;
        }

        public unsafe Vector2 generationExtents
        {
            get => *(Vector2*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_generationExtents));
            set => *(Vector2*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_generationExtents)) = value;
        }

        public unsafe Vector2 pivot
        {
            get => *(Vector2*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_pivot));
            set => *(Vector2*)((ulong)IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + IL2CPP.il2cpp_field_get_offset(f_pivot)) = value;
        }
    }
}
