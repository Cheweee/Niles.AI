import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

// Стираем кэш, чтобы после релиза обновились скрипты и стили
if (localStorage.getItem('refreshed') === null) {
  localStorage['refreshed'] = true;
  window.location.reload();
} else {
  localStorage.removeItem('refreshed');
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
