using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Singleton<T> where T: Singleton<T>,new()
{
    protected static T m_instance = null;

    public static T Instance
    {
        get
        {
            if(m_instance == null)
                m_instance = new T();
            return m_instance;
        }
    }
}