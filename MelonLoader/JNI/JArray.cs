#if ANDROID
namespace MelonLoader.Java;

using System.Collections;
using System.Collections.Generic;

public class JArray<T> : JObject, IEnumerable<T>
{
    public JArray() : base() { }

    public JArray(int size) => JNI.NewArray<T>(size);

    public int Length => JNI.GetArrayLength(this);

    public T this[int index]
    {
        get => JNI.GetArrayElement(this, index);
        set => JNI.SetArrayElement(this, index, value);
    }

    public T[] GetElements()
    {
        return JNI.GetArrayElements(this);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < this.Length; i++)
        {
            yield return JNI.GetArrayElement(this, i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
#endif
