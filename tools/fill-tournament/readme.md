# Fill Tournament Tool

This script fills an existing tournament in any turnierplan.NET with randomly generated match outcomes. This is useful for testing purposes when you need a tournament with outcomes but don't want to enter the match outcomes by hand.

This script requires the following parameters:

- URL of the turnierplan.NET instance (default is local dev environment)
- Valid authentication token (can be acquired using browser dev tools)
- ID of an existing tournament with matches
- Whether to overwrite existing match outcomes or skip them

In addition, the following parameters can be adjusted regarding the generated match outcomes:

- Maximum score per team
- Probability of deciding matches ending in overtime or penalties.

> [!WARNING]  
> The script will behave unexpectedly if the sum of the two above-mentioned probabilities exceeds 1.0
