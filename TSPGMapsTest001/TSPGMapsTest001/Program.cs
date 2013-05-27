using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Google.Maps;
using Google.Maps.Direction;
using Google.Maps.DistanceMatrix;

namespace TSPGMapsTest001
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Google Maps TSP Test 001 - ©2013 Adam E. Reed. All rights reserved.");
            Console.ReadLine();
            
            var tsp = new Tsp();
            tsp.Do();

            Console.ReadLine();}

       
    }

    public class Tsp
    {
        public void Do()
        {
            var cities = new List<Location>
                {
                    new Location("475 hawley st lockport ny"),
                    new Location("1359 hartland rd barker ny"),
                    new Location("4444 main st gasport ny"),
                    new Location("4466 grayce ave gasport ny"),
                    new Location("79 south bristol lockport ny")
                };

            var nearestNeighTour = cities.Skip(1).Aggregate(
                new List<int> { 0 },
                (list, curr) =>
                {
                    var lastCity = cities[list.Last()];
                    var minDist = Enumerable.Range(0, cities.Count).Where(i => !list.Contains(i)).Min(cityIdx => Distance(lastCity, cities[cityIdx]));
                    var minDistCityIdx = Enumerable.Range(0, cities.Count).Where(i => !list.Contains(i)).First(cityIdx => minDist == Distance(lastCity, cities[cityIdx]));
                    list.Add(minDistCityIdx);
                    return list;
                });

            Console.WriteLine();
            Console.WriteLine("----------------------------");
            Console.WriteLine("Best Possible Route:");
            foreach (var i in nearestNeighTour)
            {
                Console.WriteLine(i);
            }
        }

        private double Distance(Location a, Location b)
        {
            return new Directions(a, b).Distance;
        }
    }

    public class Directions
    {
        private readonly Location _start;
        private readonly Location _stop;

        public Directions(Location start, Location stop)
        {
            _start = start;
            _stop = stop;
            Calculate();
        }

        public int Distance { get; private set; }

        public int Duration { get; private set; }

        private void Calculate()
        {
            var req = new DistanceMatrixRequest();
            req.AddOrigin(new Waypoint() { Address = _start.ToString() });
            req.AddDestination(new Waypoint() {Address = _stop.ToString()});
            req.Sensor = false;
            req.Mode = TravelMode.driving;
            req.Units = Units.imperial;

            var response = new DistanceMatrixService().GetResponse(req);
            
            Distance = Convert.ToInt32(response.Rows.First().Elements.First().distance.Value);
            Duration = Convert.ToInt32(response.Rows.First().Elements.First().duration.Value);

            Console.WriteLine("Distance from {0}, to {1} is {2} - Travel Time: {3}", _start, _stop, Distance, Duration);
        }
    }
}
