import { Injectable } from '@angular/core';
import { Auth, signInWithEmailAndPassword, createUserWithEmailAndPassword, onAuthStateChanged, User } from '@angular/fire/auth';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {

  // Expose current user as an Observable
  private userSubject = new BehaviorSubject<User | null>(null);
  user$: Observable<User | null> = this.userSubject.asObservable();

  constructor(private auth: Auth) {
    // Keep BehaviorSubject in sync
    onAuthStateChanged(this.auth, user => {
      this.userSubject.next(user);
      if (user) {
        localStorage.setItem('ownerId', user.uid);
      } else {
        localStorage.removeItem('ownerId');
      }
    });
  }

  /** Sign in with email/password */
  async login(email: string, password: string): Promise<User> {
    const cred = await signInWithEmailAndPassword(this.auth, email, password);
    this.userSubject.next(cred.user);
    return cred.user;
  }

  /** Register a new user */
  async signup(email: string, password: string): Promise<User> {
    const cred = await createUserWithEmailAndPassword(this.auth, email, password);
    this.userSubject.next(cred.user);
    return cred.user;
  }

  /** Get the current ID token (for interceptor) */
  getIdToken(): Promise<string | null> {
    const user = this.auth.currentUser;
    return user ? user.getIdToken() : Promise.resolve(null);
  }

  /** Convenience to read ownerId */
  getOwnerId(): string | null {
    return localStorage.getItem('ownerId');
  }

  /** Optional: log out */
  async logout(): Promise<void> {
    await this.auth.signOut();
    this.userSubject.next(null);
    localStorage.removeItem('ownerId');
  }
}