import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ListingModel } from '../models/listing.model';
import { ListingDetailModel } from '../models/listing-detail.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ListingService {
  private base = `${environment.apiGatewayUrl}/listings`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ListingModel[]> {
    return this.http.get<ListingModel[]>(`${this.base}/user/${this.getOwnerId()}`);
  }

  getById(id: string): Observable<ListingDetailModel> {
    return this.http.get<ListingDetailModel>(`${this.base}/${id}`);
  }

  create(payload: Omit<ListingDetailModel, 'ownerId'>) {
    // Omit the 'ownerId' property from the payload
    const dto: ListingDetailModel = { 
      ...payload, 
      ownerId: this.getOwnerId() 
    };
    return this.http.post<void>(this.base, dto);
  }

  update(id: string, payload: Partial<ListingDetailModel>) {
    return this.http.put<void>(`${this.base}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  private getOwnerId(): string {
    return localStorage.getItem('ownerId') || '';
  }
}