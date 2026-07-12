import { Component, inject } from '@angular/core';
import { Language } from '../language';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-language-selector',
  imports: [NgbTooltip, TranslatePipe],
  templateUrl: './language-selector.html'
})
export class LanguageSelector {
  protected readonly language = inject(Language);

  protected switchLanguage(): void {
    const current = this.language.language();
    this.language.setLanguage(current === 'de' ? 'en' : 'de');
  }
}
