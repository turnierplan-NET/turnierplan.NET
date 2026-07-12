import { inject, isDevMode, Service, signal, Signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appVersion } from '../app.config';
import { catchError, of } from 'rxjs';
import { map } from 'rxjs/operators';

export type VersionInfo =
  | {
      state: 'loading' | 'error';
      current: string;
    }
  | {
      state: 'up-to-date' | 'outdated' | 'newer';
      current: string;
      latest: string;
    };

interface GitHubApiResponse {
  tag_name: string;
}

interface VersionCache {
  timestamp: number;
  version?: string;
}

@Service()
export class Version {
  private static readonly localStorageKey = 'tp_version_cache'; // TODO: Move all local storage keys to shared location
  private static readonly maxCacheAgeMilliseconds = 6 * 60 * 60 * 1000; // 6 hours

  public readonly version: Signal<VersionInfo>;

  private readonly http = inject(HttpClient);
  private readonly _version = signal<VersionInfo>({ state: 'loading', current: appVersion });

  constructor() {
    this.version = this._version.asReadonly();

    if (isDevMode()) {
      // Don't spam GitHub API when running locally
      this._version.set({ state: 'up-to-date', current: appVersion, latest: appVersion });
    } else {
      const localStorageValue = localStorage.getItem(Version.localStorageKey);

      if (localStorageValue) {
        try {
          const parsed = JSON.parse(localStorageValue) as VersionCache;
          const cacheExpiry = parsed.timestamp + Version.maxCacheAgeMilliseconds;
          if (Date.now() < cacheExpiry) {
            this.processLatestVersion(parsed.version);
          }
        } catch (e) {
          this.processLatestVersion(undefined);
        }
      } else {
        this.http
          .get<GitHubApiResponse>(
            'https://api.github.com/repos/turnierplan-NET/turnierplan.NET/releases/latest'
          )
          .pipe(
            map((response) => response.tag_name),
            catchError(() => of(undefined))
          )
          .subscribe({
            next: (version) => {
              localStorage.setItem(
                Version.localStorageKey,
                JSON.stringify({
                  version: version,
                  timestamp: Date.now()
                } as VersionCache)
              );

              this.processLatestVersion(version);
            }
          });
      }
    }
  }

  private processLatestVersion(latest: string | undefined): void {
    if (!latest) {
      this._version.set({ state: 'error', current: appVersion });
      return;
    }

    // The version schema can be sorted alphanumerically. Thus, we can use localeCompare to
    // find out whether the current version is older, newer or equal to the latest version.
    const compare = appVersion.localeCompare(latest);

    if (compare < 0) {
      this._version.set({ state: 'outdated', current: appVersion, latest: latest });
    } else if (compare > 0) {
      this._version.set({ state: 'newer', current: appVersion, latest: latest });
    } else {
      this._version.set({ state: 'up-to-date', current: appVersion, latest: latest });
    }
  }
}
