import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchListingsQuery } from '../models/discovery.model';
import { SearchResult } from '../dtos/search-result.dto';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DiscoveryService {
  private base = `${environment.apiGatewayUrl}/discovery`;

  constructor(private http: HttpClient) {}

  search(query: SearchListingsQuery): Observable<SearchResult> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        params = params.set(key, value.toString());
      }
    });
    
    return this.http.get<SearchResult>(`${this.base}/search`, { params });
  }
}