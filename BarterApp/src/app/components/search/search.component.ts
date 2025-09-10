import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { DiscoveryService } from '../../services/discovery.service';
import { ProfileService } from '../../services/profile.service';
import { ProfileLocation } from '../../models/profile.model';
import { SearchListingsQuery } from '../../models/discovery.model';
import { SearchResult } from '../../dtos/search-result.dto';
import { SearchListingDto } from '../../dtos/search-listing.dto';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-search',
  encapsulation: ViewEncapsulation.None,
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSliderModule,
    MatButtonModule,
    MatListModule,
    MatCardModule
  ],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  form: FormGroup;

  categories = ['All', 'Tools', 'Electronics', 'Furniture', 'Misc'];
  sortOptions = [
    { value: 'relevance', label: 'Relevance' },
    { value: 'distance', label: 'Distance' },
    { value: 'date', label: 'Newest' }
  ];

  location: ProfileLocation | null = null;
  results: SearchListingDto[] = [];
  total = 0;
  page = 1;
  pageSize = 20;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private discovery: DiscoveryService,
    private profile: ProfileService
  ) {
    this.form = this.fb.group({
      q: [''],
      category: [''],
      radiusKm: [10], // <-- must be a number, not null or undefined
      sort: ['']
    });
  }

  ngOnInit(): void {    
    this.profile.getLocation().subscribe(loc => {
      this.location = loc;
      this.search();
    });
  }

  search(page: number = 1): void {
    if (!this.location) return;
    this.loading = true;
    const { q, category, radiusKm, sort } = this.form.value;
    const query: SearchListingsQuery = {
      q,
      category,
      page,
      pageSize: this.pageSize,
      lat: this.location.latitude,
      lng: this.location.longitude,
      radiusKm,
      sort
    };
    this.discovery.search(query).subscribe({
      next: (res: SearchResult) => {
        this.results = res.results;
        this.total = res.total;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.loading = false;
      },
      error: () => {
        this.results = [];
        this.loading = false;
      }
    });
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.total) {
      this.search(this.page + 1);
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.search(this.page - 1);
    }
  }

  get totalPages(): number {
    return Math.ceil(this.total / this.pageSize);
  }
}