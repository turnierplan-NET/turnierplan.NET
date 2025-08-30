/// <reference types="@angular/localize" />

import { enableProdMode, importProvidersFrom } from '@angular/core';
import localeDe from '@angular/common/locales/de';
import localeDeExtra from '@angular/common/locales/extra/de';
import { environment } from './environments/environment';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authenticationInterceptor } from './app/core/interceptors/authentication.interceptor';
import { rolesInterceptor } from './app/core/interceptors/roles.interceptor';
import { CommonModule, registerLocaleData } from '@angular/common';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter, Routes, UrlSegment } from '@angular/router';
import { identityPages } from './app/identity/identity.module';
import { TranslateLoader, TranslationObject, provideTranslateLoader, provideTranslateService } from '@ngx-translate/core';
import { Observable, of } from 'rxjs';
import { de as languageDE } from './app/i18n/de';
import { CoreModule } from './app/core/core.module';
import { provideToastr } from 'ngx-toastr';
import { SharedModule } from './app/shared/shared.module';
import { ApiModule } from './app/api';
import { AppComponent } from './app/app.component';

const routes: Routes = [
  {
    path: 'portal',
    children: [
      {
        matcher: (url: UrlSegment[]) => (url.length > 0 && identityPages.some((x) => x === url[0].path) ? { consumed: [] } : null),
        title: environment.defaultTitle,
        loadChildren: () => import('./app/identity/identity.module').then((m) => m.IdentityModule)
      },
      {
        path: '',
        loadChildren: () => import('./app/portal/portal.module').then((m) => m.PortalModule)
      }
    ]
  },
  {
    path: '**',
    pathMatch: 'full',
    redirectTo: '/portal'
  }
];

class ImmediateTranslateLoader implements TranslateLoader {
  public getTranslation(lang: string): Observable<TranslationObject> {
    if (lang !== 'de') {
      throw new Error(`Language ${lang} not supported.`);
    }

    return of(languageDE);
  }
}

if (environment.production) {
  enableProdMode();
}

registerLocaleData(localeDe, 'de', localeDeExtra);

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(CommonModule, BrowserModule, CoreModule.forRoot(), SharedModule, ApiModule.forRoot({ rootUrl: window.origin })),
    provideHttpClient(withInterceptors([authenticationInterceptor, rolesInterceptor])),
    provideTranslateService({
      loader: provideTranslateLoader(ImmediateTranslateLoader)
    }),
    provideToastr({ positionClass: 'toast-bottom-left' }),
    provideAnimations(),
    provideRouter(routes)
  ]
}).catch((err) => console.error(err));
