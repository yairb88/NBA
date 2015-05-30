using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Nba
{
    public class Game : IGame
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static int FINAL_SCORE = 4;
        private Team m_team1;
        private Team m_team2;
        private int m_team1Score;
        private int m_team2Score;
        private Team m_winnerTeam;

        public Game(Team team1, Team team2)
        {
            m_team1 = team1;
            m_team1Score = 0;
            m_team2 = team2;
            m_team2Score = 0;
        }

        public void SetScore(int score1, int score2)
        {
            m_team1Score = score1;
            m_team2Score = score2;

            checkScore();
        }

        private void checkScore()
        {
            if (m_team1Score == FINAL_SCORE)
            {
                m_winnerTeam = m_team1;
                log.InfoFormat("Winning Team is: {0}", m_winnerTeam.Name); 
            }
            else if (m_team2Score == FINAL_SCORE)
            {
                m_winnerTeam = m_team2;
                log.InfoFormat("Winning Team is: {0}", m_winnerTeam.Name);
            }
        }

        public Team Team1
        {
            get
            {
                return m_team1;
            }
        }
        public Team Team2
        {
            get
            {
                return m_team2;
            }
        }
        public int Team1Score
        {
            get
            {
                return m_team1Score;
            }
        }
        public int Team2Score
        {
            get
            {
                return m_team2Score;
            }
        }
        public Team WinnerTeam
        {
            get
            {
                return m_winnerTeam;
            }
        }

    }
}
