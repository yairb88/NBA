using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nba
{
    public interface IGame
    {
        Team Team1 { get; }
        Team Team2 { get; }
        int Team1Score { get; }
        int Team2Score { get; }
        Team WinnerTeam { get; }
    }
}
