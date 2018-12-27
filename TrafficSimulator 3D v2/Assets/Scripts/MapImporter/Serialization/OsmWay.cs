using System.Collections.Generic;
using System.Xml;

class OsmWay : BaseOsm
{
    // ID-ul drumului
    public ulong ID { get; private set; }

    // True- drumul este vizibil
    public bool Visible { get; private set; }

    // Lista id-urilor unui Nod
    public List<ulong> NodeIDs { get; private set; }

    // True- drum închis
    public bool IsBoundary { get; private set; }

    // True- lista de puncte reprezintă o clădire
    public bool IsBuilding { get; private set; }

    // True- lista de puncte reprezintă un drum
    public bool IsRoad { get; private set; }

    // True- lista de puncte reprezintă un drum cu sens unic
    public bool IsOneWay { get; private set; }

    // Tipul drumului
    public string TypeOfRoad { get; private set; }

    // Înălțimea unei clădiri
    public float Height { get; private set; }
 
    /// Numele obiectului
    public string Name { get; private set; }

    // Numărul de benzi pentru un drum
    public int Lanes { get; private set; }


    /// Constructor.
    public OsmWay(XmlNode node)
    {
        NodeIDs = new List<ulong>();
        Height = 3.0f; 
        Lanes = 1; 
        Name = "";

        // Extrace atributele unui nod
        ID = GetAttribute<ulong>("id", node.Attributes);
        Visible = GetAttribute<bool>("visible", node.Attributes);

        // Exrage nodurile
        XmlNodeList nds = node.SelectNodes("nd");
        foreach (XmlNode n in nds)
        {
            ulong refNo = GetAttribute<ulong>("ref", n.Attributes);
            NodeIDs.Add(refNo);
        }

        // Determină ce tip reprezintă lista de noduri
        // Drum sau Clădire
        if (NodeIDs.Count > 1)
        {
            IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count - 1];
        }

        // Citește tagurile din XML	
        XmlNodeList tags = node.SelectNodes("tag");
        TypeOfRoad = "OneLane";
        foreach (XmlNode t in tags)
        {
            string value = "";
            
            string key = GetAttribute<string>("k", t.Attributes);
            if (key == "building:levels")
            {
                // 10 foot =3.0 m
                Height = 3.0f * GetAttribute<float>("v", t.Attributes);
            }
            else if (key == "height")
            {
                // 1 foot = 0.3040 m
                Height = 0.3048f * GetAttribute<float>("v", t.Attributes);
            }
            else if (key == "building" || key == "building:levels")
            {
                IsBuilding = true;
                if (GetAttribute<string>("v", t.Attributes) == "house")
                    Name = "house";
            }
            else if (key == "highway")
            {
                value = GetAttribute<string>("v", t.Attributes);
                
                if (value != "footway" && value != "steps" && value != "cycleway")
                    IsRoad = true;
                else
                {
                    IsRoad = false;
                    TypeOfRoad = "foot";
                }
                    
                if (value == "tertiary" || value == "service" || value == "track" || value == "path")
                {
                    TypeOfRoad = "service";
                }
            }
            else if (key == "oneway")
            {
                value = GetAttribute<string>("v", t.Attributes);
                if (value == "yes")
                {
                    IsOneWay = true;
                    TypeOfRoad = "oneway";
                }                 
                else
                    IsOneWay = false;
            }
            else if (key == "lanes")
            {
                Lanes = GetAttribute<int>("v", t.Attributes);
                if (Lanes >= 2)
                    TypeOfRoad = "TwoLanes";
            }
            else if (key == "amenity")
            {
                value = GetAttribute<string>("v", t.Attributes);
               if(value == "parking")
                    TypeOfRoad = "parking";
            }
            else if (key == "name")
            {
                Name = GetAttribute<string>("v", t.Attributes);
            }
        }
    }
}

