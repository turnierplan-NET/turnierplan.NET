import { Component, ContentChild, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, TemplateRef } from '@angular/core';

import { LocalStorageService } from '../../services/local-storage.service';

export interface PageFrameNavigationTab {
  id: number;
  title: string;
  icon: string;
}

@Component({
  standalone: false,
  selector: 'tp-page-frame',
  templateUrl: './page-frame.component.html',
  styleUrls: ['./page-frame.component.scss']
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
  public enableBottomPadding = true;

  @Output()
  public navigationTabSelected = new EventEmitter<PageFrameNavigationTab>();

  @ContentChild('buttons')
  public buttons?: TemplateRef<unknown>;

  @ContentChild('content')
  public content?: TemplateRef<unknown>;

  protected readonly history = history;
  protected currentTabId?: number;

  constructor(private readonly localStorageService: LocalStorageService) {}

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

  protected toggleNavigationTab(id: number): void {
    const navigationTab = this.navigationTabs?.find((x) => x.id === id);
    if (!navigationTab) {
      return;
    }

    if (this.rememberNavigationTab) {
      if (this.contextEntityId) {
        this.localStorageService.setNavigationTab(this.contextEntityId, navigationTab.id);
      } else {
        console.error('Cannot retrieve active navigation tab because [contextEntityId] is not set.');
      }
    }

    this.currentTabId = navigationTab.id;
    this.navigationTabSelected.emit(navigationTab);
  }
}
