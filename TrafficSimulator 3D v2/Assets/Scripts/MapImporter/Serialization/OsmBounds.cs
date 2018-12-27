using System.Xml;
using UnityEngine;

class OsmBounds : BaseOsm
{
    // Minimum latitude (y-axis)
    public float MinLat { get; private set; }

    /// Maximum latitude (y-axis)
    public float MaxLat { get; private set; }

    /// Minimum longitude (x-axis)
    public float MinLon { get; private set; }

    /// Maximum longitude (x-axis)
    public float MaxLon { get; private set; }

    /// Centrul hăriții în unități Unity
    public Vector3 Centre { get; private set; }

    public OsmBounds(XmlNode node)
    {
        // Preia valorile nodului
        MinLat = GetAttribute<float>("minlat", node.Attributes);
        MaxLat = GetAttribute<float>("maxlat", node.Attributes);
        MinLon = GetAttribute<float>("minlon", node.Attributes);
        MaxLon = GetAttribute<float>("maxlon", node.Attributes);

        // Crează centrul hărții
        //			  x longitude
        //            |
        //            |
        //            |
        //--------(Centre)----------> y latitude
        //	          |
        //   	      |
        //            |
        float x = (float)((MercatorProjection.lonToX(MaxLon) + MercatorProjection.lonToX(MinLon)) / 2);
        float y = (float)((MercatorProjection.latToY(MaxLat) + MercatorProjection.latToY(MinLat)) / 2);
        Centre = new Vector3(x, 0, y);
    }
}