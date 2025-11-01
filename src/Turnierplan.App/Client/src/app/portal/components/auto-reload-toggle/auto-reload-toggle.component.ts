import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { LocalStorageService } from '../../services/local-storage.service';
import { AutoReloadConfig } from '../../models/auto-reload-config';

const defaultConfig: AutoReloadConfig = { enableAutoReload: true, autoReloadInterval: 60 };

@Component({
  selector: 'tp-auto-reload-toggle',
  imports: [],
  templateUrl: './auto-reload-toggle.component.html'
})
export class AutoReloadToggleComponent implements OnInit {
  @Output()
  public pathSuffixChanged = new EventEmitter<string>();

  protected config: AutoReloadConfig;

  private readonly localStorageService = inject(LocalStorageService);

  constructor() {
    this.config = this.localStorageService.getShareWidgetAutoReloadInterval() ?? defaultConfig;
  }

  public ngOnInit(): void {
    this.emitCurrentValue();
  }

  private emitCurrentValue(): void {
    if (this.config.enableAutoReload && this.config.autoReloadInterval > 0) {
      this.pathSuffixChanged.emit(`&autoReload=${this.config.autoReloadInterval}`);
    } else {
      this.pathSuffixChanged.emit('');
    }
  }
}
