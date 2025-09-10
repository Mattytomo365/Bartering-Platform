export interface SearchListingsQuery {
  q: string;
  category: string;
  page?: number;
  pageSize?: number;
  lat?: number;
  lng?: number;
  radiusKm?: number;
  sort?: string;
}