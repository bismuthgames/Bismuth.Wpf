using System;
using System.Collections.Generic;
using System.Text;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class GraphViewDemoViewModel
    {
        public GraphViewDemoViewModel()
        {
            Nodes = new[]
            {
                new Node(0, "Idle", 300, 50),
                new Node(1, "Jump", 500, 100),
                new Node(2, "Fall", 100, 300),
                new Node(3, "Run", 300, 200)
            };

            Connections = new[]
            {
                new Connection("Up", Nodes[0], Nodes[1]),
                new Connection("Down", Nodes[1], Nodes[2]),
                new Connection("Space", Nodes[2], Nodes[0]),
                new Connection("X", Nodes[0], Nodes[3]),
                new Connection("Y", Nodes[3], Nodes[0]),
            };
        }

        public Node[] Nodes { get; }
        public Connection[] Connections { get; }
    }

    public class Node
    {
        public Node(int id, string name, double x, double y)
        {
            Id = id;
            Name = name;
            X = x;
            Y = y;
        }

        public int Id { get; }
        public string Name { get; }
        public double X { get; }
        public double Y { get; }
    }

    public class Connection
    {
        public Connection(string name, Node a, Node b)
        {
            Name = name;
            A = a;
            B = b;
        }

        public string Name { get; }
        public Node A { get; }
        public Node B { get; }
    }
}
