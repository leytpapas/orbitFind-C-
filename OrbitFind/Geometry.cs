using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitFind
{
    class Geometry
    {
        public static double PI = 3.14159265;
        public static double TWOPI = 2 * PI;

        private String coordinate_is_inside_polygon(double latitude, double longitude, List<Double> lat_array, List<Double> long_array)
        {
            int i;
            double angle = 0;
            double point1_lat;
            double point1_long;
            double point2_lat;
            double point2_long;
            int n = lat_array.Count;

            for (i = 0; i < n; i++)
            {
                point1_lat = lat_array.ElementAt(i) - latitude;
                point1_long = long_array.ElementAt(i) - longitude;
                point2_lat = lat_array.ElementAt((i + 1) % n) - latitude;
                point2_long = long_array.ElementAt((i + 1) % n) - longitude;
                angle += Angle2D(point1_lat, point1_long, point2_lat, point2_long);
                if (Math.Abs(angle) >= PI)
                {

                    return (latitude + "," + longitude + ",0");
                }
            }
            return null;
        }

        public static double Angle2D(double y1, double x1, double y2, double x2)
        {
            double theta1 = Math.Atan2(y1, x1);
            double theta2 = Math.Atan2(y2, x2);
            double dtheta = theta2 - theta1;

            while (dtheta > PI)
            {
                dtheta -= TWOPI;
            }
            while (dtheta < -PI)
            {
                dtheta += TWOPI;
            }

            return (dtheta);
        }

        public String checkPointDensity(Double[] point1, Double[] point2, Polygon polygon)
        {
            //System.out.print("Close...");

            Double maxLat = polygon.findMaxLat();
            Double maxLong = polygon.findMaxLong();
            Double minLat = polygon.findMinLat();
            Double minLong = polygon.findMinLong();

            Double latDiff = maxLat - minLat;
            Double longDiff = maxLong - minLong;

            Double distance = Math.Sqrt((point1[0] - point2[0]) * (point1[0] - point2[0]) + (point1[1] - point2[1]) * (point1[1] - point2[1]));

            if (((distance > latDiff) || (distance > longDiff)) && (latDiff > longDiff))
            {
                int n = 4 * (int)(distance / longDiff);
                return fixPointDensity(point1, point2, polygon, n);
            }
            else if (((distance > latDiff) || (distance > longDiff)) && (longDiff > latDiff))
            {
                int n = 4 * (int)(distance / latDiff);
                return fixPointDensity(point1, point2, polygon, n);
            }
            else
            {
                return null;
            }
        }

        public String fixPointDensity(Double[] point1, Double[] point2, Polygon polygon, int n)
        {
            List<Double> tempLat = polygon.getLat();
            List<Double> tempLong = polygon.getLong();
            Double[] point = point1;

            Double latStep = (point2[0] - point1[0]) / n;
            Double longStep = (point2[1] - point1[1]) / n;

            for (int i = 0; i < n; i++)
            {
                point[0] = point[0] + latStep;
                point[1] = point[1] + longStep;

                String pointInside = coordinate_is_inside_polygon(point[0], point[1], tempLat, tempLong);

                if (pointInside != null)
                {
                    return pointInside;
                }
                else
                {
                    continue;
                }
            }

            return null;
        }

        public Boolean isCloseToPolygon(Double[] point, Polygon polygon)
        {
            Double maxLat = polygon.findMaxLat();
            Double maxLong = polygon.findMaxLong();
            Double minLat = polygon.findMinLat();
            Double minLong = polygon.findMinLong();

            Double latDiff = polygon.findMaxLat() - polygon.findMinLat();
            Double longDiff = polygon.findMaxLong() - polygon.findMinLong();

            Double latitude = point[0];
            Double longtitude = point[1];

            if ((latitude > minLat - latDiff) && (latitude < maxLat + latDiff) && (longtitude > minLong - longDiff) && (longtitude < maxLong + longDiff))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public String checkOrbit(Polygon polygon, Polygon orbit)
        {
            List<Double> polygonLat = polygon.getLat();
            List<Double> polygonLong = polygon.getLong();
            List<Double> orbitLat = orbit.getLat();
            List<Double> orbitLong = orbit.getLong();
            String pointInside = null;
            Double[] point1 = new Double[2];
            Double[] point2 = new Double[2];

            for (int i = 0; i < orbitLat.Count; i++)
            {
                point1[0] = orbitLat.ElementAt(i);
                point1[1] = orbitLong.ElementAt(i);

                pointInside = coordinate_is_inside_polygon(point1[0], point1[1], polygonLat, polygonLong);
                if (pointInside != null)
                {
                    return pointInside;
                }
                else if ((isCloseToPolygon(point1, polygon)) && (i < orbitLat.Count - 1))
                {
                    point2[0] = orbitLat.ElementAt(i + 1);
                    point2[1] = orbitLong.ElementAt(i + 1);
                    pointInside = checkPointDensity(point1, point2, polygon);
                    if (pointInside != null)
                    {
                        return pointInside;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            return pointInside;

        }

        public String checkAreaInSight(Polygon polygon, Polygon orbit, Double distance)
        {
            List<Double> tempLat = new List<Double>();
            List<Double> tempLong = new List<Double>();
            List<Double> polygonLat = polygon.getLat();
            List<Double> polygonLong = polygon.getLong();
            List<Double> orbitLat = orbit.getLat();
            List<Double> orbitLong = orbit.getLong();
            Double[] currentPoint = new Double[2];
            Double[] point1 = new Double[2];
            Double[] point2 = new Double[2];
            Double[] point3 = new Double[2];
            Double[] point4 = new Double[2];
            String pointInside = null;
            for (int i = 1; i < orbitLat.Count - 1; i++)
            {
                currentPoint[0] = orbitLat.ElementAt(i);
                currentPoint[1] = orbitLong.ElementAt(i);

                if (isCloseToPolygon(currentPoint, polygon))
                {


                    point1[0] = orbitLat.ElementAt(i - 1) + distance;
                    point1[1] = (orbitLong.ElementAt(i - 1) + distance) % 180;
                    point2[0] = orbitLat.ElementAt(i - 1) - distance;
                    point2[1] = (orbitLong.ElementAt(i - 1) - distance) % -180;
                    point3[0] = orbitLat.ElementAt(i + 1) + distance;
                    point3[1] = (orbitLong.ElementAt(i + 1) + distance) % 180;
                    point4[0] = orbitLat.ElementAt(i + 1) - distance;
                    point4[1] = (orbitLong.ElementAt(i + 1) - distance) % -180;

                    if (point1[0] > 90)
                    {
                        point1[0] = 90.0;
                    }
                    else if (point1[1] > 180)
                    {
                        point1[1] = point1[1] - 360;
                    }
                    else if (point2[0] < -90)
                    {
                        point2[0] = -90.0;
                    }
                    else if (point2[1] < -180)
                    {
                        point2[1] = point2[1] + 360;
                    }
                    else if (point3[0] > 90)
                    {
                        point1[0] = 90.0;
                    }
                    else if (point3[1] > 180)
                    {
                        point1[1] = point1[1] - 360;
                    }
                    else if (point4[0] < -90)
                    {
                        point2[0] = -90.0;
                    }
                    else if (point4[1] < -180)
                    {
                        point2[1] = point2[1] + 360;
                    }
                    else
                    {
                        continue;
                    }

                    tempLat.Add(point1[0]);
                    tempLat.Add(point2[0]);
                    tempLat.Add(point3[0]);
                    tempLat.Add(point4[0]);
                    tempLong.Add(point1[1]);
                    tempLong.Add(point2[1]);
                    tempLong.Add(point3[1]);
                    tempLong.Add(point4[1]);

                    for (int j = 0; j < polygonLat.Count; j++)
                    {
                        pointInside = coordinate_is_inside_polygon(polygonLat.ElementAt(j), polygonLong.ElementAt(j), tempLat, tempLong);
                        if (pointInside != null)
                        {
                            return pointInside;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    tempLat.Clear();
                    tempLong.Clear();

                }
                else
                {
                    continue;
                }
            }
            pointInside = checkOrbit(polygon, orbit);
            return pointInside;
        }

    }
}
