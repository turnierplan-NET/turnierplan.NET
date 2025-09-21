export type ComparisonModeOption = {
  id: number;
  modes: TeamComparisonMode[];
  isStandard: boolean;
};

// prettier-ignore
export const availableComparisonModeOptions: ComparisonModeOption[] = [
  { id: 1, modes: [ TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 2, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 3, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore ], isStandard: true },
  { id: 4, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison ], isStandard: true },
  { id: 5, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 6, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore ], isStandard: true },
  { id: 7, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 8, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference ], isStandard: true },
  { id: 9, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison ], isStandard: true },
  { id: 10, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 11, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 12, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison ], isStandard: true },
  { id: 13, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 14, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore ], isStandard: true },
  { id: 15, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 16, modes: [ TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference ], isStandard: true },
  { id: 17, modes: [ TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 18, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 19, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 20, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 21, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 22, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 23, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 24, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 25, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 26, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 27, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 28, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 29, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 30, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 31, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 32, modes: [ TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 33, modes: [ TeamComparisonMode.ByScore ], isStandard: false },
  { id: 34, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 35, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 36, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 37, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 38, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 39, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 40, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 41, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 42, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 43, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 44, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 45, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 46, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 47, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 48, modes: [ TeamComparisonMode.ByScore, TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 49, modes: [ TeamComparisonMode.ByDirectComparison ], isStandard: false },
  { id: 50, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 51, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 52, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 53, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 54, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 55, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 56, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 57, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 58, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 59, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 60, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore ], isStandard: false },
  { id: 61, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints ], isStandard: false },
  { id: 62, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByPoints, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 63, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference ], isStandard: false },
  { id: 64, modes: [ TeamComparisonMode.ByDirectComparison, TeamComparisonMode.ByScore, TeamComparisonMode.ByScoreDifference, TeamComparisonMode.ByPoints ], isStandard: false },
];

/**
 * The following Python code can be used to generate the array above.
 *

data = ['ByPoints', 'ByScoreDifference', 'ByScore', 'ByDirectComparison']
entries = []

for x in data:
  entries.append([x])
  for y in [p for p in data if p != x]:
    entries.append([x, y])
    for z in [p for p in data if p != x and p != y]:
      entries.append([x, y, z])
      for w in [p for p in data if p != x and p != y and p != z]:
        entries.append([x, y, z, w])

standard_entries = [
  [ 'ByPoints', 'ByScoreDifference', 'ByScore', 'ByDirectComparison' ],
  [ 'ByPoints', 'ByScoreDifference', 'ByScore' ],

  [ 'ByPoints', 'ByScoreDifference', 'ByDirectComparison', 'ByScore' ],

  [ 'ByPoints', 'ByScore', 'ByScoreDifference', 'ByDirectComparison' ],
  [ 'ByPoints', 'ByScore', 'ByScoreDifference' ],

  [ 'ByPoints', 'ByDirectComparison', 'ByScoreDifference', 'ByScore' ],
  [ 'ByPoints', 'ByDirectComparison', 'ByScore', 'ByScoreDifference' ],
  [ 'ByPoints', 'ByDirectComparison' ],
]

print('const availableTeamComparisonModeOptions: TeamComparisonModeOption[] = [')

next_id = 1
for entry in entries:
  modes_str = ', '.join([f'TeamComparisonMode.{p}' for p in entry])

  is_standard = False
  for standard_entry in standard_entries:
    if len(entry) == len(standard_entry):
      all_match = True
      for i in range(len(entry)):
        if entry[i] != standard_entry[i]:
          all_match = False
          break
      if all_match:
        is_standard = True
        break

  print(f'  {{ id: {next_id}, modes: [ {modes_str} ], isStandard: { "true" if is_standard else "false" } }},')
  next_id += 1

print('];')

 */
