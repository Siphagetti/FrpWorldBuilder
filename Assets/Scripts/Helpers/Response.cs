using UnityEngine;

public class Response
{
    public bool Success;
    public string MessageKey;
    public LogType Type;
}

public class Response<T> : Response
{
    public T Data;
}



