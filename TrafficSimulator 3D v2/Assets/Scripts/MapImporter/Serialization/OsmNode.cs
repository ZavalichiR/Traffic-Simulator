using System.Xml;
using UnityEngine;


class OsmNode : BaseOsm
{

    public ulong ID { get; private set; }

    public bool traffic_lights { get; set; }
    public float Latitude { get; private set; }

    public float Longitude { get; private set; }

    // position in Unity units
    public float X { get; private set; }

    // position in Unity units
    public float Y { get; private set; }

    /// Implicit conversion between OsmNode and Vector3.
    public static implicit operator Vector3(OsmNode node)
    {
        return new Vector3(node.X, 0, node.Y);
    }

    //Constructor
    /// <param name="node">Xml node</param>
    public OsmNode(XmlNode node)
    {
        // Get the attribute values
        ID = GetAttribute<ulong>("id", node.Attributes);
        if(ID== 2625598326)
        {
            traffic_lights = false;
        }
        Latitude = GetAttribute<float>("lat", node.Attributes);
        Longitude = GetAttribute<float>("lon", node.Attributes);


        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            if (t.Attributes["k"] != null)
            {
                if (t.Attributes["v"].Value == "traffic_signals")
                    traffic_lights = true;
            }
            else
                traffic_lights = false;
        }

            

        // Calculate the position in Unity units
        X = (float)MercatorProjection.lonToX(Longitude);
        Y = (float)MercatorProjection.latToY(Latitude);
    }
}

