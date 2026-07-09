import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Statistics, StatisticsService } from '../statistics.service';

@Component({
  selector: 'app-statistics-dashboard',
  templateUrl: './statistics-dashboard.html',
  styleUrl: './statistics-dashboard.css',
})
export class StatisticsDashboard implements OnInit {
  statistics: Statistics | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private readonly statisticsService: StatisticsService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadStatistics(); // automatically load statistics 
  }

  loadStatistics(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.statisticsService.getStatistics().subscribe({
      next: (statistics) => {
        this.statistics = statistics;
        this.isLoading = false;
        this.changeDetector.markForCheck(); // Mark for check to update the view
      },
      error: (error) => {
        console.error('Failed to load statistics: ', error);
        this.errorMessage = 'Could not load statistics.';
        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }
}
