using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class TextGenerationSettings : InternalObjectBase
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
            InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "TextGenerationSettings");
            //UnityInternals.runtime_class_init(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr);

            uint align = 0;
            classsize = UnityInternals.class_value_size(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, ref align);


            f_font = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "font");
			f_color = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "color");
			f_fontSize = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "fontSize");
			f_lineSpacing = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "lineSpacing");
			f_richText = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "richText");
			f_scaleFactor = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "scaleFactor");
			f_fontStyle = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "fontStyle");
			f_textAnchor = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "textAnchor");
			//f_alignByGeometry = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "alignByGeometry");
			//f_resizeTextForBestFit = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextForBestFit");
			//f_resizeTextMinSize = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextMinSize");
			//f_resizeTextMaxSize = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "resizeTextMaxSize");
			//f_updateBounds = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "updateBounds");
			f_verticalOverflow = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "verticalOverflow");
			//f_horizontalOverflow = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "horizontalOverflow");
			f_generationExtents = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "generationExtents");
			f_pivot = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "pivot");
			//f_generateOutOfBounds = UnityInternals.GetField(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, "generateOutOfBounds");
		}

        public TextGenerationSettings(IntPtr ptr) : base(ptr) { }

        public unsafe TextGenerationSettings()
        {
            byte** data = stackalloc byte*[classsize];
            IntPtr pointer = UnityInternals.value_box(InternalClassPointerStore<TextGenerationSettings>.NativeClassPtr, (IntPtr)data);
            myGcHandle = UnityInternals.gchandle_new(pointer, false);
        }

        public unsafe Font font
        {
            get
            {
                IntPtr intPtr = *(IntPtr*)((uint)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_font));
                return (intPtr != IntPtr.Zero) ? new Font(intPtr) : null;
            }
            set => *(IntPtr*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_font)) = UnityInternals.ObjectBaseToPtr(value);
        }

        public unsafe Color color
        {
            get => *(Color*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_color));
            set => *(Color*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_color)) = value;
        }

        public unsafe int fontSize
        {
            get => *(int*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_fontSize));
            set => *(int*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_fontSize)) = value;
        }

        public unsafe float lineSpacing
        {
            get => *(float*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_lineSpacing));
            set => *(float*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_lineSpacing)) = value;
        }

        public unsafe bool richText
        {
            get => *(bool*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_richText));
            set => *(bool*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_richText)) = value;
        }

        public unsafe float scaleFactor
        {
            get => *(float*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_scaleFactor));
            set => *(float*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_scaleFactor)) = value;
        }

        public unsafe FontStyle fontStyle
        {
            get => *(FontStyle*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_fontStyle));
            set => *(FontStyle*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_fontStyle)) = value;
        }

        public unsafe TextAnchor textAnchor
        {
            get => *(TextAnchor*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_textAnchor));
            set => *(TextAnchor*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_textAnchor)) = value;
        }

        public unsafe VerticalWrapMode verticalOverflow
        {
            get => *(VerticalWrapMode*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_verticalOverflow));
            set => *(VerticalWrapMode*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_verticalOverflow)) = value;
        }

        public unsafe Vector2 generationExtents
        {
            get => *(Vector2*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_generationExtents));
            set => *(Vector2*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_generationExtents)) = value;
        }

        public unsafe Vector2 pivot
        {
            get => *(Vector2*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_pivot));
            set => *(Vector2*)((ulong)UnityInternals.ObjectBaseToPtrNotNull(this) + UnityInternals.field_get_offset(f_pivot)) = value;
        }
    }
}
