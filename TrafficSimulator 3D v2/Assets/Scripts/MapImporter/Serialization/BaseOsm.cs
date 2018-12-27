using System;
using System.Xml;

class BaseOsm
{
    // E ok comentariul de genul d emai jos :D ?? 
    // Întreabă pe Alexandrescu, ( /// )
    /// <summary>
    /// Extrage datele din XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="attrName"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    protected T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
    {
        string strValue = attributes[attrName].Value;

        return (T)Convert.ChangeType(strValue, typeof(T));
    }
}

