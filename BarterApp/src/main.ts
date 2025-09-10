import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { firebaseConfig } from './firebase-config';
// import { FirebaseApp } from 'firebase/app';
// import { Auth } from 'firebase/auth';
import { provideFirebaseApp, initializeApp } from '@angular/fire/app';
import { provideAuth, getAuth } from '@angular/fire/auth';
import { AuthInterceptor } from './app/core/interceptors/auth.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';

bootstrapApplication(AppComponent, {
  // Spread any properties from your existing config
  ...appConfig,
  // Make sure providers includes both your appConfig providers (if any) and the new ones
  providers: [
    ...(appConfig.providers || []),
    provideAnimations(),
    provideFirebaseApp(() => initializeApp(firebaseConfig)),
    provideAuth(() => getAuth()),
    provideHttpClient(withInterceptorsFromDi()),

        // Register your interceptor
    { 
      provide: HTTP_INTERCEPTORS, 
      useClass: AuthInterceptor, 
      multi: true 
    }
    
    // ...other providers
  ]
});