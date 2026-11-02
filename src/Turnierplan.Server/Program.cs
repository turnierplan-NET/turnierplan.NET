using Turnierplan.Server;

Console.WriteLine();
Console.WriteLine(  "  __                                                     ___                                        __");
Console.WriteLine( @" /\ \__                        __                       /\_ \                                      /\ \__");
Console.WriteLine( @" \ \ ,_\  __  __  _ __    ___ /\_\     __   _ __   _____\//\ \      __      ___         ___      __\ \ ,_\");
Console.WriteLine( @"  \ \ \/ /\ \/\ \/\`'__\/' _ `\/\ \  /'__`\/\`'__\/\ '__`\\ \ \   /'__`\  /' _ `\     /' _ `\  /'__`\ \ \/");
Console.WriteLine( @"   \ \ \_\ \ \_\ \ \ \/ /\ \/\ \ \ \/\  __/\ \ \/ \ \ \L\ \\_\ \_/\ \L\.\_/\ \/\ \  __/\ \/\ \/\  __/\ \ \_");
Console.WriteLine( @"    \ \__\\ \____/\ \_\ \ \_\ \_\ \_\ \____\\ \_\  \ \ ,__//\____\ \__/.\_\ \_\ \_\/\_\ \_\ \_\ \____\\ \__\");
Console.WriteLine( @"     \/__/ \/___/  \/_/  \/_/\/_/\/_/\/____/ \/_/   \ \ \/ \/____/\/__/\/_/\/_/\/_/\/_/\/_/\/_/\/____/ \/__/");
Console.WriteLine( @"                                                     \ \_\");
Console.WriteLine($@"                                                      \/_/   v{TurnierplanVersion.Version}");
Console.WriteLine();

var server = new TurnierplanServer(args);

await server.RunAsync();
