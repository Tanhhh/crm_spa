class Graph
{
    /// <summary>
    /// Đọc đồ thị dưới dạng Ma trận kề
    /// </summary>
    /// <param name="path">Đường dẫn đến file text chứa dữ liệu đồ thị</param>
    /// <param name="n">Số đỉnh của đồ thị</param>
    /// <param name="adjMatrix">Ma trận kề của đồ thị</param>
    static void ReadAdjMatrix(string path, out int n, out int[,] adjMatrix)
    {
        StreamReader sr = new StreamReader(path);
        n = Convert.ToInt32(sr.ReadLine());
        adjMatrix = new int[n,n];
        for(int u=0; u<n;u++)
        {
            string line = sr.ReadLine();
            string[] values = line.Split();
            for (int v = 0; v < n; v++)
                adjMatrix[u, v] = Convert.ToInt32(values[v]);
        }
        sr.Close();
    }

    /// <summary>
    /// Đọc đồ thị dưới dạng Danh sách kề
    /// </summary>
    /// <param name="path">Đường dẫn đến file text chứa dữ liệu đồ thị</param>
    /// <param name="n">Số đỉnh của đồ thị</param>
    /// <param name="adjList">Danh sách kề của đồ thị</param>
    static void ReadAdjList(string path, out int n, out List<int>[] adjList)
    {
        StreamReader sr = new StreamReader(path);
        n = Convert.ToInt32(sr.ReadLine());
        // Đồ thị có đỉnh đánh thứ tự từ 1
        adjList = new List<int>[n+1];
        for (int u = 1; u <= n; u++)
        {
            string line = sr.ReadLine();
            string[] values = line.Split();
            /*Cách 1: Thêm lần lượt từng đỉnh v kề u vào adjList[u]
            foreach(string v in values)
                adjList[u].Add(Convert.ToInt32(v));
            */

            //Cách 2: Chuyển string[] -> int[]. Sau đó chuyển int[] -> List<int>
            //Gán toàn bộ List<int> chuyển được vào adjList[u]
            adjList[u] = Array.ConvertAll(values, s => int.Parse(s)).ToList<int>();
        }
        sr.Close();
    }

    /// <summary>
    /// Đọc đồ thị dưới dạng Danh sách cạnh
    /// </summary>
    /// <param name="path">Đường dẫn đến file text chứa dữ liệu đồ thị</param>
    /// <param name="n">Số đỉnh của đồ thị</param>
    /// <param name="m">Số cạnh của đồ thị</param>
    /// <param name="edgeList">Danh sách cạnh của đồ thị</param>
    static void ReadEdgeList(string path, out int n, out int m, out List<Tuple<int,int>> edgeList)
    {
        StreamReader sr = new StreamReader(path);
        string line = sr.ReadLine();
        string[] values = line.Split();
        n = Convert.ToInt32(values[0]);
        m = Convert.ToInt32(values[1]);
        edgeList = new List<Tuple<int,int>>();

        for (int i = 0; i < m; i++)
        {
            line = sr.ReadLine();
            values = line.Split();
            int u = Convert.ToInt32(values[0]);
            int v = Convert.ToInt32(values[1]);
            Tuple<int, int> e = new Tuple<int, int>(u, v);
            edgeList.Add(e); 
        }
        sr.Close();
    }

    static void Main(string[] args)
    {
        /*
        int n;
        List<int>[] adjList;
        ReadAdjList("AdjacencyMatrix.INP", out n, out adjList);
        */

        /*
        int n;
        List<int>[] adjList;
        ReadAdjList("AdjacencyList.INP", out n, out adjList);
        */

        int n, m;
        List<Tuple<int,int>> edgeList;
        ReadEdgeList("EdgeList.INP", out n, out m, out edgeList);
        Console.WriteLine(n);
        int[] deg = new int[n+1];
        for(int i=0; i<m; i++)
        {
            int u = edgeList[i].Item1;
            int v = edgeList[i].Item2;
            deg[u]++;
            deg[v]++;
        }
        for (int u = 1; u <= n; u++)
        {
            Console.Write(deg[u] + " ");
        }
        Console.WriteLine();
    }
}

