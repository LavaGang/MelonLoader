using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader.InternalUtils
{
    /*
     * A list of Unity's built-in messages / magic methods and their (optional) argument types.
     * Used to prevent false-positive "IL2CPP method got inlined, patch may not work" warnings.
     * Put in its own static class to prevent unnecessary initialization for Mono games.
     */
    public static class UnityMagicMethods
    {

        private static Type MonoBehaviourType = null;
        private static Type ScriptableObjectType = null;

        // Key = name of magic method. Value = optional argument types
        // See: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
        private static Dictionary<string, Type[]> MonoBehaviourMethods = null;
        // See: https://docs.unity3d.com/ScriptReference/ScriptableObject.html
        private static Dictionary<string, Type[]> ScriptableObjectMethods = null;

        static UnityMagicMethods()
        {
            Assembly unityAssembly = Assembly.Load("UnityEngine.CoreModule") ?? Assembly.Load("UnityEngine");
            Type unityType(string name) // This may return null - especially for types such as NetworkPlayer that have been removed in newer Unity versions
                => unityAssembly.GetType(name);

            MonoBehaviourType = unityType("UnityEngine.MonoBehaviour");
            ScriptableObjectType = unityType("UnityEngine.ScriptableObject");

            MonoBehaviourMethods = new Dictionary<string, Type[]>
            {
                ["Awake"] = new Type[0],
                ["FixedUpdate"] = new Type[0],
                ["LateUpdate"] = new Type[0],
                ["OnAnimatorIK"] = new[] { typeof(int) },
                ["OnAnimatorMove"] = new Type[0],
                ["OnApplicationFocus"] = new[] { typeof(bool) },
                ["OnApplicationPause"] = new[] { typeof(bool) },
                ["OnApplicationQuit"] = new Type[0],
                ["OnAudioFilterRead"] = new[] { typeof(float[]), typeof(int) },
                ["OnBecameInvisible"] = new Type[0],
                ["OnBecameVisible"] = new Type[0],
                ["OnCollisionEnter"] = new[] { unityType("UnityEngine.Collision") },
                ["OnCollisionEnter2D"] = new[] { unityType("UnityEngine.Collision2D") },
                ["OnCollisionExit"] = new[] { unityType("UnityEngine.Collision") },
                ["OnCollisionExit2D"] = new[] { unityType("UnityEngine.Collision2D") },
                ["OnCollisionStay"] = new[] { unityType("UnityEngine.Collision") },
                ["OnCollisionStay2D"] = new[] { unityType("UnityEngine.Collision2D") },
                ["OnConnectedToServer"] = new Type[0],
                ["OnControllerColliderHit"] = new[] { unityType("UnityEngine.ControllerColliderHit") },
                ["OnDestroy"] = new Type[0],
                ["OnDisable"] = new Type[0],
                ["OnDisconnectedFromServer"] = new[] { unityType("UnityEngine.NetworkDisconnection") },
                // "OnDrawGizmos" is editor-only
                // "OnDrawGizmosSelected" is editor-only
                ["OnEnable"] = new Type[0],
                ["OnFailedToConnect"] = new[] { unityType("UnityEngine.NetworkConnectionError") },
                ["OnFailedToConnectToMasterServer"] = new[] { unityType("UnityEngine.NetworkConnectionError") },
                ["OnGUI"] = new Type[0],
                ["OnJointBreak"] = new[] { typeof(float) },
                ["OnJointBreak2D"] = new[] { unityType("UnityEngine.Joint2D") },
                ["OnMasterServerEvent"] = new[] { unityType("UnityEngine.MasterServerEvent") },
                ["OnMouseDown"] = new Type[0],
                ["OnMouseDrag"] = new Type[0],
                ["OnMouseEnter"] = new Type[0],
                ["OnMouseExit"] = new Type[0],
                ["OnMouseOver"] = new Type[0],
                ["OnMouseUp"] = new Type[0],
                ["OnMouseUpAsButton"] = new Type[0],
                ["OnNetworkInstantiate"] = new[] { unityType("UnityEngine.NetworkMessageInfo") },
                ["OnParticleCollision"] = new[] { unityType("UnityEngine.GameObject") },
                ["OnParticleSystemStopped"] = new Type[0],
                ["OnParticleTrigger"] = new Type[0],
                ["OnParticleUpdateJobScheduled"] = new Type[0],
                ["OnPlayerConnected"] = new[] { unityType("UnityEngine.NetworkPlayer") },
                ["OnPlayerDisconnected"] = new[] { unityType("UnityEngine.NetworkPlayer") },
                ["OnPostRender"] = new Type[0],
                ["OnPreCull"] = new Type[0],
                ["OnPreRender"] = new Type[0],
                ["OnRenderImage"] = new[] { unityType("UnityEngine.RenderTexture"), unityType("UnityEngine.RenderTexture") },
                ["OnRenderObject"] = new Type[0],
                ["OnSerializeNetworkView"] = new[] { unityType("UnityEngine.BitStream"), unityType("UnityEngine.NetworkMessageInfo") },
                ["OnServerInitialized"] = new Type[0],
                ["OnTransformChildrenChanged"] = new Type[0],
                ["OnTransformParentChanged"] = new Type[0],
                ["OnTriggerEnter"] = new[] { unityType("UnityEngine.Collider") },
                ["OnTriggerEnter2D"] = new[] { unityType("UnityEngine.Collider2D") },
                ["OnTriggerExit"] = new[] { unityType("UnityEngine.Collider") },
                ["OnTriggerExit2D"] = new[] { unityType("UnityEngine.Collider2D") },
                ["OnTriggerStay"] = new[] { unityType("UnityEngine.Collider") },
                ["OnTriggerStay2D"] = new[] { unityType("UnityEngine.Collider2D") },
                // "OnValidate" is editor-only
                ["OnWillRenderObject"] = new Type[0],
                // "Reset" is editor-only
                ["Start"] = new Type[0],
                ["Update"] = new Type[0],
            };

            ScriptableObjectMethods = new Dictionary<string, Type[]>
            {
                ["Awake"] = new Type[0],
                ["OnDestroy"] = new Type[0],
                ["OnDisable"] = new Type[0],
                ["OnEnable"] = new Type[0],
            };
        }

        public static bool IsUnityMagicMethod(MethodBase method)
        {
            if (method == null
                || method.IsStatic
                || method.IsAbstract
                || method.IsGenericMethod
                || method.IsConstructor)
                return false;

            Type owner = method.DeclaringType;
            if (owner.IsSubclassOf(MonoBehaviourType))
                return CheckMagicMethod(method, MonoBehaviourMethods);
            else if (owner.IsSubclassOf(ScriptableObjectType))
                return CheckMagicMethod(method, ScriptableObjectMethods);
            return false;
        }

        private static bool CheckMagicMethod(MethodBase method, Dictionary<string, Type[]> magicMethods)
        {
            if (!magicMethods.TryGetValue(method.Name, out Type[] magicArgTypes))
                return false;

            ParameterInfo[] parameters = method.GetParameters();

            // No-args method with the correct name and correct owner -> magic method
            // If there are arguments, all of their types have to match the magic method template. This may be too restrictive.

            if (parameters.Length == 0)
                return true;
            else if (parameters.Length != magicArgTypes.Length)
                return false;

            for (int i = 0; i < parameters.Length; ++i)
            {
                if (magicArgTypes[i] == null
                    || magicArgTypes[i] != parameters[i].ParameterType)
                    return false;
            }

            return true;
        }
    }
}