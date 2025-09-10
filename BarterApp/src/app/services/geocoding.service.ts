import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface GeocodeSuggestion {
  displayName: string;
  latitude: number;
  longitude: number;
  address?: any;
}

@Injectable({ providedIn: 'root' })
export class GeocodingService {
  private nominatimSearch  = 'https://nominatim.openstreetmap.org/search';
  private nominatimReverse = 'https://nominatim.openstreetmap.org/reverse';

  constructor(private http: HttpClient) {}

  autocomplete(query: string): Observable<GeocodeSuggestion[]> {
    const params = new HttpParams()
      .set('format', 'json')
      .set('addressdetails', '1') // changed to 1
      .set('limit', '5')
      .set('q', query);

    return this.http.get<any[]>(this.nominatimSearch, { params }).pipe(
      map(results => results.map(r => ({
        displayName: r.display_name,
        latitude: +r.lat,
        longitude: +r.lon,
        address: r.address // pass address object for town extraction
      })))
    );
  }

  reverse(lat: number, lon: number): Observable<any> {
    return this.http.get<any>(this.nominatimReverse, {
      params: new HttpParams()
        .set('format', 'json')
        .set('addressdetails', '1') // changed to 1
        .set('lat', lat.toString())
        .set('lon', lon.toString())
    }).pipe(
      map(res => res)
    );
  }  
}