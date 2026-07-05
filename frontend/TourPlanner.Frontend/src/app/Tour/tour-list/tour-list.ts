import { Component, EventEmitter, Output, Signal } from '@angular/core';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface'
import { TourEntry } from "./tour-entry/tour-entry";
import { RouterModule } from "@angular/router";
import { TourListService } from './tour-list-service';
import { ImportExportService, ImportTourData } from '../../Import-Export/import-export.service';

@Component({
  selector: 'app-tour-list',
  imports: [TourEntry, RouterModule],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList {
  @Output() selectTour = new EventEmitter<TourItemInterface>();
  @Output() editTour = new EventEmitter<TourItemInterface>();
  @Output() createTour = new EventEmitter<void>();

  // Tour Log events
  @Output() viewLogList = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>()

  tours: TourItemInterface[] = [];
  importExportMessage = '';
  isImportExportLoading = false;

  constructor(
    private tourListService: TourListService,
    private importExportService: ImportExportService
  ) { }

  ngOnInit() {
    this.tourListService.tours$.subscribe(tours => {
      this.tours = tours;
    });
    this.tourListService.loadTours();
  }

  onQueryInput(arg0: string) {
    this.tourListService.query = arg0;
    this.tourListService.loadTours();
  }

  clearQuery() {
    this.tourListService.query = '';
    this.tourListService.loadTours();
  }

  exportTours(): void {
    this.isImportExportLoading = true;
    this.importExportMessage = '';

    this.importExportService.exportTours().subscribe({
      next: (exportFile) => {
        const fileUrl = URL.createObjectURL(exportFile);
        const downloadLink = document.createElement('a');
        downloadLink.href = fileUrl;
        downloadLink.download = `tourplanner-export-${new Date().toISOString().slice(0, 10)}.json`;
        downloadLink.click();
        URL.revokeObjectURL(fileUrl);
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
    const file = input.files?.[0];
    input.value = '';

    if (!file) return;

    this.isImportExportLoading = true;
    this.importExportMessage = '';

    const reader = new FileReader();
    reader.onload = () => {
      try {
        const importData = JSON.parse(reader.result as string) as ImportTourData;
        this.importExportService.importTours(importData).subscribe({
          next: (result) => {
            this.tourListService.loadTours();
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
