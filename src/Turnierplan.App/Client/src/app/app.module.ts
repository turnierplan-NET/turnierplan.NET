import { CommonModule, registerLocaleData } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import localeDe from '@angular/common/locales/de';
import localeDeExtra from '@angular/common/locales/extra/de';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes, UrlSegment } from '@angular/router';
import { TranslateLoader, TranslateModule, TranslateService, TranslationObject } from '@ngx-translate/core';
import { ToastrModule } from 'ngx-toastr';
import { Observable, of } from 'rxjs';

import { environment } from '../environments/environment';

import { ApiModule } from './api';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AuthenticationInterceptor } from './core/interceptors/authentication.interceptor';
import { de as languageDE } from './i18n/de';
import { identityPages } from './identity/identity.module';
import { SharedModule } from './shared/shared.module';
import { RolesInterceptor } from './core/interceptors/roles.interceptor';

const routes: Routes = [
  {
    path: 'portal',
    children: [
      {
        matcher: (url: UrlSegment[]) => (url.length > 0 && identityPages.some((x) => x === url[0].path) ? { consumed: [] } : null),
        title: environment.defaultTitle,
        loadChildren: () => import('./identity/identity.module').then((m) => m.IdentityModule)
      },
      {
        path: '',
        loadChildren: () => import('./portal/portal.module').then((m) => m.PortalModule)
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

@NgModule({
  declarations: [AppComponent],
  imports: [
    CommonModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    RouterModule.forRoot(routes),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useClass: ImmediateTranslateLoader,
        deps: [HttpClient]
      }
    }),
    CoreModule.forRoot(),
    ToastrModule.forRoot({ positionClass: 'toast-bottom-left' }),
    SharedModule,
    ApiModule.forRoot({ rootUrl: window.origin })
  ],
  bootstrap: [AppComponent],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: RolesInterceptor, multi: true }
  ]
})
export class AppModule {
  constructor(translateService: TranslateService) {
    registerLocaleData(localeDe, 'de', localeDeExtra);

    translateService.setDefaultLang('de');
    translateService.use('de');
  }
}
