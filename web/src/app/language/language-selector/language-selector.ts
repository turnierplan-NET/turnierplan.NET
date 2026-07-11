import { Component, inject } from '@angular/core';
import { Language } from '../language';

@Component({
  selector: 'tp-language-selector',
  imports: [],
  templateUrl: './language-selector.html'
})
export class LanguageSelector {
  protected readonly language = inject(Language);

  protected switchLanguage(): void {
    const current = this.language.language();
    this.language.setLanguage(current === 'de' ? 'en' : 'de');
  }
}
