import { Component, inject } from '@angular/core';
import { Language } from '../language';

@Component({
  selector: 'tp-language-selector',
  imports: [],
  templateUrl: './language-selector.html'
})
export class LanguageSelector {
  protected readonly language = inject(Language);
}
