import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';

import { environment } from '../../../environments/environment';

export interface NamedEntity {
  name: string;
}

@Injectable({ providedIn: 'root' })
export class TitleService {
  constructor(
    private readonly title: Title,
    private readonly translate: TranslateService
  ) {}

  public setTitleFrom(value: NamedEntity | undefined): void {
    if (value) {
      this.setCompoundTitle(value.name);
    } else {
      this.title.setTitle(environment.defaultTitle);
    }
  }

  public setTitleTranslated(key: string): void {
    this.setCompoundTitle(this.translate.instant(key) as string);
  }

  public setCompoundTitle(value: string): void {
    this.title.setTitle(`${value} \u00b7 ${environment.defaultTitle}`);
  }
}
