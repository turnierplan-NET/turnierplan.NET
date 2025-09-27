import { Component, ContentChild, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, TemplateRef } from '@angular/core';

import { LocalStorageService } from '../../services/local-storage.service';
import { Action } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { NgClass, NgTemplateOutlet, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { E2eDirective } from '../../../core/directives/e2e.directive';

export interface PageFrameNavigationTab {
  id: number;
  title: string;
  icon: string;
  authorization?: Action;
}

@Component({
  selector: 'tp-page-frame',
  templateUrl: './page-frame.component.html',
  styleUrls: ['./page-frame.component.scss'],
  imports: [NgClass, NgTemplateOutlet, RouterLink, AsyncPipe, TranslatePipe, E2eDirective]
})
export class PageFrameComponent implements OnInit, OnChanges {
  @Input()
  public title?: string;

  @Input()
  public ultraSlim = false;

  @Input()
  public backLink?: string;

  @Input()
  public navigationTabs?: PageFrameNavigationTab[] = undefined;

  @Input()
  public contextEntityId?: string;

  @Input()
  public rememberNavigationTab: boolean = false;

  @Input()
  public hideMainContent: boolean = false;

  @Output()
  public navigationTabSelected = new EventEmitter<PageFrameNavigationTab>();

  @ContentChild('buttons')
  public buttons?: TemplateRef<unknown>;

  @ContentChild('preContent')
  public preContent?: TemplateRef<unknown>;

  @ContentChild('mainContent')
  public mainContent?: TemplateRef<unknown>;

  @ContentChild('fullWidthContent')
  public fullWidthContent?: TemplateRef<unknown>;

  protected currentTabId?: number;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnInit(): void {
    if (this.rememberNavigationTab && this.navigationTabs) {
      if (this.contextEntityId) {
        const value = this.localStorageService.getNavigationTab(this.contextEntityId);
        if (value !== undefined) {
          this.toggleNavigationTab(value);
        }
      } else {
        console.error('Cannot retrieve active navigation tab because [contextEntityId] is not set.');
      }
    }
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if ('navigationTabs' in changes) {
      if (this.navigationTabs && this.navigationTabs.length > 0) {
        if (this.currentTabId) {
          const stillExists = this.navigationTabs.findIndex((x) => x.id === this.currentTabId) >= 0;
          if (!stillExists) {
            this.currentTabId = this.navigationTabs[0].id;
          }
        } else {
          this.currentTabId = this.navigationTabs[0].id;
        }
      } else {
        this.currentTabId = undefined;
      }
    }
  }

  public toggleNavigationTab(id: number): void {
    const navigationTab = this.navigationTabs?.find((x) => x.id === id);

    if (!navigationTab) {
      return;
    }

    if (navigationTab.authorization && this.contextEntityId) {
      this.authorizationService.isActionAllowed$(this.contextEntityId, navigationTab.authorization).subscribe({
        next: (isAllowed) => {
          if (isAllowed) {
            this.toggleNavigationTabImpl(navigationTab);
          } else {
            // The following assumes that the first tab is always accessible which is generally true at the moment
            this.toggleNavigationTabImpl(this.navigationTabs![0]);
          }
        }
      });
    } else {
      this.toggleNavigationTabImpl(navigationTab);
    }
  }

  private toggleNavigationTabImpl(tab: PageFrameNavigationTab): void {
    if (this.rememberNavigationTab) {
      if (this.contextEntityId) {
        this.localStorageService.setNavigationTab(this.contextEntityId, tab.id);
      } else {
        console.error('Cannot save active navigation tab because [contextEntityId] is not set.');
      }
    }

    this.currentTabId = tab.id;
    this.navigationTabSelected.emit(tab);
  }
}
