import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api.config';

export interface ExportTourLog {
  id: string;
  timeStamp: string;
  comment?: string;
  difficulty: number | string;
  totalDistance: number;
  totalTime: number;
  rating: number;
}

export interface ExportTour {
  id: string;
  title?: string;
  description?: string;
  from: string;
  to: string;
  transportType?: number | string;
  distance: number;
  time: number;
  tourLogs: ExportTourLog[];
}

export interface ExportTourData {
  exportedAt: string;
  tours: ExportTour[];
}

export interface ImportTourLog {
  timeStamp: string;
  comment?: string;
  difficulty: number | string;
  totalDistance: number;
  totalTime: number;
  rating: number;
}

export interface ImportTour {
  title?: string;
  description?: string;
  from: string;
  to: string;
  transportType: number | string;
  distance: number;
  time: number;
  tourLogs: ImportTourLog[];
}

export interface ImportTourData {
  tours: ImportTour[];
}

export interface ImportResult {
  importedTours: number;
}

@Injectable({
  providedIn: 'root',
})
export class ImportExportService {
  private readonly importExportUrl = `${API_BASE_URL}/import-export`;

  constructor(private readonly http: HttpClient) {}

  exportTours(): Observable<Blob> { // export as Blob, so it can be downloaded as a file
    return this.http.get(`${this.importExportUrl}/export`, {
      responseType: 'blob',
    });
  }

  importTours(importData: ImportTourData): Observable<ImportResult> { // sends JSON data to the backend for import, returns the number of imported tours
    return this.http.post<ImportResult>(`${this.importExportUrl}/import`, importData);
  }
}
