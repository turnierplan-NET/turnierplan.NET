import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { LocalStorageService } from '../../services/local-storage.service';
import { AutoReloadConfig } from '../../models/auto-reload-config';
import { FormsModule } from '@angular/forms';
import { TranslateDirective } from '@ngx-translate/core';

const defaultConfig: AutoReloadConfig = { enableAutoReload: true, autoReloadInterval: 60 };

@Component({
  selector: 'tp-auto-reload-toggle',
  imports: [FormsModule, TranslateDirective],
  templateUrl: './auto-reload-toggle.component.html'
})
export class AutoReloadToggleComponent implements OnInit {
  @Output()
  public pathSuffixChanged = new EventEmitter<string>();

  protected config: AutoReloadConfig;

  private readonly localStorageService = inject(LocalStorageService);

  constructor() {
    this.config = this.localStorageService.getAutoReloadConfig() ?? defaultConfig;
  }

  public ngOnInit(): void {
    this.emitCurrentValue();
  }

  protected emitCurrentValue(): void {
    if (this.config.enableAutoReload) {
      if (!this.config.autoReloadInterval) {
        this.config.autoReloadInterval = defaultConfig.autoReloadInterval;
      }
      this.pathSuffixChanged.emit(`&autoReload=${this.config.autoReloadInterval}`);
    } else {
      this.pathSuffixChanged.emit('');
    }

    this.localStorageService.setAutoReloadConfig(this.config);
  }
}
