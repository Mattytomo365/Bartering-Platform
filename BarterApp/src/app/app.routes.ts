// 
import { Routes } from '@angular/router';
import { LoginComponent }       from './components/login/login.component';
import { ListingListComponent } from './components/listing/listing-list/listing-list.component';
import { authGuard }            from './core/guards/auth.guard';
import { LayoutComponent } from './components/layout/layout.component';
import { ProfileComponent } from './components/profile/profile.component';
import { SearchComponent } from './components/search/search.component';

export const routes: Routes = [
  // public login page:
  { path: 'login', component: LoginComponent },

  // all authenticated routes live under layout:
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'listings', component: ListingListComponent },
      { path: 'profile',   component: ProfileComponent },
      { path: 'search',   component: SearchComponent },

      // ...other child routes...
      { path: '', redirectTo: 'listings', pathMatch: 'full' }
    ]
  },

  // fallback:
  { path: '**', redirectTo: '' }
];