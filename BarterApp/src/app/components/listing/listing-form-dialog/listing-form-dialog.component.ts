import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule  } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ListingService } from '../../../services/listing.service';
import { ProfileService } from '../../../services/profile.service';
import { GeocodingService, GeocodeSuggestion } from '../../../services/geocoding.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { debounceTime, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-listing-form-dialog',
  templateUrl: './listing-form-dialog.component.html',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule,
    MatCardModule,
    CommonModule,
    MatIconModule,
    MatAutocompleteModule
  ]
})
export class ListingFormDialogComponent implements OnInit {
  form!: FormGroup;
  userLocation: { displayName: string; latitude: number; longitude: number } | null = null;
  listingLocation: { displayName: string; latitude: number; longitude: number } | null = null;
  suggestions: GeocodeSuggestion[] = [];
  loading = false;

  constructor(
    private fb: FormBuilder,
    private service: ListingService,
    public dialogRef: MatDialogRef<ListingFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public id: string,
    private profileService: ProfileService,
    private geocode: GeocodingService
  ) {}

  // Utility to extract town from Nominatim display_name
  private extractTown(displayName: string): string {
    if (!displayName) return '';
    return displayName.split(',')[0].trim();
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

  ngOnInit() {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      priceAmount: [0, Validators.required],
      priceCurrency: ['GBP', Validators.required],
      category: ['', Validators.required],
      condition: ['', Validators.required],
      locationDisplay: ['']
    });

    // Get user profile location
    this.profileService.getLocation().subscribe(loc => {
      this.userLocation = loc;
      if (!this.listingLocation) {
        this.listingLocation = loc;
        this.form.patchValue({ locationDisplay: loc.displayName });
      }
    });

    if (this.id) {
      this.service.getById(this.id)
          .subscribe(data => {
            this.form.patchValue(data);
            if (typeof data.latitude === 'number' && typeof data.longitude === 'number') {
              this.geocode.reverse(data.latitude, data.longitude).subscribe(
                (result: any) => {
                  const town = this.extractTownFromAddress(result.address);
                  this.listingLocation = {
                    displayName: town,
                    latitude: data.latitude,
                    longitude: data.longitude
                  };
                  this.form.patchValue({ locationDisplay: town });
                },
                () => {
                  this.listingLocation = {
                    displayName: '',
                    latitude: data.latitude,
                    longitude: data.longitude
                  };
                }
              );
            }
          });
    }

    // Setup autocomplete for location
    this.form.controls['locationDisplay'].valueChanges.pipe(
      debounceTime(300),
      switchMap(val => typeof val === 'string' ? this.geocode.autocomplete(val) : [])
    ).subscribe(s => this.suggestions = s);
  }

  displayFn(s: GeocodeSuggestion) {
    return s && s.displayName;
  }

  optionSelected(event: any) {
    const sel: GeocodeSuggestion = event.option.value;
    const town = this.extractTownFromAddress(sel.address);
    this.listingLocation = {
      displayName: town,
      latitude: sel.latitude,
      longitude: sel.longitude
    };
    this.form.patchValue({ locationDisplay: town });
    this.suggestions = [];
  }

  save() {
    let payload = this.form.value;
    // Attach location for this listing
    if (this.listingLocation) {
      payload = {
        ...payload,
        displayName: this.listingLocation.displayName,
        latitude: this.listingLocation.latitude,
        longitude: this.listingLocation.longitude
      };
    }
    if (this.id) {
      payload = { ...payload, id: this.id };
    }
    const obs = this.id 
      ? this.service.update(this.id, payload) 
      : this.service.create(payload);
    obs.subscribe(() => this.dialogRef.close(true));
  }
}