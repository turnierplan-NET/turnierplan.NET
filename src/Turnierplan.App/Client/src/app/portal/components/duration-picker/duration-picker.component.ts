import { Component, EventEmitter, Input, Output } from '@angular/core';

const pad = (i: number): string => (i < 10 ? `0${i}` : `${i}`);

@Component({
  selector: 'tp-duration-picker',
  templateUrl: './duration-picker.component.html'
})
export class DurationPickerComponent {
  @Output()
  public durationChange = new EventEmitter<string>();

  protected minutes: number = 0;
  protected seconds: number = 0;

  @Input()
  public set duration(value: string) {
    // The regex matches duration strings from the .NET TimeSpan class. This allows for the 'days' value
    // to be set, however the resulting minutes/seconds is clamped to be at most 23h:59m:59s
    const match = /^(?:(?<d>\d+)\.)?(?<h>\d{2}):(?<m>\d{2}):(?<s>\d{2})(?:\.\d{7})?$/.exec(value);

    if (match?.groups) {
      let hours = +match.groups['h'];
      if (match.groups['d']) {
        hours += 24 * +match.groups['d'];
      }

      this.minutes = +match.groups['m'] + 60 * hours;
      this.seconds = +match.groups['s'];
    }
  }

  protected valueChanged(): void {
    this.minutes = Math.min(Math.max(0, this.minutes ?? 0), 1439);
    this.seconds = Math.min(Math.max(0, this.seconds ?? 0), 60);

    if (this.minutes >= 0 && this.seconds >= 0 && this.seconds <= 60) {
      const hours = Math.floor(this.minutes / 60);
      const minutes = this.minutes % 60;
      this.durationChange.emit(`${pad(hours)}:${pad(minutes)}:${pad(this.seconds)}`);
    }
  }
}
