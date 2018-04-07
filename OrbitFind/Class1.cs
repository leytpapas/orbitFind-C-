using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Xml;

namespace OrbitFind
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class GeoJson
    {
        List<Polygon> polygon = new List<Polygon>();

        public void addPoly(Polygon polygon) {
            this.polygon.Add(polygon);
        }

        public string toString() {
            string output = "{ \"type\": \"FeatureCollection\",\"features\": [";
            var pass = 0;
            foreach (Polygon poly in polygon) {
                output += "{\"type\": \"Feature\",\"geometry\": {\"type\": \"" + poly.getType() + "\",\"coordinates\": [";
                for (var i = 0; i < poly.getLat().Count; i++) {
                    output += "["+poly.getLat().ElementAt(i) + ", " + poly.getLong().ElementAt(i) + "],";
                }
                output = output.Substring(0, output.Length - 1);
                output += "]},";
                output += "\"properties\": {\"prop0\": \""+pass+"\"} },";
                pass += 1;
                Console.WriteLine(pass);
            }
            output = output.Substring(0, output.Length - 1);
            output += "]}";
            Console.WriteLine(output);
            return output;
            ;
        }

        public void readKMLa(string filepath)
        {
            string tab = "\t";
            string spaces = "";
            int i = 0;
            XmlTextReader reader = new XmlTextReader(filepath);
            while (reader.Read())
            {
                i += 1;
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        Console.Write(i+"."+spaces+"<" + reader.Name);
                        Console.WriteLine(">");
                        spaces += tab;
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(i + "." + spaces +reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        spaces = spaces.Substring(0, (spaces.Length <= 2) ? 0 : spaces.Length - 3);

                        Console.Write(i + "." + spaces +"</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                    default:
                        i--;
                        break;

                        break;
                }
            }
            Console.ReadLine();
        }
    }
}