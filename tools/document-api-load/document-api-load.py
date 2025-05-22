from multiprocessing import Process
import requests
import humanize


url = "http://localhost:45000/api"
token = ""
document_id = "___________"
language_code = "de"
time_zone = "Europe/Berlin"
parallel_jobs = 3
requests_per_job = 25


def run_job(job_id):
    session = requests.session()
    for i in range(requests_per_job):
        print(f"Job ({job_id}): Performing request {i + 1}/{requests_per_job}")
        response = session.get(f"{url}/documents/{document_id}/pdf?languageCode={language_code}&timeZone={time_zone}", headers={"Cookie": f"acc_token={token}"})
        if response.status_code != 200:
            print(f"Got status {response.status_code}, exiting [{response.content}]")
            exit(0)
        else:
            print(f"Got {humanize.naturalsize(len(response.content))}")
    pass


if __name__ == '__main__':
    processes = []

    for i in range(parallel_jobs):
        p = Process(target=run_job, args=(i + 1,))
        processes.append(p)
        p.start()

    for p in processes:
        p.join()
