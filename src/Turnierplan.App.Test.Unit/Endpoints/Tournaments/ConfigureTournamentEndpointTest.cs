using FluentValidation;
using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Tournaments;
using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Test.Unit.Endpoints.Tournaments;

public sealed class ConfigureTournamentEndpointTest
{
    private static readonly ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[] __validGroups =
    [
        new()
        {
            Id = null,
            AlphabeticalId = 'A',
            DisplayName = null,
            Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
            {
                new()
                {
                    Id = null,
                    Name = "Test 1"
                },
                new()
                {
                    Id = null,
                    Name = "Test 2"
                }
            }
        }
    ];

    private static readonly ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[] __validGroupsWith6Teams =
    [
        new()
        {
            Id = null,
            AlphabeticalId = 'A',
            DisplayName = null,
            Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
            {
                new()
                {
                    Id = null,
                    Name = "Test 1"
                },
                new()
                {
                    Id = null,
                    Name = "Test 2"
                },
                new()
                {
                    Id = null,
                    Name = "Test 3"
                },
                new()
                {
                    Id = null,
                    Name = "Test 4"
                },
                new()
                {
                    Id = null,
                    Name = "Test 5"
                },
                new()
                {
                    Id = null,
                    Name = "Test 6"
                }
            }
        }
    ];

    private static readonly ScheduleConfigurationDto __validSchedule = new() { PlayTime = 10.Minutes(), PauseTime = 2.Minutes() };
    private static readonly GroupPhaseConfigurationDto __groupPhase = new() { Schedule = null, NumberOfCourts = 1, UseAlternatingOrder = false, NumberOfGroupRounds = 1 };
    private static readonly FinalsPhaseConfigurationDto __finalsPhase = new() { Schedule = null, NumberOfCourts = 1, FirstFinalsRound = (int)FinalsRoundOrder.SemiFinals, ThirdPlacePlayoff = true, AdditionalPlayoffs = [] };

