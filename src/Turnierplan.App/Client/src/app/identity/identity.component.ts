import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from '../core/components/footer/footer.component';

@Component({
  templateUrl: './identity.component.html',
  styleUrls: ['./identity.component.scss'],
  imports: [RouterOutlet, FooterComponent]
})
export class IdentityComponent {}
