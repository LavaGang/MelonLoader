﻿using MelonUnityEngine;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal class UI_Text : UI_Object
    {
        private UIConfig.TextSettings config;
        private Mesh mesh;
        internal bool isDirty = true;
        internal string text;

        internal UI_Text(UIConfig.TextSettings textSettings)
        {
            config = textSettings;
            text = config.Text;
            AllElements.Add(this);
        }

        internal override void Render()
            => Render(config.Position.Item1, config.Position.Item2);
        internal void Render(int x, int y)
        {
            if (!config.Enabled)
                return;

            UpdateMesh();
            UIStyleValues.TextFont.material.SetPass(0);
            if (mesh == null)
                return;

            UIUtils.AnchorToScreen(config.ScreenAnchor, x, y, out int anchor_x, out int anchor_y);
            Graphics.DrawMeshNow(mesh, new Vector3(anchor_x, anchor_y, 0), Quaternion.identity);
        }

        private void UpdateMesh()
        {
            if (!isDirty)
                return;
            if (string.IsNullOrEmpty(text))
                return;

            if (mesh != null)
            {
                mesh.DestroyImmediate();
                mesh = null;
            }

            TextGenerationSettings settings = new TextGenerationSettings();
            settings.generationExtents = new Vector2(540, 47.5f);
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.verticalOverflow = VerticalWrapMode.Overflow;

            settings.font = UIStyleValues.TextFont; // To-Do: Font Customization

            settings.textAnchor = (TextAnchor)config.Anchor;
            settings.color = config.TextColor;
            settings.richText = config.RichText;
            settings.fontSize = config.TextSize;
            settings.fontStyle = config.Style;
            settings.scaleFactor = config.Scale;
            settings.lineSpacing = config.LineSpacing;

            string displayText = text;
            displayText = displayText.Replace("<loaderName/>", (MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON) ? "LemonLoader" : "MelonLoader");
            displayText = displayText.Replace("<loaderVersion/>", BuildInfo.Version);
            displayText = displayText.Replace("LemonLoader", "<color=#FFCC4D>LemonLoader</color>");
            displayText = displayText.Replace("MelonLoader", "<color=#78f764>Melon</color><color=#ff3c6a>Loader</color>");

            mesh = TextMeshGenerator.Generate(displayText, settings);
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.DontDestroyOnLoad();

            isDirty = false;
        }

        internal override void Dispose()
        {
            if (mesh == null)
                return;

            mesh.DestroyImmediate();
            mesh = null;
        }
    }
}
