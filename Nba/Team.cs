using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nba
{
    public class Team
    {
        private string m_name;
        private int m_position;
        //logo

        public Team(string name, int position)
        {
            this.m_name = name;
            this.m_position = position;
        }
        public string Name { get { return m_name; } }
        public int Position { get { return m_position; } }
    }
}
