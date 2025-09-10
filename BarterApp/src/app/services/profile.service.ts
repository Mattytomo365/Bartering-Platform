import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ProfileLocation } from '../models/profile.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private base = `${environment.apiGatewayUrl}/profile`;

  private locationSubject = new BehaviorSubject<ProfileLocation | null>(null);
  location$ = this.locationSubject.asObservable();

  constructor(private http: HttpClient) {}

  getLocation(): Observable<ProfileLocation> {
    return this.http.get<ProfileLocation>(`${this.base}/location`).pipe(
      tap(loc => this.locationSubject.next(loc))
    );
  }

  updateLocation(payload: ProfileLocation): Observable<void> {
    return this.http.post<void>(`${this.base}/location`, payload).pipe(
      tap(() => this.locationSubject.next(payload))
    );
  }
}