    public ConfigureTournamentEndpointTest()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___Without_Groups___Is_Invalid()
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = [],
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Groups)
            .WithErrorMessage("Configuration must include at least one group.")
            .Only();
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Non_Unique_Alphabetical_Ids___Is_Invalid()
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = null,
                AlphabeticalId = 'A',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 2"
                    }
                }
            },
            new()
            {
                Id = null,
                AlphabeticalId = 'A',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 3"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 4"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Groups)
            .WithErrorMessage("Group alphabetical IDs must be unique.")
            .Only();
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Non_Unique_Existing_Group_Ids___Is_Invalid()
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = 1,
                AlphabeticalId = 'A',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 2"
                    }
                }
            },
            new()
            {
                Id = 1,
                AlphabeticalId = 'B',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 3"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 4"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Groups)
            .WithErrorMessage("Configuration may not contain more than 1 entry per existing group ID.")
            .Only();
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Non_Unique_Existing_Team_Ids___Is_Invalid()
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = 1,
                AlphabeticalId = 'A',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = 1,
                        Name = "Test 2"
                    }
                }
            },
            new()
            {
                Id = 2,
                AlphabeticalId = 'B',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = 1,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = 3,
                        Name = "Test 2"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Groups)
            .WithErrorMessage("Configuration may not contain more than 1 entry per existing team ID.")
            .Only();
    }

    [Theory]
    [InlineData('@')] // @=64, A=65
    [InlineData('[')] // [=91, Z=90
    [InlineData('a')] // must be upper case
    [InlineData('z')] // must be upper case
    public void ConfigureTournamentEndpointRequest___With_Invalid_Group_Alphabetical_Id___Is_Invalid(char alphabeticalId)
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = null,
                AlphabeticalId = alphabeticalId,
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 2"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Groups[0].AlphabeticalId")
            .WithErrorMessage("Group alphabetical ID must be between 'A' and 'Z'.")
            .Only();
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "")]
    [InlineData(false, " ")]
    [InlineData(true, "Test group 123")]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Group_DisplayName___Is_Invalid(bool isValid, string? displayName)
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = null,
                AlphabeticalId = 'A',
                DisplayName = displayName,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = "Test 1"
                    },
                    new()
                    {
                        Id = null,
                        Name = "Test 2"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor("Groups[0].DisplayName")
                .WithErrorMessage("Group display name must be null or a non-empty string.")
                .Only();
        }
    }

    [Theory]
    [InlineData(false, 0, 10, 9)]
    [InlineData(false, 0, 10, 4, 4)]
    [InlineData(false, 1, 4, 10, 4)]
    [InlineData(false, 2, 4, 4, 0)]
    [InlineData(false, 2, 4, 4, 1)]
    [InlineData(true, null, 9, 9, 9)]
    [InlineData(true, null, 4, 4)]
    public void ConfigureTournamentEndpointRequest___With_Too_Many_Teams_Per_Group___Is_Invalid(bool isValid, int? invalidGroupIndex, params int[] teamsPerGroup)
    {
        var groups = teamsPerGroup.Select((x, index) =>
        {
            return new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry()
            {
                Id = null,
                AlphabeticalId = (char)('A' + index),
                DisplayName = null,
                Teams = Enumerable.Range(0, x).Select(_ => new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry
                {
                    Id = null,
                    Name = "Test"
                }).ToArray()
            };
        }).ToArray();

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            var message = teamsPerGroup[invalidGroupIndex!.Value] <= 1
                ? "Each group must contain at least 2 teams."
                : "Each group must contain at most 9 teams.";

            result.ShouldHaveValidationErrorFor($"Groups[{invalidGroupIndex}].Teams")
                .WithErrorMessage(message)
                .Only();
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Team_Name___Is_Invalid(string teamName)
    {
        var groups = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestGroupEntry[]
        {
            new()
            {
                Id = null,
                AlphabeticalId = 'A',
                DisplayName = null,
                Teams = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequestTeamEntry[]
                {
                    new()
                    {
                        Id = null,
                        Name = teamName
                    },
                    new()
                    {
                        Id = null,
                        Name = "Other"
                    }
                }
            }
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = groups,
            FirstMatchKickoff = null,
            GroupPhase = __groupPhase,
            FinalsPhase = __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Groups[0].Teams[0].Name")
            .WithErrorMessage("Team name must be a non-empty string.")
            .Only();
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Neither_Group_Nor_Final_Phase_Config___Is_Invalid()
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = null,
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(string.Empty)
            .WithErrorMessage("Either group phase, finals phase, or both must be specified.")
            .Only();
    }

    [Theory]
    [InlineData(false, -1)]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(true, 3)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Number_Of_Group_Phase_Courts___Is_Invalid(bool isValid, short numberOfGroupPhaseCourts)
    {
        var groupPhaseConfig = new GroupPhaseConfigurationDto { Schedule = null, NumberOfCourts = numberOfGroupPhaseCourts, UseAlternatingOrder = true, NumberOfGroupRounds = 1 };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = groupPhaseConfig,
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.NumberOfCourts)
                .WithErrorMessage("Number of courts for group phase must be at least 1.")
                .Only();
        }
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(true, 4)]
    [InlineData(true, 8)]
    [InlineData(false, 9)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Number_Of_Group_Phase_Rounds___Is_Invalid(bool isValid, int numberOfGroupPhaseRounds)
    {
        var groupPhaseConfig = new GroupPhaseConfigurationDto { Schedule = null, NumberOfCourts = 1, UseAlternatingOrder = true, NumberOfGroupRounds = numberOfGroupPhaseRounds };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = groupPhaseConfig,
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.NumberOfGroupRounds)
                .WithErrorMessage("Number of group phase rounds must be between 1 and 8.")
                .Only();
        }
    }

    [Theory]
    [InlineData(false, -1)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(true, 3)]
    [InlineData(false, 4)]
    [InlineData(false, 5)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_First_Final_Round___Is_Invalid(bool isValid, int firstFinalRound)
    {
        var finalsPhaseConfig = new FinalsPhaseConfigurationDto { Schedule = null, NumberOfCourts = 1, FirstFinalsRound = firstFinalRound, ThirdPlacePlayoff = false, AdditionalPlayoffs = [] };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = null,
            FinalsPhase = finalsPhaseConfig
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.FirstFinalsRound)
                .WithErrorMessage("First finals round must be one of the following: 0, 1, 2, 3")
                .Only();
        }
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void ConfigureTournamentEndpointRequest___With_Third_Place_Playoff_And_Only_Final___Is_Invalid(bool isValid, int firstFinalRound)
    {
        var finalsPhaseConfig = new FinalsPhaseConfigurationDto { Schedule = null, NumberOfCourts = 1, FirstFinalsRound = firstFinalRound, ThirdPlacePlayoff = true, AdditionalPlayoffs = [] };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = null,
            FinalsPhase = finalsPhaseConfig
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.ThirdPlacePlayoff)
                .WithErrorMessage("Third place playoff must be disabled if first finals round is 'final only'.")
                .Only();
        }
    }

    [Theory]
    [InlineData(false, -1)]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(true, 3)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Number_Of_Finals_Phase_Courts___Is_Invalid(bool isValid, short numberOfFinalsPhaseCourts)
    {
        var finalsPhaseConfig = new FinalsPhaseConfigurationDto { Schedule = null, NumberOfCourts = numberOfFinalsPhaseCourts, FirstFinalsRound = (int)FinalsRoundOrder.SemiFinals, ThirdPlacePlayoff = true, AdditionalPlayoffs = [] };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = null,
            GroupPhase = null,
            FinalsPhase = finalsPhaseConfig
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.NumberOfCourts)
                .WithErrorMessage("Number of courts for finals phase must be at least 1.")
                .Only();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureTournamentEndpointRequest___With_Group_Phase_Schedule_But_No_Kickoff_Time___Is_Invalid(bool setValidKickoffTime)
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = setValidKickoffTime ? DateTime.Now : null,
            GroupPhase = __groupPhase with { Schedule = __validSchedule },
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (setValidKickoffTime)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.Schedule)
                .WithErrorMessage("Schedule for group phase not allowed when FirstMatchKickoff is null.")
                .Only();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureTournamentEndpointRequest___With_Finals_Phase_Schedule_But_No_Kickoff_Time___Is_Invalid(bool setValidKickoffTime)
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = setValidKickoffTime ? DateTime.Now : null,
            GroupPhase = null,
            FinalsPhase = __finalsPhase with { Schedule = __validSchedule }
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (setValidKickoffTime)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.Schedule)
                .WithErrorMessage("Schedule for finals phase not allowed when FirstMatchKickoff is null.")
                .Only();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureTournamentEndpointRequest___With_No_Group_Phase_Schedule_But_Kickoff_Time___Is_Invalid(bool setGroupPhaseSchedule)
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = setGroupPhaseSchedule ? __groupPhase with { Schedule = __validSchedule } : __groupPhase,
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (setGroupPhaseSchedule)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.Schedule)
                .WithErrorMessage("Schedule for group phase must be specified when FirstMatchKickoff is specified.")
                .Only();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureTournamentEndpointRequest___With_No_Finals_Phase_Schedule_But_Kickoff_Time___Is_Invalid(bool setFinalsPhaseSchedule)
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = null,
            FinalsPhase = setFinalsPhaseSchedule ? __finalsPhase with { Schedule = __validSchedule } : __finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (setFinalsPhaseSchedule)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.Schedule)
                .WithErrorMessage("Schedule for finals phase must be specified when FirstMatchKickoff is specified.")
                .Only();
        }
    }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(10, -2)]
    [InlineData(0, 2)]
    [InlineData(-10, 2)]
    [InlineData(-10, -2)]
    [InlineData(0, -2)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Group_Phase_Schedule___Is_Invalid(int playTimeMinutes, int pauseTimeMinutes)
    {
        var schedule = new ScheduleConfigurationDto { PlayTime = playTimeMinutes.Minutes(), PauseTime = pauseTimeMinutes.Minutes() };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = __groupPhase with { Schedule = schedule },
            FinalsPhase = null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (playTimeMinutes > 0 && pauseTimeMinutes >= 0)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            var expectedErrorCount = 0;

            if (playTimeMinutes <= 0)
            {
                expectedErrorCount++;
                result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.Schedule!.PlayTime)
                    .WithErrorMessage("Group phase play time must be greater than zero.");
            }

            if (pauseTimeMinutes < 0)
            {
                expectedErrorCount++;
                result.ShouldHaveValidationErrorFor(x => x.GroupPhase!.Schedule!.PauseTime)
                    .WithErrorMessage("Group phase pause time must be greater than or equal to zero.");
            }

            result.Errors.Count.Should().Be(expectedErrorCount);
        }
    }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(10, -2)]
    [InlineData(0, 2)]
    [InlineData(-10, 2)]
    [InlineData(-10, -2)]
    [InlineData(0, -2)]
    public void ConfigureTournamentEndpointRequest___With_Invalid_Finals_Phase_Schedule___Is_Invalid(int playTimeMinutes, int pauseTimeMinutes)
    {
        var schedule = new ScheduleConfigurationDto { PlayTime = playTimeMinutes.Minutes(), PauseTime = pauseTimeMinutes.Minutes() };
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = null,
            FinalsPhase = __finalsPhase with { Schedule = schedule }
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (playTimeMinutes > 0 && pauseTimeMinutes >= 0)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            var expectedErrorCount = 0;

            if (playTimeMinutes <= 0)
            {
                expectedErrorCount++;
                result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.Schedule!.PlayTime)
                    .WithErrorMessage("Finals phase play time must be greater than zero.");
            }

            if (pauseTimeMinutes < 0)
            {
                expectedErrorCount++;
                result.ShouldHaveValidationErrorFor(x => x.FinalsPhase!.Schedule!.PauseTime)
                    .WithErrorMessage("Finals phase pause time must be greater than or equal to zero.");
            }

            result.Errors.Count.Should().Be(expectedErrorCount);
        }
    }

    [Theory]
    [InlineData(1, FinalsRoundOrder.FinalOnly, false)]
    [InlineData(2, FinalsRoundOrder.FinalOnly, false)]
    [InlineData(3, FinalsRoundOrder.FinalOnly, true)]
    [InlineData(4, FinalsRoundOrder.FinalOnly, false)]
    [InlineData(5, FinalsRoundOrder.FinalOnly, true)]
    [InlineData(1, FinalsRoundOrder.SemiFinals, false)]
    [InlineData(2, FinalsRoundOrder.SemiFinals, false)]
    [InlineData(3, FinalsRoundOrder.SemiFinals, true)]
    [InlineData(4, FinalsRoundOrder.SemiFinals, false)]
    [InlineData(5, FinalsRoundOrder.SemiFinals, true)]
    public void ConfigureTournamentEndpointRequest___With_Additional_Playoffs_And_Conflicting_Final_Round___Is_Invalid(int playoffPosition, FinalsRoundOrder firstFinalRound, bool expectValid)
    {
        var finalsPhase = new FinalsPhaseConfigurationDto
        {
            Schedule = __validSchedule,
            NumberOfCourts = 1,
            FirstFinalsRound = (int)firstFinalRound,
            ThirdPlacePlayoff = false,
            AdditionalPlayoffs =
            [
                new AdditionalPlayoffDto
                {
                    PlayoffPosition = playoffPosition,
                    TeamSelectorA = "1.0",
                    TeamSelectorB = "1.1"
                }
            ]
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroupsWith6Teams,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = null,
            FinalsPhase = finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (expectValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor("FinalsPhase.AdditionalPlayoffs[0]")
                .WithErrorMessage("Additional playoff positions must be >= 3 and odd if first finals round is 'final only'.")
                .Only();
        }
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(3, false)]
    [InlineData(4, false)]
    [InlineData(5, true)]
    public void ConfigureTournamentEndpointRequest___With_Additional_Playoffs_And_Conflicting_Final_Round_With_Default_Third_Place_Playoff___Is_Invalid(int playoffPosition, bool expectValid)
    {
        var finalsPhase = new FinalsPhaseConfigurationDto
        {
            Schedule = __validSchedule,
            NumberOfCourts = 1,
            FirstFinalsRound = (int)FinalsRoundOrder.SemiFinals,
            ThirdPlacePlayoff = true,
            AdditionalPlayoffs =
            [
                new AdditionalPlayoffDto
                {
                    PlayoffPosition = playoffPosition,
                    TeamSelectorA = "1.0",
                    TeamSelectorB = "1.1"
                }
            ]
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroupsWith6Teams,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = null,
            FinalsPhase = finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (expectValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor("FinalsPhase.AdditionalPlayoffs[0]")
                .WithErrorMessage("Additional playoff positions must be >= 5 and odd if the default third place playoff is enabled.")
                .Only();
        }
    }

    [Theory]
    [InlineData("1.0", true)]
    [InlineData("0B1", true)]
    [InlineData("", false)]
    [InlineData("111111111", false)]
    [InlineData("1C5", false)]
    [InlineData(".12", false)]
    public void ConfigureTournamentEndpointRequest___With_Additional_Playoffs_With_Invalid_Abstract_Team_Selectors___Is_Invalid(string teamSelector, bool expectValid)
    {
        foreach (var flip in new[]{true, false})
        {
            var finalsPhase = new FinalsPhaseConfigurationDto
            {
                Schedule = __validSchedule,
                NumberOfCourts = 1,
                FirstFinalsRound = (int)FinalsRoundOrder.FinalOnly,
                ThirdPlacePlayoff = false,
                AdditionalPlayoffs =
                [
                    new AdditionalPlayoffDto
                    {
                        PlayoffPosition = 3,
                        TeamSelectorA = flip ? "1.0" : teamSelector,
                        TeamSelectorB = flip ? teamSelector : "1.0"
                    }
                ]
            };

            var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
            {
                Groups = __validGroupsWith6Teams,
                FirstMatchKickoff = DateTime.Now,
                GroupPhase = null,
                FinalsPhase = finalsPhase
            };

            var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

            if (expectValid)
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
            else
            {
                result.ShouldHaveValidationErrorFor(flip ? "FinalsPhase.AdditionalPlayoffs[0].TeamSelectorB" : "FinalsPhase.AdditionalPlayoffs[0].TeamSelectorA")
                    .WithErrorMessage("Additional playoff definition must contain only valid team selectors.")
                    .Only();
            }
        }
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(5, true)]
    [InlineData(7, false)]
    [InlineData(9, false)]
    public void ConfigureTournamentEndpointRequest___With_Additional_Playoff_With_Too_High_Playoff_Position___Is_Invalid(int rankingPosition, bool expectValid)
    {
        var finalsPhase = new FinalsPhaseConfigurationDto
        {
            Schedule = __validSchedule,
            NumberOfCourts = 1,
            FirstFinalsRound = (int)FinalsRoundOrder.FinalOnly,
            ThirdPlacePlayoff = false,
            AdditionalPlayoffs =
            [
                new AdditionalPlayoffDto
                {
                    PlayoffPosition = rankingPosition,
                    TeamSelectorA = "1.0",
                    TeamSelectorB = "1.1"
                }
            ]
        };

        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroupsWith6Teams,
            FirstMatchKickoff = DateTime.Now,
            GroupPhase = null,
            FinalsPhase = finalsPhase
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        if (expectValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(string.Empty)
                .WithErrorMessage("The maximum additional playoff position must be 1 less than the number of teams.")
                .Only();
        }
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Pause_Between_Group_And_Finals_Phase_And_Both_Specified___Is_Valid()
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            PauseBetweenGroupAndFinalsPhase = 10.Minutes(),
            GroupPhase = __groupPhase with { Schedule = __validSchedule },
            FinalsPhase = __finalsPhase with { Schedule = __validSchedule }
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ConfigureTournamentEndpointRequest___With_Pause_Between_Group_And_Finals_Phase_And_Not_Both_Specified___Is_Invalid(bool hasGroupPhase, bool hasFinalsPhase)
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            PauseBetweenGroupAndFinalsPhase = 10.Minutes(),
            GroupPhase = hasGroupPhase ? __groupPhase with { Schedule = __validSchedule } : null,
            FinalsPhase = hasFinalsPhase ? __finalsPhase with { Schedule = __validSchedule } : null
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PauseBetweenGroupAndFinalsPhase)
            .WithErrorMessage("Pause between group and finals phase must be null if group and finals phase are not both specified.")
            .Only();
    }

    [Fact]
    public void ConfigureTournamentEndpointRequest___With_Negative_Pause_Between_Group_And_Finals_Phase___Is_Invalid()
    {
        var command = new ConfigureTournamentEndpoint.ConfigureTournamentEndpointRequest
        {
            Groups = __validGroups,
            FirstMatchKickoff = DateTime.Now,
            PauseBetweenGroupAndFinalsPhase = -5.Minutes(),
            GroupPhase = __groupPhase with { Schedule = __validSchedule },
            FinalsPhase = __finalsPhase with { Schedule = __validSchedule }
        };

        var result = ConfigureTournamentEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PauseBetweenGroupAndFinalsPhase)
            .WithErrorMessage("Pause between group and finals phase must be greater than or equal to zero if specified.")
            .Only();
    }
}
