import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'tp-auto-reload-toggle',
  imports: [],
  templateUrl: './auto-reload-toggle.component.html'
})
export class AutoReloadToggleComponent implements OnInit {
  @Output()
  public pathSuffixChanged = new EventEmitter<string>();

  public ngOnInit(): void {
    // TODO bla
    this.pathSuffixChanged.emit('&autoReload=5');
  }
}
