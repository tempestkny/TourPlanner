import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ImportExportService, ImportTourData } from '../../Import-Export/import-export.service';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { TourListService } from './tour-list-service';
import { TourEntry } from './tour-entry/tour-entry';

@Component({
  selector: 'app-tour-list',
  imports: [TourEntry, RouterModule],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList implements OnInit, OnDestroy {
  @Output() selectTour = new EventEmitter<TourItemInterface>();
  @Output() editTour = new EventEmitter<TourItemInterface>();
  @Output() createTour = new EventEmitter<void>();

  @Output() viewLogList = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>();

  tours: TourItemInterface[] = [];
  importExportMessage = '';
  isImportExportLoading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private tourListService: TourListService,
    private importExportService: ImportExportService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.tourListService.loadTours();

    this.tourListService.tours$
      .pipe(takeUntil(this.destroy$))
      .subscribe(tours => {
        this.tours = tours;
        console.log('Tours updated:', this.tours);
        this.cdr.markForCheck();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onQueryInput(query: string): void {
    this.tourListService.query = query;
    this.tourListService.loadTours();
  }

  clearQuery(): void {
    this.tourListService.query = '';
    this.tourListService.loadTours();
  }

  exportTours(): void {
    this.isImportExportLoading = true; // Set loading state to true
    this.importExportMessage = ''; // Clear any previous messages

    this.importExportService.exportTours().subscribe({ // backend call to export tours
      next: (exportFile) => {
        const fileUrl = URL.createObjectURL(exportFile); // create a temporary URL for the exported file
        const downloadLink = document.createElement('a'); // create a temporary anchor element to trigger the download
        downloadLink.href = fileUrl;
        downloadLink.download = `tourplanner-export-${new Date().toISOString().slice(0, 10)}.json`;
        downloadLink.click(); // download the file
        URL.revokeObjectURL(fileUrl); // revoke the temporary URL to free up memory
        this.importExportMessage = 'Export completed.';
        this.isImportExportLoading = false;
      },
      error: (err) => {
        console.error('Failed to export tours: ', err);
        this.importExportMessage = 'Export failed.';
        this.isImportExportLoading = false;
      }
    });
  }

  importTours(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0]; // get the first selected file
    input.value = ''; 

    if (!file) return; // no file selected, exit early

    this.isImportExportLoading = true;
    this.importExportMessage = '';

    const reader = new FileReader();

    reader.onload = () => { // JSON parsing and sending to backend for import
      try {
        const importData = JSON.parse(reader.result as string) as ImportTourData;

        this.importExportService.importTours(importData).subscribe({
          next: (result) => {
            this.tourListService.loadTours(); // reload the tours after import
            this.importExportMessage = `Imported ${result.importedTours} tour(s).`;
            this.isImportExportLoading = false;
          },
          error: (err) => {
            console.error('Failed to import tours: ', err);
            this.importExportMessage = 'Import failed.';
            this.isImportExportLoading = false;
          }
        });
      } catch (error) {
        console.error('Invalid import file: ', error);
        this.importExportMessage = 'Invalid JSON file.';
        this.isImportExportLoading = false;
      }
    };

    reader.onerror = () => {
      this.importExportMessage = 'Could not read import file.';
      this.isImportExportLoading = false;
    };

    reader.readAsText(file);
  }
}
