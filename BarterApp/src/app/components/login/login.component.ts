import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
// Import Angular Material modules.
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [
      CommonModule,
      FormsModule,
      RouterModule,
      MatCardModule,
      MatFormFieldModule,
      MatInputModule,
      MatButtonModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';
  private returnUrl: string;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // grab the returnUrl if the guard sent one, otherwise default to /listings
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/listings';
  }

  login(): void {
    this.authService.login(this.email, this.password)
      .then(user => {
        console.log('Logged in:', user.uid);
        this.router.navigateByUrl(this.returnUrl);
      })
      .catch(err => this.errorMessage = err.message);
  }

  signup(): void {
    this.authService.signup(this.email, this.password)
      .then(user => {
        console.log('Signed up:', user.uid);
        this.router.navigateByUrl('/profile'); // Route to profile after signup
      })
      .catch(err => this.errorMessage = err.message);
  }
}