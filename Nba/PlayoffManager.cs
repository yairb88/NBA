using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net;

namespace Nba
{
    class PlayoffManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        enum Region { East, West };
        enum PlayoffRuond { Quarter, SemiFinals, RegionalFinals, NbaFinals };
        private static string TEAMS_XML_FILENAME = "Teams.xml";
        private static string EAST_TEAM_PATH = "//Playoff//East//Team";
        private static string WEST_TEAM_PATH = "//Playoff//West//Team";
        private XmlDocument xmlTeams;
        private XmlNodeList eastTeams;
        private XmlNodeList westTeams;
        private XmlWriter xmlWriter;

        private Team[] m_east = new Team[8];
        private Team[] m_west = new Team[8];
        private Game[] m_eastQuarter = new Game[4];
        private Game[] m_westQuarter = new Game[4];
        private Game[] m_eastSemiFinal = new Game[2];
        private Game[] m_westSemiFinal = new Game[2];
        private Game m_eastFinals;
        private Game m_westFinals;
        private Game m_nbaFinals;
        private Team m_nbaChampion;

        public PlayoffManager()
        {
            //Log4NetInitializer.Init();
            initializeFromXml();
            firstRoundBuild();
        }

        private void initializeFromXml()
        {
            xmlTeams = new XmlDocument();
            log.InfoFormat("Load the xml file from: {0}",TEAMS_XML_FILENAME);
            xmlTeams.Load(TEAMS_XML_FILENAME);
            eastTeams = xmlTeams.SelectNodes(EAST_TEAM_PATH);
            westTeams = xmlTeams.SelectNodes(WEST_TEAM_PATH);
            log.InfoFormat("Initialize team array");
            getTeamsNameAndPos(eastTeams, m_east);
            getTeamsNameAndPos(westTeams, m_west);
        }

        private void firstRoundBuild()
        {
            log.InfoFormat("Building the quarter finals");
            log.InfoFormat("East:");
            buildQuarter(m_east, m_eastQuarter);
            log.InfoFormat("West:");
            buildQuarter(m_west, m_westQuarter);
        }

        private void getTeamsNameAndPos(XmlNodeList nodeList, Team[] nbaTeams)
        {
            log.InfoFormat("Getting teams:");
            //index for putting the teams in the array
            int i = 0;
            foreach (XmlNode team in nodeList)
            {
                string name = team.Attributes["Name"].Value;
                string pos = team.Attributes["Position"].Value;
                int position = int.Parse(pos);
                nbaTeams[i] = new Team(name, position);
                i++;
                log.InfoFormat("Team: {0} position: {1}", name, pos);
            }
        }

        private void buildQuarter(Team[] teams, Game[] games)
        {
            log.InfoFormat("Creating quartere finales matches");
            games[0] = new Game(teams[0], teams[7]); // 1 place vs 8 place
            games[1] = new Game(teams[3], teams[4]); // 4 place vs 5 place
            games[2] = new Game(teams[1], teams[6]); // 2 place vs 7 place
            games[3] = new Game(teams[2], teams[5]); // 3 place vs 6 place
            printLogMatches(games);
        }

        private void buildSemiFinals(Game[] quarters, Game[] semiFinals)
        {
            log.InfoFormat("The semi finals are:");
            Team winner1 = quarters[0].WinnerTeam;
            Team winner2 = quarters[1].WinnerTeam;
            semiFinals[0] = new Game(winner1, winner2);

            winner1 = quarters[2].WinnerTeam;
            winner2 = quarters[3].WinnerTeam;
            semiFinals[1] = new Game(winner1, winner2);

            printLogMatches(semiFinals);
        }

        private void regionlfinalsBuilder(Game[] semiFinals, Region region)
        {
            Team winner1 = semiFinals[0].WinnerTeam;
            Team winner2 = semiFinals[1].WinnerTeam;
            if (region == Region.East)
            {
                m_eastFinals = new Game(winner1, winner2);
                log.InfoFormat("The East regional final: {0} VS {1}", winner1, winner2);
            }
            else if (region == Region.West)
            {
                m_westFinals = new Game(winner1, winner2);
                log.InfoFormat("The West regional final: {0} VS {1}", winner1, winner2);
            }
        }

