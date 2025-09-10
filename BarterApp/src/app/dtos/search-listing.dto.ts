export interface SearchListingDto {
  id: string;
  title: string;
  description: string;
  snippet?: string;
  category: string;
  distanceKm?: number;
  thumbnailUrl?: string;
}