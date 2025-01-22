using System;

namespace MelonLoader {
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonAdditionalCreditsAttribute : Attribute {
        /// <summary>
        /// Any additional credits that the mod author might want to include
        /// </summary>
        public string Credits { get; internal set; }

        /// <summary>
        /// AdditionalCredits constructor
        /// </summary>
        /// <param name="credits">The additional credits of the mod</param>
        public MelonAdditionalCreditsAttribute(string credits) {
            Credits = credits;
        }
    }
}
