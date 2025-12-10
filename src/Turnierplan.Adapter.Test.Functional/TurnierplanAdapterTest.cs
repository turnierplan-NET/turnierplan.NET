using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnierplan.Adapter.Enums;
using Turnierplan.Adapter.Models;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Dal;
using MatchType = Turnierplan.Adapter.Enums.MatchType;

namespace Turnierplan.Adapter.Test.Functional;

public sealed class TurnierplanAdapterTest
{
    [Fact]
    public async Task Turnierplan_Client_Works_As_Expected_With_Test_Server()
    {
        var server = new TestServer();
        SeedingResult seedingResult;

        await using (var scope = server.Services.CreateAsyncScope())
        {
            seedingResult = await SeedDatabaseAsync(
                scope.ServiceProvider.GetRequiredService<TurnierplanContext>(),
                scope.ServiceProvider.GetRequiredService<IPasswordHasher<ApiKey>>());
        }

        var options = new TurnierplanClientOptions(
            server.ClientOptions.BaseAddress.ToString(),
            seedingResult.ApiKeyId,
            seedingResult.ApiKeySecret);

        var client = new TurnierplanClient(server.CreateClient(), options);

        var tournaments = await client.GetTournaments(seedingResult.FolderId);

        tournaments.Should().BeEquivalentTo([
            new TournamentHeader
            {
                Id = seedingResult.Tournament1Id,
                Name = "T1",
                OrganizationName = "TestOrg",
                FolderName = "TestFolder",
                Visibility = Visibility.Private
            },
            new TournamentHeader
            {
                Id = seedingResult.Tournament2Id,
                Name = "T2",
                OrganizationName = "TestOrg",
                FolderName = "TestFolder",
                Visibility = Visibility.Public
            }
        ]);

        var tournament1 = await client.GetTournament(seedingResult.Tournament1Id);
        var tournament2 = await client.GetTournament(seedingResult.Tournament2Id);

        tournament1.Should().BeEquivalentTo(new Tournament
        {
            Id = seedingResult.Tournament1Id,
            Name = "T1",
            OrganizationName = "TestOrg",
            FolderName = "TestFolder",
            VenueName = null,
            Visibility = Visibility.Private,
            PublicPageViews = 0,
            Teams = [],
            Groups = [],
            Matches = [],
            Rankings = []
        });

        tournament2.Should().BeEquivalentTo(new Tournament
        {
            Id = seedingResult.Tournament2Id,
            Name = "T2",
            OrganizationName = "TestOrg",
            FolderName = "TestFolder",
            VenueName = null,
            Visibility = Visibility.Public,
            PublicPageViews = 3,
            Teams = [
                new Team
                {
                    Id = 1,
                    Name = "Team 1",
                    OutOfCompetition = false,
                    Statistics = new TeamStatistics
                    {
                        ScoreFor = 2,
                        ScoreAgainst = 3,
                        MatchesPlayed = 1,
                        MatchesWon = 0,
                        MatchesDrawn = 0,
                        MatchesLost = 1
                    }
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2",
                    OutOfCompetition = false,
                    Statistics = new TeamStatistics
                    {
                        ScoreFor = 3,
                        ScoreAgainst = 2,
                        MatchesPlayed = 1,
                        MatchesWon = 1,
                        MatchesDrawn = 0,
                        MatchesLost = 0
                    }
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3",
                    OutOfCompetition = false,
                    Statistics = new TeamStatistics
                    {
                        ScoreFor = 0,
                        ScoreAgainst = 0,
                        MatchesPlayed = 0,
                        MatchesWon = 0,
                        MatchesDrawn = 0,
                        MatchesLost = 0
                    }
                }
            ],
            Groups = [
                new Group
                {
                    Id = 4,
                    AlphabeticalId = 'A',
                    DisplayName = "Gruppe A",
                    HasCustomDisplayName = false,
                    Participants =
                    [
                        new GroupParticipant
                        {
                            TeamId = 1,
                            Priority = 0,
                            Statistics = new TeamGroupStatistics
                            {
                                Position = 3,
                                ScoreFor = 2,
                                ScoreAgainst = 3,
                                ScoreDifference = -1,
                                MatchesPlayed = 1,
                                MatchesWon = 0,
                                MatchesDrawn = 0,
                                MatchesLost = 1,
                                Points = 0
                            }
                        },
                        new GroupParticipant
                        {
                            TeamId = 2,
                            Priority = 0,
                            Statistics = new TeamGroupStatistics
                            {
                                Position = 1,
                                ScoreFor = 3,
                                ScoreAgainst = 2,
                                ScoreDifference = 1,
                                MatchesPlayed = 1,
                                MatchesWon = 1,
                                MatchesDrawn = 0,
                                MatchesLost = 0,
                                Points = 3
                            }
                        },
                        new GroupParticipant
                        {
                            TeamId = 3,
                            Priority = 2,
                            Statistics = new TeamGroupStatistics
                            {
                                Position = 2,
                                ScoreFor = 0,
                                ScoreAgainst = 0,
                                ScoreDifference = 0,
                                MatchesPlayed = 0,
                                MatchesWon = 0,
                                MatchesDrawn = 0,
                                MatchesLost = 0,
                                Points = 0
                            }
                        }
                    ]
                }
            ],
            Matches = [
                new Match
                {
                    Id = 5,
                    Index = 1,
                    Court = 0,
                    Kickoff = null,
                    Type = MatchType.GroupMatch,
                    FormattedType = "Gruppenspiel",
                    GroupId = 4,
                    TeamA = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/0",
                            Localized = "Mannschaft 1, Gruppe A"
                        },
                        TeamId = 1,
                        Score = 2
                    },
                    TeamB = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/1",
                            Localized = "Mannschaft 2, Gruppe A"
                        },
                        TeamId = 2,
                        Score = 3
                    },
                    State = MatchState.Finished,
                    OutcomeType = MatchOutcomeType.Standard
                },
                new Match
                {
                    Id = 6,
                    Index = 2,
                    Court = 0,
                    Kickoff = null,
                    Type = MatchType.GroupMatch,
                    FormattedType = "Gruppenspiel",
                    GroupId = 4,
                    TeamA = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/1",
                            Localized = "Mannschaft 2, Gruppe A"
                        },
                        TeamId = 2,
                        Score = 1
                    },
                    TeamB = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/2",
                            Localized = "Mannschaft 3, Gruppe A"
                        },
                        TeamId = 3,
                        Score = 1
                    },
                    State = MatchState.CurrentlyPlaying,
                    OutcomeType = MatchOutcomeType.AfterOvertime
                },
                new Match
                {
                    Id = 7,
                    Index = 3,
                    Court = 0,
                    Kickoff = null,
                    Type = MatchType.GroupMatch,
                    FormattedType = "Gruppenspiel",
                    GroupId = 4,
                    TeamA = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/2",
                            Localized = "Mannschaft 3, Gruppe A"
                        },
                        TeamId = 3,
                        Score = null
                    },
                    TeamB = new MatchTeamInfo
                    {
                        TeamSelector = new TeamSelector
                        {
                            Key = "G4/0",
                            Localized = "Mannschaft 1, Gruppe A"
                        },
                        TeamId = 1,
                        Score = null
                    },
                    State = MatchState.NotStarted,
                    OutcomeType = null
                }
            ],
            Rankings = [
                new Ranking
                {
                    PlacementRank = 1,
                    IsDefined = false,
                    TeamId = null
                },
                new Ranking
                {
                    PlacementRank = 2,
                    IsDefined = false,
                    TeamId = null
                },
                new Ranking
                {
                    PlacementRank = 3,
                    IsDefined = false,
                    TeamId = null
                }
            ]
        });
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Turnierplan_Client_Throws_Exception_When_Version_Does_Not_Match(bool sendHeader)
    {
        using var httpClient = new HttpClient(new MockHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Unauthorized;

            if (sendHeader)
            {
                response.Headers.Add("x-turnierplan-version", "2024.0.0"); // old version that does not exist
            }

            return response;
        }));

        var options = new TurnierplanClientOptions(new Uri("http://localhost"), "_", "_");
        var client = new TurnierplanClient(httpClient, options);

        var action = async () =>
        {
            _ = await client.GetTournament("x");
        };

        var actualVersion = typeof(TurnierplanClient).Assembly.GetName().Version!.ToString();
        var expectedMessage = sendHeader
            ? $"Server version '2024.0.0' does not match the Turnierplan.Adapter version '{actualVersion}'."
            : "Could not get 'X-Turnierplan-Version' header from response.";

        await action.Should()
            .ThrowAsync<TurnierplanClientException>()
            .WithMessage(expectedMessage);
    }

    private static async Task<SeedingResult> SeedDatabaseAsync(TurnierplanContext context, IPasswordHasher<ApiKey> secretHasher)
    {
        var organization = new Organization("TestOrg");

        var folder = new Folder(organization, "TestFolder");

        var tournament1 = new Turnierplan.Core.Tournament.Tournament(organization, "T1", Core.Tournament.Visibility.Private);
        var tournament2 = new Turnierplan.Core.Tournament.Tournament(organization, "T2", Core.Tournament.Visibility.Public);

        tournament1.SetFolder(folder);
        tournament2.SetFolder(folder);

        {
            var team1 = tournament2.AddTeam("Team 1");
            var team2 = tournament2.AddTeam("Team 2");
            var team3 = tournament2.AddTeam("Team 3");

            var group = tournament2.AddGroup('A');

            tournament2.AddGroupParticipant(group, team1);
            tournament2.AddGroupParticipant(group, team2);
            tournament2.AddGroupParticipant(group, team3, 2);

            tournament2.GenerateMatchPlan(new Turnierplan.Core.Tournament.MatchPlanConfiguration
            {
                GroupRoundConfig = new Turnierplan.Core.Tournament.GroupRoundConfig
                {
                    GroupMatchOrder = Core.Tournament.GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null,
                ScheduleConfig = null
            });

            tournament2.Matches[0].SetOutcome(false, 2, 3, Core.Tournament.MatchOutcomeType.Standard);
            tournament2.Matches[1].SetOutcome(true, 1, 1, Core.Tournament.MatchOutcomeType.AfterOvertime);

            tournament2.IncrementPublicPageViews();
            tournament2.IncrementPublicPageViews();
            tournament2.IncrementPublicPageViews();
        }

        var apiKey = new ApiKey(organization, "Test", null, DateTime.UtcNow + TimeSpan.FromDays(1));
        apiKey.AssignNewSecret(plainText => secretHasher.HashPassword(apiKey, plainText), out var secretPlainText);

        organization.AddRoleAssignment(Role.Owner, apiKey.AsPrincipal());

        context.Organizations.Add(organization);
        await context.SaveChangesAsync();

        return new SeedingResult(folder.PublicId.ToString(), tournament1.PublicId.ToString(), tournament2.PublicId.ToString(), apiKey.PublicId.ToString(), secretPlainText);
    }

    private sealed record SeedingResult(string FolderId, string Tournament1Id, string Tournament2Id, string ApiKeyId, string ApiKeySecret);

    private sealed class TestServer : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection([
                    new KeyValuePair<string, string?>("Database:InMemory", "true")
                ]);
            });
        }
    }

    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
