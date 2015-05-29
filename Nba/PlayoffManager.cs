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

        public void ExportPalyoff()
        {
            xmlWriter = XmlWriter.Create("NBA.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Playoff");

            xmlWriter.WriteStartElement("East");
            writeFirstRound(Region.East);
            writeSemiFinals(Region.East);
            writeregionlFinals(Region.East);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("West");
            writeFirstRound(Region.West);
            writeSemiFinals(Region.West);
            writeregionlFinals(Region.West);
            xmlWriter.WriteEndElement();

            writeNbaFinals();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void writeNbaFinals()
        {
            string scoreString;
            xmlWriter.WriteStartElement("NBAFinals");
            scoreString = m_nbaFinals.Team1Score.ToString();
            xmlWriter.WriteStartElement("Game");
            xmlWriter.WriteStartElement("Team");
            xmlWriter.WriteAttributeString("Name", m_nbaFinals.Team1.Name);
            xmlWriter.WriteEndElement();

            scoreString = m_nbaFinals.Team2Score.ToString();
            xmlWriter.WriteStartElement("Team");
            xmlWriter.WriteAttributeString("Name", m_nbaFinals.Team2.Name);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Winner");
            xmlWriter.WriteAttributeString("Name", m_nbaFinals.WinnerTeam.Name);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();




        }

        private void writeregionlFinals(Region region)
        {
            string scoreString;
            if (region == Region.East)
            {
                xmlWriter.WriteStartElement("regionlFinals");
                scoreString = m_eastFinals.Team1Score.ToString();
                xmlWriter.WriteStartElement("Game");
                xmlWriter.WriteStartElement("Team");
                xmlWriter.WriteAttributeString("Name", m_eastFinals.Team1.Name);
                xmlWriter.WriteAttributeString("Score", scoreString);
                xmlWriter.WriteEndElement();

                scoreString = m_eastFinals.Team2Score.ToString();
                xmlWriter.WriteStartElement("Team");
                xmlWriter.WriteAttributeString("Name", m_eastFinals.Team2.Name);
                xmlWriter.WriteAttributeString("Score", scoreString);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Winner");
                xmlWriter.WriteAttributeString("Name", m_eastFinals.WinnerTeam.Name);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
            else if (region == Region.West)
            {
                xmlWriter.WriteStartElement("regionlFinals");
                scoreString = m_westFinals.Team1Score.ToString();
                xmlWriter.WriteStartElement("Game");
                xmlWriter.WriteStartElement("Team");
                xmlWriter.WriteAttributeString("Name", m_westFinals.Team1.Name);
                xmlWriter.WriteAttributeString("Score", scoreString);
                xmlWriter.WriteEndElement();

                scoreString = m_westFinals.Team2Score.ToString();
                xmlWriter.WriteStartElement("Team");
                xmlWriter.WriteAttributeString("Name", m_westFinals.Team2.Name);
                xmlWriter.WriteAttributeString("Score", scoreString);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Winner");
                xmlWriter.WriteAttributeString("Name", m_eastFinals.WinnerTeam.Name);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }

        private void writeSemiFinals(Region region)
        {
            string scoreString;
            if (region == Region.East)
            {
                xmlWriter.WriteStartElement("SemiFinals");
                for (int i = 0; i < m_eastSemiFinal.Length; i++)
                {
                    //for Log writing the first round
                    scoreString = m_eastSemiFinal[i].Team1Score.ToString();
                    xmlWriter.WriteStartElement("Game");
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_eastSemiFinal[i].Team1.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    scoreString = m_eastSemiFinal[i].Team2Score.ToString();
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_eastSemiFinal[i].Team2.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Winner");
                    xmlWriter.WriteAttributeString("Name", m_eastSemiFinal[i].WinnerTeam.Name);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            else if (region == Region.West)
            {
                xmlWriter.WriteStartElement("SemiFinals");
                for (int i = 0; i < m_westSemiFinal.Length; i++)
                {
                    //for Log writing the first round
                    scoreString = m_westSemiFinal[i].Team1Score.ToString();
                    xmlWriter.WriteStartElement("Game");
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_westSemiFinal[i].Team1.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    scoreString = m_westSemiFinal[i].Team2Score.ToString();
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_westSemiFinal[i].Team2.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Winner");
                    xmlWriter.WriteAttributeString("Name", m_westSemiFinal[i].WinnerTeam.Name);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }

        private void writeFirstRound(Region region)
        {
            string scoreString;
            if (region == Region.East)
            {
                xmlWriter.WriteStartElement("FirstRound");
                for (int i = 0; i < m_eastQuarter.Length; i++)
                {
                    //for Log writing the first round
                    scoreString = m_eastQuarter[i].Team1Score.ToString();
                    xmlWriter.WriteStartElement("Game");
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_eastQuarter[i].Team1.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    scoreString = m_eastQuarter[i].Team2Score.ToString();
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_eastQuarter[i].Team2.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Winner");
                    xmlWriter.WriteAttributeString("Name", m_eastQuarter[i].WinnerTeam.Name);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            else if (region == Region.West)
            {
                xmlWriter.WriteStartElement("FirstRound");
                for (int i = 0; i < m_westQuarter.Length; i++)
                {
                    //for Log writing the first round
                    scoreString = m_westQuarter[i].Team1Score.ToString();
                    xmlWriter.WriteStartElement("Game");
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_westQuarter[i].Team1.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    scoreString = m_westQuarter[i].Team2Score.ToString();
                    xmlWriter.WriteStartElement("Team");
                    xmlWriter.WriteAttributeString("Name", m_westQuarter[i].Team2.Name);
                    xmlWriter.WriteAttributeString("Score", scoreString);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Winner");
                    xmlWriter.WriteAttributeString("Name", m_westQuarter[i].WinnerTeam.Name);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
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
        //Add here property that gets the nba Champion
    }
}
