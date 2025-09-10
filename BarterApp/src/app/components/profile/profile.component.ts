import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { debounceTime, switchMap } from 'rxjs/operators';
import { ProfileService } from '../../services/profile.service';
import { GeocodingService, GeocodeSuggestion } from '../../services/geocoding.service';
import { ProfileLocation } from '../../models/profile.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatAutocompleteModule,
    MatListModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  locationForm!: FormGroup;
 
  suggestions: GeocodeSuggestion[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private geocode: GeocodingService
  ) {}

  ngOnInit() {
    this.locationForm = this.fb.group({
      displayName: ['', Validators.required],
      latitude:    [{ value: 0, disabled: true }],
      longitude:   [{ value: 0, disabled: true }]
    });
    
    // Subscribe to location$ for live updates
    this.profileService.location$.subscribe(loc => {
      if (loc) this.locationForm.patchValue(loc);
    });
    this.loadLocation();
    this.setupAutocomplete();
  }

  private loadLocation() {
    // Only trigger the HTTP call, patching is handled by the subscription above
    this.profileService.getLocation().subscribe({
      error: () => this.error = 'Failed to load profile location.'
    });
  }

  private setupAutocomplete() {
    this.locationForm.controls['displayName'].valueChanges.pipe(
      debounceTime(300),
      switchMap(val => typeof val === 'string' ? this.geocode.autocomplete(val) : [])
    ).subscribe(s => this.suggestions = s);
  }

  displayFn(s: GeocodeSuggestion) {
    return s && s.displayName;
  }

  // Utility to extract town/city from Nominatim address object
  private extractTownFromAddress(address: any): string {
    return (
      address?.city ||
      address?.town ||
      address?.village ||
      address?.hamlet ||
      address?.municipality ||
      address?.county ||
      ''
    );
  }

  optionSelected(event: any) {
    const sel: GeocodeSuggestion = event.option.value;
    const town = this.extractTownFromAddress(sel.address);
    this.locationForm.patchValue({
      displayName: town,
      latitude: sel.latitude,
      longitude: sel.longitude
    });
    this.suggestions = [];
  }

  useMyLocation() {
    this.error   = null;
    this.loading = true;

    if (!navigator.geolocation) {
      this.error   = 'Geolocation not supported.';
      this.loading = false;
      return;
    }

    navigator.geolocation.getCurrentPosition(
      pos => {
        const { latitude, longitude } = pos.coords;

        // 1) Reverse-geocode to get a nice town/postcode
        this.geocode.reverse(latitude, longitude).subscribe({
          next: result => {
            const town = this.extractTownFromAddress(result.address);
            // 2) Patch the form and then save
            this.locationForm.patchValue({ latitude, longitude, displayName: town });

            this.profileService.updateLocation({
              displayName: town,
              latitude,
              longitude
            }).subscribe({
              next: () => this.loading = false,
              error: () => {
                this.error   = 'Failed to save location.';
                this.loading = false;
              }
            });
          },
          error: () => {
            this.error   = 'Unable to determine address from your location.';
            this.loading = false;
          }
        });
      },
      () => {
        this.error   = 'Unable to retrieve your location.';
        this.loading = false;
      }
    );
  }  

  save() {
    if (this.locationForm.invalid) return;
    const payload = this.locationForm.getRawValue() as ProfileLocation;
    this.profileService.updateLocation(payload).subscribe({
      next: () => this.error = null,
      error: () => this.error = 'Failed to save location.'
    });
  }
}