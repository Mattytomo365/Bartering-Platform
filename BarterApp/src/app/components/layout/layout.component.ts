import { Component, OnInit } from '@angular/core';
import { AuthService }        from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { Router }    from '@angular/router';
import { ProfileService }    from '../../services/profile.service';
import { ProfileLocation }   from '../../models/profile.model';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent implements OnInit {

  constructor(
    public auth: AuthService,
    private router: Router,
    private profileService: ProfileService
  ) {}

  // hold their profile location
  location: ProfileLocation | null = null;

  ngOnInit(): void {
    // Subscribe to location$ for live updates
    this.profileService.location$.subscribe(loc => {
      this.location = loc;
    });
    // Trigger initial load
    this.auth.user$.subscribe(u => {
      if (u) {
        this.profileService.getLocation().subscribe({
          error: _ => this.location = null
        });
      } else {
        this.location = null;
      }
    });
  }

  logout(): void {
    this.auth.logout()
      .then(() => {
        // send them to the login page
        this.router.navigateByUrl('/login');
      })
      .catch(err => {
        console.error('Logout failed', err);
      });
  }

}
