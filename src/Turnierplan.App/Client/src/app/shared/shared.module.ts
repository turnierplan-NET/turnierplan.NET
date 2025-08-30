import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { FooterComponent } from './components/footer/footer.component';
import { SmallSpinnerComponent } from './components/small-spinner/small-spinner.component';

@NgModule({
  exports: [FooterComponent, SmallSpinnerComponent],
  imports: [CommonModule, TranslateModule.forChild(), RouterModule, FooterComponent, SmallSpinnerComponent]
})
export class SharedModule {}
