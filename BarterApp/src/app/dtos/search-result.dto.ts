import { SearchListingDto } from "./search-listing.dto";

export interface SearchResult {
  results: SearchListingDto[];
  total: number;
  page: number;
  pageSize: number;
}