        private void nbaFinalsBuilder()
        {
            
            Team winner1 = m_eastFinals.WinnerTeam;
            Team winner2 = m_westFinals.WinnerTeam;
            log.InfoFormat("Create Nba final betwee {0} and {1}", winner1.Name,winner2.Name);
            m_nbaFinals = new Game(winner1, winner2);
        }

        private void printLogMatches(Game[] matches)
        {
            foreach (Game game in matches)
            {
                log.InfoFormat("{0} VS {1}", game.Team1.Name, game.Team2.Name);
            }
        }

        public void ExportPalyoff(){
            XmlWriterManager.ExportPlayoff(m_eastQuarter, m_eastSemiFinal, m_eastFinals, m_westQuarter, m_westSemiFinal, m_westFinals, m_nbaFinals);
        }

        public void SetScore(int scoreTeam1, int scoreTeam2, int index, Region region, PlayoffRuond ruond)
        {
            if (0 > scoreTeam1 || scoreTeam1 > 4 || 0 > scoreTeam2 || scoreTeam2 > 4)
            {
                log.ErrorFormat("The score must be less then 4 but biger then 0, score1: {0} score2 : {1}", score1, score2);
                return;
            }
            else if (scoreTeam1 == 4 && scoreTeam2 == 4)
            {
                log.ErrorFormat("Only one team can get score of 4");
                return;
            }
            else
            {

                switch (ruond)
                {
                    case PlayoffRuond.Quarter:
                        if (Region.East == region)
                        {
                            setGame(m_eastQuarter[index], scoreTeam1, scoreTeam2);
                        }
                        else if (Region.West == region)
                        {
                            setGame(m_westQuarter[index], scoreTeam1, scoreTeam2);
                        }
                        break;
                    case PlayoffRuond.SemiFinals:
                        if (Region.East == region)
                        {
                            setGame(m_eastSemiFinal[index], scoreTeam1, scoreTeam2);
                        }
                        else if (Region.West == region)
                        {
                            setGame(m_westSemiFinal[index], scoreTeam1, scoreTeam2);
                        }
                        break;
                    case PlayoffRuond.RegionalFinals:
                        if (Region.East == region)
                        {
                            setGame(m_eastFinals, scoreTeam1, scoreTeam2);
                        }
                        else if (Region.West == region)
                        {
                            setGame(m_westFinals, scoreTeam1, scoreTeam2);
                        }
                        break;
                    case PlayoffRuond.NbaFinals:
                        setGame(m_nbaFinals, scoreTeam1, scoreTeam2);
                        checkForChampions();
                        break;
                }
            }
        }

        private void setGame(Game game, int score1, int score2)
        {
            log.InfoFormat("Seting Score for Game score team1: {0}  score team2: {1}", score1, score2);
            game.SetScore(score1, score2);
            log.InfoFormat("The game after seting: {0}: {1} vs {2}: {3}", game.Team1.Name, game.Team1Score, game.Team2.Name, game.Team2Score);
        }

        private void checkForChampions()
        {
            if (m_nbaFinals.WinnerTeam != null)
            {
                log.InfoFormat("The NBA Champions are the {0}", m_nbaChampion.Name);
                m_nbaChampion = m_nbaFinals.WinnerTeam;
            }
        }

        public Team[] East { get { return m_east; } }
        public Team[] West { get { return m_west; } }
        public Game[] EastQuarter { get { return m_eastQuarter; } }
        public Game[] WestQuarter { get { return m_westQuarter; } }
        public Game[] EastSemiFinals { get { return m_eastSemiFinal; } }
        public Game[] WestSemiFinals { get { return m_westSemiFinal; } }
        public Game EastFinals { get { return m_eastFinals; } }
        public Game WestFinals { get { return m_westFinals; } }
        public Game NbaFinals { get { return m_nbaFinals; } }
        public Team NbaChampion { get { return m_nbaChampion; } }
    }
}
