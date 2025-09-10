import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { ListingService } from '../../../services/listing.service';
import { ListingModel } from '../../../models/listing.model';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { ListingFormDialogComponent } from '../listing-form-dialog/listing-form-dialog.component';

@Component({
  selector: 'app-listing-list',
  templateUrl: './listing-list.component.html',
  standalone: true,
  imports: [
    CommonModule, 
    MatTableModule, 
    MatButtonModule, 
    MatIconModule
  ]
})
export class ListingListComponent implements OnInit {
  columns = ['title', 'createdAt', 'actions'];
  listings: ListingModel[] = [];

  constructor(private service: ListingService, private dialog: MatDialog) {}

  ngOnInit() { this.load(); }

  load() {
    this.service.getAll().subscribe(data => this.listings = data);
  }

  create() {
    // const ref = this.dialog.open(ListingFormDialogComponent);
    const ref = this.dialog.open(ListingFormDialogComponent, {
      panelClass: 'listing-form-dialog-container',
      width:      '600px'   // optional, matches your max-width
    });
    ref.afterClosed().subscribe(res => res && this.load());
  }

  edit(listing: ListingModel) {
    // const ref = this.dialog.open(ListingFormDialogComponent, { data: listing.id });
    const ref = this.dialog.open(ListingFormDialogComponent, {
      data:       listing.id,
      panelClass: 'listing-form-dialog-container',
      width:      '600px'
    });    
    ref.afterClosed().subscribe(res => res && this.load());
  }

  delete(id: string) {
    this.service.delete(id).subscribe(() => this.load());
  }
}