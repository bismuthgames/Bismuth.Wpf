using System;
using System.Collections.Generic;
using System.Text;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class GraphControlDemoViewModel
    {
        public GraphControlDemoViewModel()
        {
            Nodes = new[]
            {
                new Node(0, "Idle"),
                new Node(1, "Jump"),
                new Node(2, "Fall"),
                new Node(3, "Run")
            };

            Connections = new[]
            {
                new Connection(Nodes[0], Nodes[1]),
                new Connection(Nodes[1], Nodes[2]),
                new Connection(Nodes[2], Nodes[0]),
                new Connection(Nodes[0], Nodes[3]),
                new Connection(Nodes[3], Nodes[0]),
            };
        }

        public Node[] Nodes { get; }
        public Connection[] Connections { get; }
    }

    public class Node
    {
        public Node(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }

    public class Connection
    {
        public Connection(Node a, Node b)
        {
            A = a;
            B = b;
        }

        public Node A { get; }
        public Node B { get; }
    }
}
