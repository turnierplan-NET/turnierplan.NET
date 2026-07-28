using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Turnierplan.ImageStorage.Test.Migration")]

// TODO: When image storage projects are refactored, each implementation should have a testable public API and the following line should not be necessary
[assembly: InternalsVisibleTo("Turnierplan.ImageStorage.Test.Integration")]
