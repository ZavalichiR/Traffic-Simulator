using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

[ExecuteInEditMode]
class MapReader : MonoBehaviour
{

    [HideInInspector]
    public Dictionary<ulong, OsmNode> Nodes;

    [HideInInspector]
    public List<OsmWay> Ways;

    [HideInInspector]
    public OsmBounds Bounds;

    //public GameObject groundPlane;

    [Tooltip("The resource file that contains the OSM map data")]
    public string ResourceFile;

    public bool IsReady { get; private set; }

    void Start()
    {
        Nodes = new Dictionary<ulong, OsmNode>();
        Ways = new List<OsmWay>();

        var txtAsset = File.ReadAllText(@"C:/Users/zzava/Desktop/Proiect Licență/TrafficSimulator 3D v2/Assets/Resources/City/" + ResourceFile);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(txtAsset);

        //The bounds of the map
        SetBounds(doc.SelectSingleNode("/osm/bounds"));
        //All points on map
        GetNodes(doc.SelectNodes("/osm/node"));
        //All ways on map are created by a list of nodes
        GetWays(doc.SelectNodes("/osm/way"));

        float minx = (float)MercatorProjection.lonToX(Bounds.MinLon);
        float maxx = (float)MercatorProjection.lonToX(Bounds.MaxLon);
        float miny = (float)MercatorProjection.latToY(Bounds.MinLat);
        float maxy = (float)MercatorProjection.latToY(Bounds.MaxLat);

        //groundPlane.transform.localScale = new Vector3((maxx - minx) / 2, 1, (maxy - miny) / 2);
        IsReady = true;
    }

    void Update()
    {
        foreach (OsmWay w in Ways)
        {
            if (w.Visible)
            {
                bool ok = false;

                Vector3 lastPoint = Vector3.zero;

                Color c = Color.green;

                if (w.IsBuilding)
                {
                    c = Color.white;
                    ok = false;
                }
                else if (w.IsRoad)
                {
                    c = Color.red;

                    ok = true;
                }
                else if (w.IsBoundary)
                {
                    ok = false;
                    c = Color.green;
                }

                if (ok)
                {
                    for (int i = 1; i < w.NodeIDs.Count; i++)
                    {
                        OsmNode p1 = Nodes[w.NodeIDs[i - 1]];
                        OsmNode p2 = Nodes[w.NodeIDs[i]];

                        Vector3 v1 = p1 - Bounds.Centre;
                        Vector3 v2 = p2 - Bounds.Centre;

                        Debug.DrawLine(v1, v2, c);

                    }
                }
            }

        }
    }

    void GetWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            OsmWay way = new OsmWay(node);
            Ways.Add(way);
        }
    }

    void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode n in xmlNodeList)
        {
            OsmNode node = new OsmNode(n);
            Nodes[node.ID] = node;
        }
    }

    //Set bounds for city
    void SetBounds(XmlNode xmlNode)
    {
        Bounds = new OsmBounds(xmlNode);
    }
}