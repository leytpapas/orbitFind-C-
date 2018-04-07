using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrbitFind
{
    class Polygon
    {
        string type = "";
        private List<Double> lat_array = new List<double>();
        private List<Double> long_array = new List<double>();

        private List<Double> lat_array2 = new List<double>();
        private List<Double> long_array2 = new List<double>();

        public Polygon(String path,string type) {
            this.type = type;

            string pattern = @"(-?[0-9]+?\.+?[0-9]*),[ ]?(-?[0-9]+?\.+?[0-9]*)";
            foreach (Match m in Regex.Matches(path, pattern))
            {
                //Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
                // Console.WriteLine(m.Groups[0]); 
                // group 0 is the whole match and 1,2 are latitude, longtitude respectively
                try
                {
                    lat_array.Add(double.Parse(m.Groups[1].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //lat
                    long_array.Add(double.Parse(m.Groups[2].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //long
                }
                catch (IndexOutOfRangeException e) {
                    Console.WriteLine(e.ToString());
                    throw new Exception("Expected \"lat,long\"");
                }
                
            }
        }

        //path1: left , path2: right
        public Polygon(String path1, String path2, string type)
        {
            this.type = type;
            double width = 0;
            string pattern = @"(-?[0-9]+?\.+?[0-9]*),[ ]?(-?[0-9]+?\.+?[0-9]*)";
            foreach (Match m in Regex.Matches(path1, pattern))
            {
                //Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
                // Console.WriteLine(m.Groups[0]); 
                // group 0 is the whole match and 1,2 are latitude, longtitude respectively
                try
                {
                    lat_array.Add(double.Parse(m.Groups[1].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //lat
                    long_array.Add(double.Parse(m.Groups[2].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //long
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e.ToString());
                    throw new Exception("Expected \"lat,long\"");
                }

            }

            foreach (Match m in Regex.Matches(path2, pattern))
            {
                //Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
                // Console.WriteLine(m.Groups[0]); 
                // group 0 is the whole match and 1,2 are latitude, longtitude respectively
                try
                {
                    lat_array2.Add(double.Parse(m.Groups[1].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //lat
                    long_array2.Add(double.Parse(m.Groups[2].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //long
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e.ToString());
                    throw new Exception("Expected \"lat,long\"");
                }

            }

            var len = lat_array.Count;
            for (var i = 0; i < len; i++) {
                var plus = (Math.Sqrt(Math.Pow(long_array.ElementAt(i) - long_array2.ElementAt(len-1 - i), 2) + Math.Pow(lat_array.ElementAt(i) - lat_array2.ElementAt(len-1 - i), 2)));
                //Console.WriteLine(((360 - plus > plus)?plus:360-plus));
                width += plus;
            }
            Console.WriteLine("~"+lat_array.Count+"~!" + (width / lat_array.Count));
        }

        public Polygon(List<Double> lat_array,List<Double> long_array,string type) {
            this.type = type;
            if (lat_array.Count() != long_array.Count()) {
                throw new Exception("Two lists must be of equal size");
            }
            this.lat_array = lat_array;
            this.long_array = long_array;
            
        }
        
        public Double findMinLat(){
            Double min = lat_array.ElementAt(0);

            for (int i = 1; i < lat_array.Count; i++)
            {
                if (min > lat_array.ElementAt(i))
                {
                    min = lat_array.ElementAt(i);
                }
            }

            return min;
        }

        public Double findMinLong()
        {
            Double min = long_array.ElementAt(0);

            for (int i = 1; i < long_array.Count; i++)
            {
                if (min > long_array.ElementAt(i))
                {
                    min = long_array.ElementAt(i);
                }
            }

            return min;
        }

        public Double findMaxLat()
        {
            Double max = lat_array.ElementAt(0);

            for (int i = 1; i < lat_array.Count; i++)
            {
                if (max < lat_array.ElementAt(i))
                {
                    max = lat_array.ElementAt(i);
                }
            }

            return max;
        }

        public Double findMaxLong()
        {
            Double max = long_array.ElementAt(0);

            for (int i = 1; i < long_array.Count; i++)
            {
                if (max < long_array.ElementAt(i))
                {
                    max = long_array.ElementAt(i);
                }
            }

            return max;
        }

        public List<Double> getLat()
        {
            return this.lat_array;
        }

        public List<Double> getLong()
        {
            return this.long_array;
        }

        public String getType() { return type; }

        public string ToString() {
            string output = "";

            for(var i= 0; i < lat_array.Count();i++) {
                output += lat_array.ElementAt(i) + ", " + long_array.ElementAt(i) + "\n";
            }
            return output;
        }
    }
}
