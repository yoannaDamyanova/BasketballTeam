using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;

namespace CreatingABasketballTeamJSON
{
    public class Program
    {
        static void Main(string[] args)
        {
            const int MinStatsEntryValue = 0;

            const int MaxStatsEntryValue = 100;

            string[] teamCommandsExampleJSONStringArray = new string[]
            {
                @"
                    [
                        ""Team;Sacramento Kings"",
                        ""Add;Sacramento Kings Roster;Sasha Vezenkov;75;85;84;92;67"",
                        ""Add;Sacramento Kings;Sasha Vezenkov;75;85;84;92;67"",
                        ""Add;Sacramento Kings;Jordan Ford;95;82;82;89;68"",
                        ""Add;Sacramento Kings;Sasha Vezenkov;75;85;84;92;67"",
                        ""Remove;Sacramento Kings;Jordan Ford"",
                        ""END""
                    ]
                ",

                @"
                    [
                        ""Add;Sacramento Kings;Sasha Vezenkov;;75;85;84;92;67"",
                        ""Team;Sacramento Kings"",
                        ""Add;Sacramento Kings;Sasha Vezenkov;;75;85;84;92;67"",
                        ""Add;Sacramento Kings;;Colby Jones;195;82;82;89;68"",
                        ""Remove;Sacramento Kings;Colby Jones"",
                        ""END""
                    ]
                ",

                @"
                    [
                        ""Team;Sacramento Kings"",
                        ""Team;Sacramento Kings"",
                        ""END""
                    ]
                "
            };

            foreach (string teamCommandsExampleJSONString in teamCommandsExampleJSONStringArray)
            {
                JArray deserializedTeamCommandsJArray = JArray.Parse(teamCommandsExampleJSONString);

                string[] teamCommands = deserializedTeamCommandsJArray
                    .Select(command => command.ToString())
                    .ToArray();

                Team team = null;

                TeamStats teamStats = new TeamStats();

                List<Team> teams = new List<Team>();

                foreach (var teamCommand in teamCommands)
                {
                    string[] teamCommandTokens = teamCommand.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


                    switch (teamCommandTokens[0])
                    {
                        case "Team":
                            string teamName = teamCommandTokens[1];

                            if (string.IsNullOrWhiteSpace(teamName))
                            {
                                teamStats.Errors.Add("The team name should not be empty");
                                continue;
                            }

                            if (teams.Any(t => t.Name == teamName))
                            {
                                teamStats.Errors.Add($"The team is already created as {teamName}.");
                                continue;
                            }

                            team = new(teamName);
                            teams.Add(team);
                            break;
                        case "Add":
                            if (team == null)
                            {
                                teamStats.Errors.Add("The team hasn't been created yet.");
                            }
                            else
                            {
                                string teamToAddPlayerName = teamCommandTokens[1];

                                if (team.Name != teamToAddPlayerName)
                                {
                                    teamStats.Errors.Add($"Team {teamToAddPlayerName} does not exist.");
                                }
                                else
                                {
                                    string playerName = teamCommandTokens[2];

                                    int endurance = int.Parse(teamCommandTokens[3]);
                                    int sprint = int.Parse(teamCommandTokens[4]);
                                    int dribble = int.Parse(teamCommandTokens[5]);
                                    int passes = int.Parse(teamCommandTokens[6]);
                                    int shooting = int.Parse(teamCommandTokens[7]);

                                    Stats playerStats = new(endurance, sprint, dribble, passes, shooting);

                                    Player playerToFind = team.Players.FirstOrDefault(p => p.Name == playerName);

                                    if (playerToFind != null)
                                    {
                                        teamStats.Errors.Add($"Player with name {playerName} is already in team {team.Name}");
                                        continue;
                                    }

                                    int playerEndurance = int.Parse(teamCommandTokens[3]);

                                    if (playerEndurance < MinStatsEntryValue || playerEndurance > MaxStatsEntryValue)
                                    {
                                        teamStats.Errors.Add($"Endurance should be between {MinStatsEntryValue} and {MaxStatsEntryValue}.");
                                        continue;
                                    }

                                    team.Players.Add(new Player(playerName, playerStats));
                                }
                            }
                            break;
                        case "Remove":
                            if (team == null)
                            {
                                teamStats.Errors.Add("The team hasn't been created yet.");
                            }
                            else
                            {
                                string teamToRemovePlayerName = teamCommandTokens[1];

                                if (team.Name != teamToRemovePlayerName)
                                {
                                    teamStats.Errors.Add($"Team {teamToRemovePlayerName} does not exist.");
                                }
                                else
                                {
                                    string playerName = teamCommandTokens[2];
                                    Player playerToRemove = team.Players.FirstOrDefault(p => p.Name == playerName);

                                    if (playerToRemove == null)
                                    {
                                        teamStats.Errors.Add($"Player {playerName} is not in the {teamToRemovePlayerName} team.");
                                    }
                                    else
                                    {
                                        team.Players.Remove(playerToRemove);
                                    }
                                }
                            }
                            break;
                        case "END":
                            if (team == null)
                            {
                                teamStats.Errors.Add("The team hasn't been created yet.");
                            }
                            else
                            {
                                teamStats.Team = team.Name;
                                teamStats.Average = team.Rating;
                                Console.WriteLine(JsonConvert.SerializeObject(teamStats, Formatting.Indented));
                            }
                            break;
                    }
                }
                teams = new List<Team>();
            }
        }
    }
}
