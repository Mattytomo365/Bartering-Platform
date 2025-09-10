import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListingFormDialogComponent } from './listing-form-dialog.component';

describe('ListingFormDialogComponent', () => {
  let component: ListingFormDialogComponent;
  let fixture: ComponentFixture<ListingFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListingFormDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListingFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
