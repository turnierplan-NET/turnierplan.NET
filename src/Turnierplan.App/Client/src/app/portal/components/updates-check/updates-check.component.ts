import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';

interface GitHubApiResponse {
  tag_name: string;
}

type VersionData = {
  timestamp: number;
  version?: string;
};

@Component({
  selector: 'tp-updates-check',
  imports: [SmallSpinnerComponent, TranslateDirective, TranslatePipe, NgbTooltip],
  templateUrl: './updates-check.component.html',
  styleUrl: 'updates-check.component.scss'
})
export class UpdatesCheckComponent implements OnInit {
  protected isInitializing = true;
  protected currentVersion: string = environment.version;
  protected latestVersion?: string;

  constructor(private readonly http: HttpClient) {}

  public ngOnInit(): void {
    this.getMostRecentReleaseVersion().subscribe((result) => {
      this.latestVersion = result.version;
      this.isInitializing = false;
    });
  }

  private getMostRecentReleaseVersion(): Observable<VersionData> {
    if (!environment.production) {
      return of({
        version: environment.version,
        timestamp: new Date().getTime()
      });
    }

    const localStorageKey = 'tp_updatesCheck_cache';
    const localStorageValue = localStorage.getItem(localStorageKey);
    const cacheMaxAgeMilliseconds = 6 * 60 * 60 * 1000;

    if (localStorageValue) {
      try {
        const parsed = JSON.parse(localStorageValue) as VersionData;
        const cacheExpiry = parsed.timestamp + cacheMaxAgeMilliseconds;
        if (new Date().getTime() < cacheExpiry) {
          return of(parsed);
        }
      } catch (e) {
        return of({
          version: undefined,
          timestamp: 0
        });
      }
    }

    return this.http.get<GitHubApiResponse>('https://api.github.com/repos/turnierplan-NET/turnierplan.NET/releases/latest').pipe(
      map(
        (response): VersionData => ({
          version: response.tag_name,
          timestamp: new Date().getTime()
        })
      ),
      catchError(() =>
        of({
          version: undefined,
          timestamp: new Date().getTime()
        })
      ),
      tap((data) => {
        localStorage.setItem(localStorageKey, JSON.stringify(data));
      })
    );
  }
}
