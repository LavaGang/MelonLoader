using System;
using System.Collections.Generic;
using System.Reflection;

namespace MelonLoader.InternalUtils;

/*
 * A list of Unity's built-in messages / magic methods and their (optional) argument types.
 * Used to prevent false-positive "IL2CPP method got inlined, patch may not work" warnings.
 * Put in its own static class to prevent unnecessary initialization for Mono games.
 */
public static class UnityMagicMethods
{

    private static readonly Type MonoBehaviourType = null;
    private static readonly Type ScriptableObjectType = null;

    // Key = name of magic method. Value = optional argument types
    // See: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    private static readonly Dictionary<string, Type[]> MonoBehaviourMethods = null;
    // See: https://docs.unity3d.com/ScriptReference/ScriptableObject.html
    private static readonly Dictionary<string, Type[]> ScriptableObjectMethods = null;

    static UnityMagicMethods()
    {
        var unityAssembly = Assembly.Load("UnityEngine.CoreModule") ?? Assembly.Load("UnityEngine");
        Type unityType(string name) // This may return null - especially for types such as NetworkPlayer that have been removed in newer Unity versions
            => unityAssembly.GetType(name);

        MonoBehaviourType = unityType("UnityEngine.MonoBehaviour");
        ScriptableObjectType = unityType("UnityEngine.ScriptableObject");

        MonoBehaviourMethods = new Dictionary<string, Type[]>
        {
            ["Awake"] = [],
            ["FixedUpdate"] = [],
            ["LateUpdate"] = [],
            ["OnAnimatorIK"] = [typeof(int)],
            ["OnAnimatorMove"] = [],
            ["OnApplicationFocus"] = [typeof(bool)],
            ["OnApplicationPause"] = [typeof(bool)],
            ["OnApplicationQuit"] = [],
            ["OnAudioFilterRead"] = [typeof(float[]), typeof(int)],
            ["OnBecameInvisible"] = [],
            ["OnBecameVisible"] = [],
            ["OnCollisionEnter"] = [unityType("UnityEngine.Collision")],
            ["OnCollisionEnter2D"] = [unityType("UnityEngine.Collision2D")],
            ["OnCollisionExit"] = [unityType("UnityEngine.Collision")],
            ["OnCollisionExit2D"] = [unityType("UnityEngine.Collision2D")],
            ["OnCollisionStay"] = [unityType("UnityEngine.Collision")],
            ["OnCollisionStay2D"] = [unityType("UnityEngine.Collision2D")],
            ["OnConnectedToServer"] = [],
            ["OnControllerColliderHit"] = [unityType("UnityEngine.ControllerColliderHit")],
            ["OnDestroy"] = [],
            ["OnDisable"] = [],
            ["OnDisconnectedFromServer"] = [unityType("UnityEngine.NetworkDisconnection")],
            // "OnDrawGizmos" is editor-only
            // "OnDrawGizmosSelected" is editor-only
            ["OnEnable"] = [],
            ["OnFailedToConnect"] = [unityType("UnityEngine.NetworkConnectionError")],
            ["OnFailedToConnectToMasterServer"] = [unityType("UnityEngine.NetworkConnectionError")],
            ["OnGUI"] = [],
            ["OnJointBreak"] = [typeof(float)],
            ["OnJointBreak2D"] = [unityType("UnityEngine.Joint2D")],
            ["OnMasterServerEvent"] = [unityType("UnityEngine.MasterServerEvent")],
            ["OnMouseDown"] = [],
            ["OnMouseDrag"] = [],
            ["OnMouseEnter"] = [],
            ["OnMouseExit"] = [],
            ["OnMouseOver"] = [],
            ["OnMouseUp"] = [],
            ["OnMouseUpAsButton"] = [],
            ["OnNetworkInstantiate"] = [unityType("UnityEngine.NetworkMessageInfo")],
            ["OnParticleCollision"] = [unityType("UnityEngine.GameObject")],
            ["OnParticleSystemStopped"] = [],
            ["OnParticleTrigger"] = [],
            ["OnParticleUpdateJobScheduled"] = [],
            ["OnPlayerConnected"] = [unityType("UnityEngine.NetworkPlayer")],
            ["OnPlayerDisconnected"] = [unityType("UnityEngine.NetworkPlayer")],
            ["OnPostRender"] = [],
            ["OnPreCull"] = [],
            ["OnPreRender"] = [],
            ["OnRenderImage"] = [unityType("UnityEngine.RenderTexture"), unityType("UnityEngine.RenderTexture")],
            ["OnRenderObject"] = [],
            ["OnSerializeNetworkView"] = [unityType("UnityEngine.BitStream"), unityType("UnityEngine.NetworkMessageInfo")],
            ["OnServerInitialized"] = [],
            ["OnTransformChildrenChanged"] = [],
            ["OnTransformParentChanged"] = [],
            ["OnTriggerEnter"] = [unityType("UnityEngine.Collider")],
            ["OnTriggerEnter2D"] = [unityType("UnityEngine.Collider2D")],
            ["OnTriggerExit"] = [unityType("UnityEngine.Collider")],
            ["OnTriggerExit2D"] = [unityType("UnityEngine.Collider2D")],
            ["OnTriggerStay"] = [unityType("UnityEngine.Collider")],
            ["OnTriggerStay2D"] = [unityType("UnityEngine.Collider2D")],
            // "OnValidate" is editor-only
            ["OnWillRenderObject"] = [],
            // "Reset" is editor-only
            ["Start"] = [],
            ["Update"] = [],
        };

        ScriptableObjectMethods = new Dictionary<string, Type[]>
        {
            ["Awake"] = [],
            ["OnDestroy"] = [],
            ["OnDisable"] = [],
            ["OnEnable"] = [],
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

        var owner = method.DeclaringType;
        if (owner.IsSubclassOf(MonoBehaviourType))
            return CheckMagicMethod(method, MonoBehaviourMethods);
        else if (owner.IsSubclassOf(ScriptableObjectType))
            return CheckMagicMethod(method, ScriptableObjectMethods);
        return false;
    }

    private static bool CheckMagicMethod(MethodBase method, Dictionary<string, Type[]> magicMethods)
    {
        if (!magicMethods.TryGetValue(method.Name, out var magicArgTypes))
            return false;

        var parameters = method.GetParameters();

        // No-args method with the correct name and correct owner -> magic method
        // If there are arguments, all of their types have to match the magic method template. This may be too restrictive.

        if (parameters.Length == 0)
            return true;
        else if (parameters.Length != magicArgTypes.Length)
            return false;

        for (var i = 0; i < parameters.Length; ++i)
        {
            if (magicArgTypes[i] == null
                || magicArgTypes[i] != parameters[i].ParameterType)
                return false;
        }

        return true;
    }
}