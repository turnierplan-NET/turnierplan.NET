import requests
import time
import random

url = "http://localhost:45000/api"
token = ""
tournament_id = "___________"
skip_finished = False

max_score = 4
overtime_chance = 0.2
penalties_chance = 0.3


def create_outcome(match):
    is_deciding_match = match['metadata']['type'] != 'GroupMatch'

    # Generate a random score for each team. In case of a deciding match, make sure the two scores are not equal.
    while True:
        score_a = random.randint(0, max_score)
        score_b = random.randint(0, max_score)
        if (not is_deciding_match) or (score_a != score_b):
            break

    outcome_type = "Standard"
    if is_deciding_match:
        x = random.random()
        if x < overtime_chance:
            outcome_type = "AfterOvertime"
        elif x < (overtime_chance + penalties_chance):
            outcome_type = "AfterPenalties"

    return score_a, score_b, outcome_type


if __name__ == '__main__':
    print("Reading tournament...")
    session = requests.session()
    response = session.get(f"{url}/tournaments/{tournament_id}", headers={"Cookie": f"acc_token={token}"})

    if response.status_code != 200:
        print(f"Got status {response.status_code}, exiting")
        print(response.content)
        exit(0)

    matches = response.json()['matches']
    print(f"Tournament contains {len(matches)} matches")

    for match in matches:
        if skip_finished and match['outcome']['isFinished']:
            print(f"Skipping match {match['index']} (already finished)")
            continue

        scoreA, scoreB, outcomeType = create_outcome(match)

        command = {
            "TournamentId": tournament_id,
            "MatchId": match['id'],
            "IsFinished": True,
            "ScoreA": scoreA,
            "ScoreB": scoreB,
            "OutcomeType": outcomeType,
            "State": "Finished"
        }

        time_before = time.time()
        response = session.patch(f"{url}/tournaments/{tournament_id}/matches/{match['id']}/outcome",
                                 json=command,
                                 headers={"Cookie": f"acc_token={token}"})

        if response.status_code != 204:
            print(f"Got status {response.status_code} when updating match {match['index']}, exiting")
            print(response.content)
            break
        else:
            print(f"Updated match {match['index']} to {scoreA}:{scoreB} ({outcomeType}) "
                  f"in {(time.time() - time_before):.2f} seconds")
