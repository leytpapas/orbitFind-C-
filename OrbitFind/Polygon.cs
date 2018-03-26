using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrbitFind
{
    class Polygon
    {
        private List<Double> lat_array;
        private List<Double> long_array;

        public Polygon(String path) {

            string pattern = @"(-?[0-9]+?\.+?[0-9]*), (-?[0-9]+?\.+?[0-9]*)";
            foreach (Match m in Regex.Matches(path, pattern))
            {
                //Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
                //Console.WriteLine(m.Groups[0]);
                lat_array.Add(double.Parse(m.Groups[1].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //lat
                long_array.Add(double.Parse(m.Groups[2].ToString(), System.Globalization.CultureInfo.InvariantCulture)); //long
            }

        }
        public Polygon(List<Double> lat_array,List<Double> long_array) {
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

    }
}
