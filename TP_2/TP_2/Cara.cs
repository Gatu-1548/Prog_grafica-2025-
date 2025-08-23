using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP_2
{
    public class Face
    {
        public List<int> Indices { get; set; } = new List<int>();
        public Face(params int[] indices)
        {
            Indices.AddRange(indices);
        }
    }
}
