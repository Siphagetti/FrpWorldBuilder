using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    // Overload the assignment operator (=) to assign from Quaternion
    public static implicit operator SerializableQuaternion(Quaternion q)
    {
        return new SerializableQuaternion(q);
    }

    // Overload the assignment operator (=) to assign to Quaternion
    public static implicit operator Quaternion(SerializableQuaternion sq)
    {
        return sq.ToQuaternion();
    }
}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    // Overload the assignment operator (=) to assign from Vector3
    public static implicit operator SerializableVector3(Vector3 v)
    {
        return new SerializableVector3(v);
    }

    // Overload the assignment operator (=) to assign to Vector3
    public static implicit operator Vector3(SerializableVector3 sv)
    {
        return sv.ToVector3();
    }
}

[Serializable]
public struct SerializableTransform
{
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public SerializableVector3 localScale;

    public static implicit operator SerializableTransform(Transform t)
    {
        var st = new SerializableTransform();

        st.position = t.position;
        st.rotation = t.rotation;
        st.localScale = t.localScale;

        return st;
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public int Count
    {
        get { return keys.Count; }
    }

    public TKey GetKeyAt(int index)
    {
        return keys[index];
    }

    public TValue GetValueAt(int index)
    {
        return values[index];
    }

    public void Add(TKey key, TValue value)
    {
        keys.Add(key);
        values.Add(value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = keys.IndexOf(key);
        if (index != -1)
        {
            value = values[index];
            return true;
        }
        else
        {
            value = default(TValue);
            return false;
        }
    }
}