using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CountingRoadNetworks
{
    class Program
    {
        static int counter = 0;

        static void Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            List<int> requests = new List<int>();
            for (int i = 0; i < n; i++)
                requests.Add(Convert.ToInt32(Console.ReadLine()));
            
            foreach (int q in requests)
            {
                int maxSides = 0;
                counter = 0;

                for (int i = 1; i < q; i++)
                {
                    maxSides += i;
                }

                Area area = CreateMaxСonnectednessArea(q);

                List<Area> listarea = new List<Area>();
                listarea.Add(area);
                CountRoutesAfterSubstract(listarea);
                Console.WriteLine(counter );
            }
            Console.ReadKey();
        }

        static void CountRoutesAfterSubstract(List<Area> listarea)
        {
            foreach (Area a in listarea)
            {
                if (CheckСonnectedness(a))
                {
                    counter++;
                    CountRoutesAfterSubstract(SubstractByOneRoute(a));
                }
            }
        }

        static Area CreateMaxСonnectednessArea(int q)
        {
            Area area = new Area();
            area.cities = new List<City>();
            for (int i=0;i<q;i++)
            {
                area.cities.Add(new City() { name = i, connected = new List<City>()});
            }

            for (int i = 0; i < q; i++)
            {
                for (int j = 0; j < q; j++)
                {
                    if (area.cities[i].name!= area.cities[j].name)
                        area.cities[i].connected.Add(area.cities[j]);
                }
            }
            return area;
        }

        static List<Area> SubstractByOneRoute(Area base_area)
        {
            List<Area> res = new List<Area>();
            List<Connection> allConnections = new List<Connection>();

            foreach (City a in base_area.cities)
            {
                allConnections.AddRange(GetConnectionFromCity(a));
            }
            allConnections = DeduplicateConnections(allConnections);

            for (int i=0;i<allConnections.Count;i++)
            {
                res.Add(DeleteRoute(CopyArea(base_area), allConnections[i]));
            }

            return res;
        }

        public static Area CopyArea(object a)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream fs = new MemoryStream();
            formatter.Serialize(fs, a);
            fs.Position = 0;

            return (Area)formatter.Deserialize(fs);
        }

        static bool CheckСonnectedness(Area base_area)
        {
            List<TwoCities> citiesCoupleList = new List<TwoCities>();

            for (int i=0;i<base_area.cities.Count;i++)
            {
                for (int j=i+1;j< base_area.cities.Count;j++)
                {
                    citiesCoupleList.Add(new TwoCities() { city1= base_area.cities[i].name, city2 = base_area.cities[j].name });
                }
            }


            foreach (TwoCities a in citiesCoupleList)
            {
                Area area = CopyArea(base_area);
                Step(area, a.city1, a.city2);
                foreach (City b in area.cities)
                {
                    if (b.name==a.city2 && b.color!= true)
                        return false;
                }
            }


            return true;
        }

        

        static void Step(Area base_area, int cityPointer, int target)
        {
            int internalCityIndex=0;
            Area area = base_area;
            
            //mark start city
            for (int i=0;i<area.cities.Count;i++)
            {
                if (area.cities[i].name == cityPointer)
                {
                    area.cities[i].color = true;
                    internalCityIndex = i;
                    break;
                }
            }

            for (int i=0;i< area.cities[internalCityIndex].connected.Count;i++)
            {
                if (area.cities[internalCityIndex].connected[i].color!=true)
                    Step(area, area.cities[internalCityIndex].connected[i].name, target);
            }
        }


        static Area DeleteRoute(Area area, Connection connection)
        {
            Area res = area;
            int cityadress1=0, cityadress2=0;

            for (int i=0;i< res.cities.Count;i++)
            {
                if (res.cities[i].name == connection.Start)
                    cityadress1 = i;
                if (res.cities[i].name == connection.End)
                    cityadress2 = i;
            }

            for (int i=0;i< res.cities[cityadress1].connected.Count;i++)
            {
                if (res.cities[cityadress1].connected[i].name==connection.End)
                    res.cities[cityadress1].connected.RemoveAt(i);
            }
            for (int i = 0; i < res.cities[cityadress2].connected.Count; i++)
            {
                if (res.cities[cityadress2].connected[i].name == connection.Start) 
                    res.cities[cityadress2].connected.RemoveAt(i);
            }

            return res;
        }

        static List<Connection> GetConnectionFromCity(City city)
        {
            List<Connection> res = new List<Connection>();
            foreach (City a in city.connected)
            {
                res.Add(new Connection() { Start = city.name, End = a.name } );
            }
            return res;
        }

        static List<Connection> DeduplicateConnections(List<Connection> connections)
        {
            List<Connection> res = new List<Connection>();
            res.AddRange(connections);

            for (int i=0;i< res.Count;i++)
            {
                for (int j=i;j<res.Count;j++)
                {
                    if (res[i].Start==res[j].End && res[i].End == res[j].Start)
                        res.RemoveAt(j);
                }
            }

            return res;
        }

    }

    [Serializable]
    public class City
    {
        public int name;
        public bool color;
        public List<City> connected;
    }

    [Serializable]
    public class Area
    {
        public List<City> cities;
    }

    public class Connection
    {
        public int Start;
        public int End;
    }

    public class TwoCities
    {
        public int city1;
        public int city2;
    }


}
