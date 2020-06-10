using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public interface ModSettingsMenu_RenderHelper
    {

    }

    class ModSettingsMenu
    {
        private static ModSettingsMenu_RenderHelper RenderHelper = null;

        internal static void Setup()
        {
            RenderHelper = SupportModule.GetModSettingsMenuRenderHelper();
            if (RenderHelper == null)
                throw new NotSupportedException("Support module must be initialized before Rendering Mod Settings");
        }

        internal static void Render()
        {
            if (RenderHelper == null)
                return;


        }
    }
}
