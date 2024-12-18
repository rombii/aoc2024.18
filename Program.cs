var map = new int[71, 71];
var bytes = await File.ReadAllLinesAsync(Path.Join(Directory.GetCurrentDirectory(), "input.txt"));
var bytesCoordinates = new (int, int)[bytes.Length];
var maps = new int[bytes.Length + 1][,];
for (var i = 0; i < bytes.Length; i++)
{
   var coords = bytes[i].Split(',').Select(int.Parse).ToArray();
   bytesCoordinates[i] = (coords[0], coords[1]);
}

maps[0] = map;
for (var i = 0; i < bytesCoordinates.Length; i++)
{
   maps[i + 1] = new int[maps[i].GetLength(0), maps[i].GetLength(1)];
   for (var x = 0; x < maps[i].GetLength(0); x++)
   {
      for (var y = 0; y < maps[i].GetLength(1); y++)
      {
         maps[i + 1][x, y] = maps[i][x, y];
      }
   }
   maps[i + 1][bytesCoordinates[i].Item1, bytesCoordinates[i].Item2] = 1;
}

Console.WriteLine($"First part: {FindShortestPath(maps[1024])}");

var left = 1025; // From the first part we know that first kb will not block path to the exit so we can skip them
var right = bytesCoordinates.Length - 1;
while (left < right)
{
   var pivot = (left + right) / 2;
   if (FindShortestPath(maps[pivot]) == -1)
   {
      right = pivot - 1;
   }
   else
   {
      left = pivot + 1;
   }
}

Console.WriteLine($"Second part: {bytes[left - 1]}"); // -1 because of maps length for dp



return;

int FindShortestPath(int[,] map)
{
   var directions = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) };
   var start = (0, 0);
   var end = (map.GetLength(0) - 1, map.GetLength(1) - 1);
   
   var distances = new int[map.GetLength(0), map.GetLength(1)];
   for (var i = 0; i < distances.GetLength(0); i++)
   {
      for (var j = 0; j < distances.GetLength(1); j++)
      {
         distances[i, j] = int.MaxValue;
      }
   }

   var pq = new SortedSet<(int dist, int r, int c)>();
   distances[start.Item1, start.Item2] = 0;
   pq.Add((0, start.Item1, start.Item2));

   while (pq.Count > 0)
   {
      var (current, r, c) = pq.Min;
      pq.Remove(pq.Min);

      if ((r, c) == end)
      {
         return current;
      }

      for (var i = 0; i < directions.Length; i++)
      {
         var newR = r + directions[i].Item1;
         var newC = c + directions[i].Item2;

         if (newR >= 0 && newR < map.GetLength(0) && newC >= 0 && newC < map.GetLength(1) && map[newR, newC] == 0)
         {
            var newDistance = current + 1;
            if (newDistance < distances[newR, newC])
            {
               pq.Remove((distances[newR, newC], newR, newC));
               distances[newR, newC] = newDistance;
               pq.Add((newDistance, newR, newC));
            }
         }
      }
   }

   return -1;
}