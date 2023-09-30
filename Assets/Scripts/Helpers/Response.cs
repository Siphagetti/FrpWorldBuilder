using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Response
{
    public bool Success;
    public string Message;
}

public class Response<T> : Response
{
    public T Data;
}



